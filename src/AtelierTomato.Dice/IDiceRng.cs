namespace AtelierTomato.Dice
{
	/// <summary>
	/// Interface to abstract the randomness of random number generation away.
	/// </summary>
	public interface IDiceRng
	{
		/// <summary>
		/// Generates a random number between <paramref name="from"/> and <paramref name="to"/>, the boundaries are inclusive.
		/// </summary>
		int GenerateInteger(int from, int to);
	}
}
