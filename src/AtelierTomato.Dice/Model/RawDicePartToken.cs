using AtelierTomato.Calculator.Model;

namespace AtelierTomato.Dice.Model
{
	public class RawDicePartToken : ITreeOrToken
	{
		public RawDicePartToken(string value)
		{
			this.Value = value;
		}
		public string Value { get; init; }
	}
}
