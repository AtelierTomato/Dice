namespace AtelierTomato.Dice
{
	/// <summary>
	/// Interface to abstract the randomness of random number generation away.
	/// </summary>
	public interface IDiceRng
	{
		/// <summary>
		/// Generates a random number between <paramref name="minValue"/> and <paramref name="maxValue"/>, the boundaries are inclusive.
		/// </summary>
		int GenerateInteger(int minValue, int maxValue);
	}
}
