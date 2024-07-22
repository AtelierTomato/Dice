namespace AtelierTomato.Dice
{
	public class DiceRng : IDiceRng
	{
		private readonly Random random = new();
		/// <summary>
		/// Generates a random number between <paramref name="from"/> and <paramref name="to"/>, the boundaries are inclusive.
		/// </summary>
		public int GenerateInteger(int from, int to) => random.Next(from, to + 1);
	}
}
