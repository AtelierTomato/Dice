using AtelierTomato.Calculator.Model.Nodes;
using System.Text;

namespace AtelierTomato.Dice.Model.Nodes
{
	public class DiceNode : IExpressionNode
	{
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb = sb.Append(ParenthesizeIfNeeded(Quantity)).Append("d").Append(ParenthesizeIfNeeded(Sides));

			if (ExplodeThreshold is not null)
			{
				if (ExplodeIsInfinite)
					sb = sb.Append("i");
				sb = sb.Append("e").Append(ExplodeThreshold);
				if (ExplodeIterations is not null)
					sb = sb.Append(";").Append(ParenthesizeIfNeeded(ExplodeIterations));

			}

			if (RerollThreshold is not null)
			{
				if (RerollIsInfinite)
					sb = sb.Append("i");
				sb = sb.Append("r").Append(ParenthesizeIfNeeded(RerollThreshold));
				if (RerollIterations is not null)
					sb = sb.Append(";").Append(ParenthesizeIfNeeded(RerollIterations));

			}

			if (DropLowestAmount is not null)
			{
				sb = sb.Append("p").Append(ParenthesizeIfNeeded(DropLowestAmount));
			}

			if (KeepHighestAmount is not null)
			{
				sb = sb.Append("k").Append(ParenthesizeIfNeeded(KeepHighestAmount));
			}

			if (TargetThreshold is not null)
			{
				sb = sb.Append("t").Append(ParenthesizeIfNeeded(TargetThreshold));
			}

			if (FailureThreshold is not null)
			{
				sb = sb.Append("f").Append(ParenthesizeIfNeeded(FailureThreshold));
			}

			if (SortDescending)
				sb = sb.Append("o");

			if (DiceDisplayBehavior == DiceDisplayBehavior.Hide)
				sb = sb.Append("h");

			if (DiceDisplayBehavior == DiceDisplayBehavior.Show)
				sb = sb.Append("s");

			if (DiceVerbosity == DiceVerbosity.Quiet)
				sb = sb.Append("q");

			if (DiceVerbosity == DiceVerbosity.Verbose)
				sb = sb.Append("v");

			return sb.ToString();
		}

		private static string ParenthesizeIfNeeded(IExpressionNode node) => node is NumberNode ? node.ToString() : $"({node})";

		/// <summary>
		/// Number of dice in this roll (before any subsequent evaluation).
		/// </summary>
		public IExpressionNode Quantity { get; init; }

		/// <summary>
		/// Sides of the rolled dice.
		/// </summary>
		public IExpressionNode Sides { get; init; }

		/// <summary>
		/// Explode any dice greater than or equal to this number.
		/// </summary>
		public IExpressionNode? ExplodeThreshold { get; init; }

		/// <summary>
		/// Maximum amount of iterations of explosion caused by explosions.
		/// </summary>
		public IExpressionNode? ExplodeIterations { get; init; }

		/// <summary>
		/// Sets maximum amount of recursion for exploding to be infinite.
		/// </summary>
		public bool ExplodeIsInfinite { get; init; }

		/// <summary>
		/// Reroll any dice less than or equal to this number.
		/// </summary>
		public IExpressionNode? RerollThreshold { get; init; }

		/// <summary>
		/// Maximum amount of iterations of reroll caused by rerolls.
		/// </summary>
		public IExpressionNode? RerollIterations { get; init; }

		/// <summary>
		/// Sets maximum amount of recursion for rerolling to be infinite.
		/// </summary>
		public bool RerollIsInfinite { get; init; }

		/// <summary>
		/// Drop the lowest n dice.
		/// </summary>
		public IExpressionNode? DropLowestAmount { get; init; }

		/// <summary>
		/// Keep the highest n dice.
		/// </summary>
		public IExpressionNode? KeepHighestAmount { get; init; }

		/// <summary>
		/// Count any dice greater than or equal to this number as success.
		/// </summary>
		public IExpressionNode? TargetThreshold { get; init; }

		/// <summary>
		/// Count any dice less than or equal to this number as failure.
		/// </summary>
		public IExpressionNode? FailureThreshold { get; init; }

		/// <summary>
		/// If <see langword="true"/>, order dice rolls in descending order.
		/// </summary>
		public bool SortDescending { get; init; }

		/// <summary>
		/// Controls display of this roll in output.
		/// </summary>
		public DiceDisplayBehavior DiceDisplayBehavior { get; init; }

		/// <summary>
		/// Controls display of the dice of this roll in output.
		/// </summary>
		public DiceVerbosity DiceVerbosity { get; init; }


		public DiceNode(
				IExpressionNode quantity,
				IExpressionNode sides,
				IExpressionNode? explodeThreshold,
				IExpressionNode? explodeIterations,
				bool explodeIsInfinite,
				IExpressionNode? rerollThreshold,
				IExpressionNode? rerollIterations,
				bool rerollIsInfinite,
				IExpressionNode? dropLowestAmount,
				IExpressionNode? keepHighestAmount,
				IExpressionNode? targetThreshold,
				IExpressionNode? failureThreshold,
				bool sortDescending,
				DiceDisplayBehavior diceDisplayBehavior,
				DiceVerbosity diceVerbosity)
		{
			Quantity = quantity;
			Sides = sides;
			ExplodeThreshold = explodeThreshold;
			ExplodeIterations = explodeIterations;
			ExplodeIsInfinite = explodeIsInfinite;
			RerollThreshold = rerollThreshold;
			RerollIterations = rerollIterations;
			RerollIsInfinite = rerollIsInfinite;
			DropLowestAmount = dropLowestAmount;
			KeepHighestAmount = keepHighestAmount;
			TargetThreshold = targetThreshold;
			FailureThreshold = failureThreshold;
			SortDescending = sortDescending;
			DiceDisplayBehavior = diceDisplayBehavior;
			DiceVerbosity = diceVerbosity;
		}
	}
}
