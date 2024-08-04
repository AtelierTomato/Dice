using AtelierTomato.Calculator.Model;

namespace AtelierTomato.Calculator
{
	[Serializable]
	public class ExecuteException : Exception
	{
		public List<ITreeOrToken>? Tokens { get; init; }
		public string? ExpressionText { get; init; }
		public ExecuteException() { }
		public ExecuteException(string message) : base(message) { }
		public ExecuteException(string message, Exception inner) : base(message, inner) { }
		public ExecuteException(string message, DoubleLinkedListItem<ITreeOrToken> tokenListFirst) : base(message) { this.Tokens = DoubleLinkedListItem<ITreeOrToken>.ConvertToList(tokenListFirst); }
		public ExecuteException(string message, string expressionText) : base(message) { this.ExpressionText = expressionText; }
		public ExecuteException(string message, string expressionText, Exception inner) : base(message, inner) { this.ExpressionText = expressionText; }
	}
}
