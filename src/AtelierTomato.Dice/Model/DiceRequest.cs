using System.Text;

namespace AtelierTomato.Dice.Model
{
	public record DiceRequest(
		bool IsNegative,
		DicePartValue Quantity,
		DicePartValue Sides,
		DicePartValue? ExplodeThreshold,
		DicePartValue? ExplodeIterations,
		bool ExplodeIsInfinite,
		DicePartValue? RerollThreshold,
		DicePartValue? RerollIterations,
		bool RerollIsInfinite,
		DicePartValue? DropLowestAmount,
		DicePartValue? KeepHighestAmount,
		DicePartValue? TargetThreshold,
		DicePartValue? FailureThreshold,
		bool SortDescending,
		DiceDisplayBehavior DiceDisplayBehavior,
		DiceVerbosity DiceVerbosity)
	{
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb = sb.Append(Quantity.SourceValue).Append("d").Append(Sides.SourceValue);

			if (ExplodeThreshold is not null)
			{
				if (ExplodeIsInfinite)
					sb = sb.Append("i");
				sb = sb.Append("e").Append(ExplodeThreshold.SourceValue);
				if (ExplodeIterations is not null)
					sb = sb.Append(";").Append(ExplodeIterations.SourceValue);

			}

			if (RerollThreshold is not null)
			{
				if (RerollIsInfinite)
					sb = sb.Append("i");
				sb = sb.Append("r").Append(RerollThreshold.SourceValue);
				if (RerollIterations is not null)
					sb = sb.Append(";").Append(RerollIterations.SourceValue);

			}

			if (DropLowestAmount is not null)
			{
				sb = sb.Append("p").Append(DropLowestAmount.SourceValue);
			}

			if (KeepHighestAmount is not null)
			{
				sb = sb.Append("k").Append(KeepHighestAmount.SourceValue);
			}

			if (TargetThreshold is not null)
			{
				sb = sb.Append("t").Append(TargetThreshold.SourceValue);
			}

			if (FailureThreshold is not null)
			{
				sb = sb.Append("f").Append(FailureThreshold.SourceValue);
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
	}
}
