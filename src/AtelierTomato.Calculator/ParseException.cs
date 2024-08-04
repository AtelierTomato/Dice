using AtelierTomato.Calculator.Model;

namespace AtelierTomato.Calculator
{
	[Serializable]
	public class ParseException : Exception
	{
		public List<ITreeOrToken>? Tokens { get; init; }
		public string? ExpressionText { get; init; }
		public ParseException() { }
		public ParseException(string message) : base(message) { }
		public ParseException(string message, Exception inner) : base(message, inner) { }
		public ParseException(string message, DoubleLinkedListItem<ITreeOrToken> tokenListFirst) : base(message) { this.Tokens = tokenListFirst.ConvertToList(); }
		public ParseException(string message, string expressionText) : base(message) { this.ExpressionText = expressionText; }
		public ParseException(string message, string expressionText, Exception inner) : base(message, inner) { this.ExpressionText = expressionText; }
	}
}
