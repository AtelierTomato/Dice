using AtelierTomato.Calculator;
using AtelierTomato.Dice.Model;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Dice
{
	public class DiceRequestExecutor
	{
		private readonly IDiceRng diceRng;
		private readonly DiceOptions diceOptions;

		public DiceRequestExecutor(IDiceRng diceRng, IOptions<DiceOptions> diceOptions)
		{
			this.diceRng = diceRng;
			this.diceOptions = diceOptions.Value;
		}

		public double Execute(DiceRequest diceRequest, QueryExecutionLogBuilder queryExecutionLogBuilder)
		{
			if (diceRequest.Quantity.IsInfinite && diceRequest.TargetThreshold is not null)
			{
				queryExecutionLogBuilder.AddExpression(diceRequest, [], DicePartValue.Infinity, double.PositiveInfinity);
				return Double.PositiveInfinity;
			}

			if (diceRequest.Quantity.Value * (diceRequest.ExplodeIterations?.Value ?? 1) * (diceRequest.RerollIterations?.Value ?? 1) > 1000000)
			{
				throw new ExecuteException($"Attempted DoS with over 1000000 dice detected. {diceRequest}");
			}

			IReadOnlyList<DicePartValue> rolls = this.PerformRolls(diceRequest).ToArray();

			// Keep an amount of dice of the highest value
			if (diceRequest is { KeepHighestAmount: not null } or { DropLowestAmount: not null })
			{
				rolls = DropAndKeep(rolls, diceRequest.DropLowestAmount, diceRequest.KeepHighestAmount).ToArray();
			}

			int? targetCounter = null;
			if (diceRequest
				is { TargetThreshold: not null }
				or { FailureThreshold: not null })
			{
				targetCounter = 0;
				foreach (var roll in rolls)
				{
					if (diceRequest.TargetThreshold is not null && roll >= diceRequest.TargetThreshold)
					{
						targetCounter++;
					}
					else if (diceRequest.FailureThreshold is not null && roll <= diceRequest.FailureThreshold)
					{
						targetCounter--;
					}
				}
			}

			if (diceRequest.SortDescending)
			{
				rolls = rolls.OrderByDescending(x => x).ToArray();
			}

			var result = targetCounter switch
			{
				null => rolls.Select(r => r.IsInfinite ? double.PositiveInfinity : r.Value!.Value).Sum(),
				_ => targetCounter.Value,
			};

			queryExecutionLogBuilder.AddExpression(diceRequest, rolls, targetCounter, result);

			return result;
		}

		private static IEnumerable<DicePartValue> DropAndKeep(IEnumerable<DicePartValue> rolls, DicePartValue? dropAmount, DicePartValue? keepAmount)
		{
			if (dropAmount is { IsInfinite: true }) return [];

			var indexedRollsMinusDrops = rolls.Select((roll, index) => (roll, index))
				.OrderBy(t => t.roll)
				.Skip(dropAmount?.Value!.Value ?? 0);

			var indexedRollsKept = keepAmount switch
			{
				null or { IsInfinite: true } => indexedRollsMinusDrops,
				_ => indexedRollsMinusDrops.Reverse().Take(keepAmount.Value!.Value),
			};

			return indexedRollsKept.OrderBy(r => r.index).Select(r => r.roll);
		}

		public IEnumerable<DicePartValue> PerformRolls(DiceRequest diceRequest)
		{
			// if we have an infinite quantity of dice, cap if infinity is broken by other directives
			var cappedQuantity = diceRequest switch
			{
				{ Quantity.IsInfinite: false } => diceRequest.Quantity,
				{ KeepHighestAmount.IsInfinite: false } => diceRequest.KeepHighestAmount,
				_ => diceRequest.Quantity,
			};

			// todo replacing-with-0/infinity is not great though
			if (cappedQuantity.IsInfinite)
			{
				yield return new DicePartValue(double.PositiveInfinity);
				yield break;
			}

			for (int i = 0; i < cappedQuantity; i++)
			{
				var roll = RollOne(diceRequest);
				roll = ExplodeRoll(diceRequest, roll);
				roll = Reroll(diceRequest, roll);
				yield return roll;
			}
		}

		/// <summary>
		/// If the roll's value is over a certain value, rolls the dice again and then adds that value to the final roll, any successive rolls can also explode. This either happens a set number of times or indefinitely.
		/// </summary>
		/// <param name="diceRequest">A group of parameters for dice rolling.</param>
		/// <param name="initialRoll">The initially rolled value.</param>
		/// <returns></returns>
		private DicePartValue ExplodeRoll(DiceRequest diceRequest, DicePartValue initialRoll)
		{
			if (initialRoll.IsInfinite) return initialRoll;
			if (diceRequest.ExplodeThreshold is null) return initialRoll;
			if (diceRequest.ExplodeIsInfinite && diceRequest.ExplodeThreshold <= 1) return new DicePartValue(double.PositiveInfinity);

			var explodeIterations = diceRequest.ExplodeIsInfinite ? new DicePartValue(double.PositiveInfinity) : diceRequest.ExplodeIterations ?? diceOptions.DefaultExplosionRecursions;

			var total = initialRoll;
			var lastRoll = initialRoll;
			while (explodeIterations > 0 && lastRoll >= diceRequest.ExplodeThreshold)
			{
				var currentRoll = RollOne(diceRequest);
				total = new DicePartValue(total.Value!.Value + currentRoll.Value!.Value);
				lastRoll = currentRoll;
				explodeIterations = explodeIterations.IsInfinite ? explodeIterations : new DicePartValue(explodeIterations.Value!.Value - 1);
			}
			return total;
		}

		/// <summary>
		/// Rerolls the dice if they are under a certain value. This either happens a set number of times or indefinitely.
		/// </summary>
		/// <param name="diceRequest">A group of parameters for dice rolling.</param>
		/// <param name="initialRoll">The initially rolled value.</param>
		/// <returns></returns>
		private DicePartValue Reroll(DiceRequest diceRequest, DicePartValue initialRoll)
		{
			if (diceRequest.RerollThreshold is null) return initialRoll;
			if (initialRoll > diceRequest.RerollThreshold) return initialRoll;

			var rerollIterations = diceRequest.RerollIsInfinite ? DicePartValue.Infinity : diceRequest.RerollIterations ?? diceOptions.DefaultRerolls;
			if (rerollIterations == 0)
			{
				return initialRoll;
			}

			if (!rerollIterations.IsInfinite)
			{
				for (int i = 1; i < rerollIterations.Value!.Value; i++)
				{
					var reroll = ExplodeRoll(diceRequest, RollOne((diceRequest)));
					if (reroll > diceRequest.RerollThreshold) return reroll;
				}
				return ExplodeRoll(diceRequest, RollOne((diceRequest)));
			}

			// to avoid getting trapped in an infinite reroll, perform a sanity check.
			var maximumExplodedSingleRoll = CalculateMaximumExplodedSingleRoll(diceRequest);
			if (diceRequest.RerollThreshold.IsInfinite || maximumExplodedSingleRoll <= diceRequest.RerollThreshold)
			{
				// just pretend we ran out of patience and ended up on the highest value we can get
				return maximumExplodedSingleRoll;
			}

			if (diceRequest.ExplodeThreshold is null || diceRequest.ExplodeThreshold > diceRequest.Sides) return RollOne(diceRequest, diceRequest.RerollThreshold.Value!.Value + 1);
			var explosionChainRerollResult = 0;
			while (explosionChainRerollResult <= diceRequest.RerollThreshold)
			{
				var directThreshold = diceRequest.RerollThreshold.Value!.Value - explosionChainRerollResult;
				var explodeThreshold = diceRequest.ExplodeThreshold!.Value!.Value;
				var requiredRoll = Math.Min(directThreshold, explodeThreshold);
				var addedDie = RollOne(diceRequest, Math.Max(1, requiredRoll));
				if (addedDie > directThreshold && addedDie >= explodeThreshold)
				{
					// do one last explosion if the final die that got past the limit was in explosion range
					addedDie = ExplodeRoll(diceRequest, addedDie);
				}
				if (addedDie.IsInfinite)
				{
					return DicePartValue.Infinity;
				}
				explosionChainRerollResult += addedDie.Value!.Value;
			}
			return explosionChainRerollResult;
		}

		/// <summary>
		/// Determines the maximum possible value that can be rolled with the current dice parameters, factoring in explosion potential.
		/// </summary>
		/// <param name="diceRequest">A group of parameters for dice rolling.</param>
		/// <returns></returns>
		private DicePartValue CalculateMaximumExplodedSingleRoll(DiceRequest diceRequest)
		{
			if (diceRequest.Sides.IsInfinite) return DicePartValue.Infinity;
			if (diceRequest.ExplodeThreshold is null) return diceRequest.Sides;
			if (diceRequest.ExplodeThreshold > diceRequest.Sides) return diceRequest.Sides;
			var explodeIterations = diceRequest.ExplodeIsInfinite ? DicePartValue.Infinity : diceRequest.ExplodeIterations ?? diceOptions.DefaultExplosionRecursions;

			return explodeIterations switch
			{
				{ IsInfinite: true } => DicePartValue.Infinity,
				{ Value: <= 0 } => diceRequest.Sides,
				_ => (explodeIterations.Value!.Value + 1) * diceRequest.Sides.Value!.Value
			};
		}

		/// <summary>
		/// Generates a number based on the amount of sides that the die has.
		/// </summary>
		/// <param name="diceRequest">A group of parameters for dice rolling.</param>
		/// <param name="minimumRoll">Minimum valid dice roll in the current scenario.</param>
		/// <returns></returns>
		private DicePartValue RollOne(DiceRequest diceRequest, int? minimumRoll = null)
		{
			if (diceRequest.Sides.IsInfinite) return new DicePartValue(double.PositiveInfinity);
			if (diceRequest.Sides == 0) return new DicePartValue(0);
			if (minimumRoll <= 0) throw new ArgumentOutOfRangeException(nameof(minimumRoll), "Dice start at 1 or higher.");

			if (minimumRoll is null)
			{
				if (diceRequest.RerollIsInfinite
					&& diceRequest.RerollThreshold is not null
					&& (diceRequest.RerollThreshold <= diceRequest.Sides))
				{
					minimumRoll = diceRequest.RerollThreshold.Value!;
				}
				else
				{
					minimumRoll = 1;
				}
			}
			var rolledValue = this.diceRng.GenerateInteger(minimumRoll.Value, diceRequest.Sides.Value!.Value);
			return new DicePartValue(rolledValue);
		}
	}
}
