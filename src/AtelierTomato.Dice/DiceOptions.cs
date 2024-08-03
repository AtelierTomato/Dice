using AtelierTomato.Dice.Settings;

namespace AtelierTomato.Dice
{
	public class DiceOptions
	{
		public int DefaultExplosionRecursions { get; set; }
		public int DefaultRerolls { get; set; }
		public DiceExpressionFormat DefaultDiceExpressionFormat { get; set; }
		public DiceQueryFormat DefaultDiceQueryFormat { get; set; }
		public int DefaultDiceOutputCutoff { get; set; }
	}
}
