namespace AtelierTomato.Dice
{
	public class DiceRng : IDiceRng
	{
		private readonly Random random = new();
		/// <summary>
		/// Generates a random number between <paramref name="minValue"/> and <paramref name="maxValue"/>, the boundaries are inclusive.
		/// </summary>
		public int GenerateInteger(int minValue, int maxValue) => random.Next(minValue, maxValue + 1);
	}
}
