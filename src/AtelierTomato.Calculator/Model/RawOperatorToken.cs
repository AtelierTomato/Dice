namespace AtelierTomato.Calculator.Model
{
	public class RawOperatorToken : ITreeOrToken
	{
		public RawOperatorToken(string value)
		{
			this.Value = value;
		}
		public string Value { get; init; }
	}
}
