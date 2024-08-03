using System.Globalization;
using System.Text;
using AtelierTomato.Dice.Model;
using AtelierTomato.Dice.Settings;

namespace AtelierTomato.Dice
{
	public class QueryExecutionLogBuilder
	{
		private readonly DiceQueryFormat diceQueryFormat;
		private readonly DiceExpressionFormat diceExpressionFormat;
		private readonly int diceOutputCutoff;

		private readonly Stack<(DiceDisplayBehavior diceDisplayBehavior, string diceLog)> diceLogs = new();

		public QueryExecutionLogBuilder(DiceQueryFormat diceQueryFormat, DiceExpressionFormat diceExpressionFormat,
			int diceOutputCutoff)
		{
			if (diceExpressionFormat == DiceExpressionFormat.Inherit) throw new ArgumentException("Dice expression format needs to have had inheritance resolved!", nameof(diceExpressionFormat));
			if (diceQueryFormat == DiceQueryFormat.Inherit) throw new ArgumentException("Dice query format needs to have had inheritance resolved!", nameof(diceExpressionFormat));

			this.diceQueryFormat = diceQueryFormat;
			this.diceExpressionFormat = diceExpressionFormat;
			this.diceOutputCutoff = diceOutputCutoff;
		}

		public void AddExpression(DiceRequest diceRequest, IReadOnlyList<DicePartValue> rolls, DicePartValue? targetCounter, double result)
		{
			this.diceLogs.Push((diceRequest.DiceDisplayBehavior, BuildExpressionEntry(diceRequest, rolls, targetCounter, result)));
		}

		private string BuildExpressionEntry(DiceRequest diceRequest, IReadOnlyList<DicePartValue> rolls, DicePartValue? targetCounter, double result)
		{
			var excludeRollList = (diceRequest.DiceVerbosity, diceExpressionFormat) switch
			{
				(DiceVerbosity.Quiet, _) => true,
				(DiceVerbosity.Verbose, _) => false,
				(DiceVerbosity.Default, DiceExpressionFormat.TotalOnly) => true,
				(DiceVerbosity.Default, DiceExpressionFormat.AllRolls) => false,
				(DiceVerbosity.Default, DiceExpressionFormat.LimitedRolls) => rolls.Count > diceOutputCutoff,
				_ => throw new NotImplementedException(),
			};

			// Create a custom NumberFormatInfo
			var infinityNumberFormat = new NumberFormatInfo
			{
				PositiveInfinitySymbol = "∞",
				NegativeInfinitySymbol = "-∞"
			};

			// Create a custom CultureInfo and set its NumberFormat
			var infinityCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
			infinityCulture.NumberFormat = infinityNumberFormat;

			// todo replace this with collection patterns when those get released
			if (targetCounter is not null)
			{
				if (excludeRollList)
				{
					return $"`{diceRequest}`: Hit: {result.ToString(infinityNumberFormat)}";
				}
				else if (!rolls.Any())
				{
					return $"`{diceRequest}`: Hit: {result.ToString(infinityNumberFormat)}";
				}
				else
				{
					return $"`{diceRequest}`: `[{string.Join(", ", rolls)}]` Hit: {result.ToString(infinityNumberFormat)}";
				}
			}
			else if (!rolls.Any())
			{
				return $"`{diceRequest}`: 0, there's no dice!";
			}
			else if (rolls.Count == 1 || excludeRollList)
			{
				return $"`{diceRequest}`: {result.ToString(infinityNumberFormat)}";
			}
			else
			{
				return $"`{diceRequest}`: `[{string.Join(", ", rolls)}]` Sum: {result.ToString(infinityNumberFormat)}";
			}
		}

		public string Build()
		{
			var outputStringBuilder = new StringBuilder();
			bool isLast = true;
			while (diceLogs.TryPop(out var logItem))
			{
				outputStringBuilder = (logItem.diceDisplayBehavior, diceQueryFormat) switch
				{
					(DiceDisplayBehavior.Hide, _) => outputStringBuilder,
					(DiceDisplayBehavior.Show, _) => outputStringBuilder.Insert(0, Environment.NewLine).Insert(0, logItem.diceLog),
					(DiceDisplayBehavior.Default, DiceQueryFormat.ResultOnly) => outputStringBuilder,
					(DiceDisplayBehavior.Default, DiceQueryFormat.AllDice) => outputStringBuilder.Insert(0, Environment.NewLine).Insert(0, logItem.diceLog),
					(DiceDisplayBehavior.Default, DiceQueryFormat.LastDie) => isLast
						? outputStringBuilder.Insert(0, Environment.NewLine).Insert(0, logItem.diceLog)
						: outputStringBuilder,
					_ => throw new NotImplementedException(),
				};
				isLast = false;
			}

			return outputStringBuilder.ToString().TrimEnd();
		}
	}
}
