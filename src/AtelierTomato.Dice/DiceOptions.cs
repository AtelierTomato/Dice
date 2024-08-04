using AtelierTomato.Dice.Settings;

namespace AtelierTomato.Dice
{
	public class DiceOptions
	{
		public int DefaultExplosionRecursions { get; set; } = 10;
		public int DefaultRerolls { get; set; } = 10;
		public DiceExpressionFormat DefaultDiceExpressionFormat { get; set; } = DiceExpressionFormat.LimitedRolls;
		public DiceQueryFormat DefaultDiceQueryFormat { get; set; } = DiceQueryFormat.LastDie;
		public int DefaultDiceOutputCutoff { get; set; } = 15;
	}
}
