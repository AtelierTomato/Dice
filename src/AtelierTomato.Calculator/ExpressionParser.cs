using System.Globalization;
using System.Text.RegularExpressions;
using AtelierTomato.Calculator.Model;
using AtelierTomato.Calculator.Model.Nodes;

namespace AtelierTomato.Calculator
{
	/// <summary>
	/// Parses expression texts into expression trees.
	/// </summary>
	public class ExpressionParser
	{
		private readonly Regex operatorRegex = new(@"[+\-*×∙/÷(^]", RegexOptions.Compiled);
		private readonly Regex numberPieceRegex = new(@"[\d.]", RegexOptions.Compiled);

		private readonly IReadOnlyDictionary<string, Func<IExpressionNode, IExpressionNode, IExpressionNode>> dyadicMultiplicationLevelOperatorConstructors = new Dictionary<string, Func<IExpressionNode, IExpressionNode, IExpressionNode>>
		{
			["*"] = (left, right) => new MultiplyNode(left, right),
			["×"] = (left, right) => new MultiplyNode(left, right),
			["∙"] = (left, right) => new MultiplyNode(left, right),
			["/"] = (left, right) => new DivideNode(left, right),
			["÷"] = (left, right) => new DivideNode(left, right),
		};

		private readonly IReadOnlyDictionary<string, Func<IExpressionNode, IExpressionNode, IExpressionNode>> dyadicAdditionLevelOperatorConstructors = new Dictionary<string, Func<IExpressionNode, IExpressionNode, IExpressionNode>>
		{
			["+"] = (left, right) => new AddNode(left, right),
			["-"] = (left, right) => new SubtractNode(left, right),
		};

		/// <summary>
		/// Parse the given <paramref name="expressionText"/> into an Expression tree.
		/// </summary>
		/// <param name="expressionText">The given text.</param>
		/// <returns>An expression tree.</returns>
		/// <exception cref="ParseException">Something went wrong during parsing.</exception>
		public IExpressionNode Parse(string expressionText)
		{
			var tokensListFirst = this.Tokenize(expressionText);
			ValidateParentheses(tokensListFirst);
			tokensListFirst = Normalize(tokensListFirst);
			return this.CreateExpressionTree(tokensListFirst);
		}

		/// <summary>
		/// Tokenizes the given <paramref name="expressionText"/> into a Linked List of tokens for further parsing.
		/// </summary>
		/// <param name="expressionText">The given text.</param>
		/// <returns>A linked list of tokens.</returns>
		/// <exception cref="ParseException">When there are invalid characters in the input.</exception>
		public DoubleLinkedListItem<ITreeOrToken> Tokenize(string expressionText)
		{
			DoubleLinkedListItem<ITreeOrToken>? last = null;

			for (int i = 0; i < expressionText.Length; i++)
			{
				switch (expressionText.Substring(i, 1))
				{
					case "(":
						last = DoubleLinkedListItem<ITreeOrToken>.AppendToOrCreate(last, new OpenParenthesisToken());
						break;
					case ")":
						last = DoubleLinkedListItem<ITreeOrToken>.AppendToOrCreate(last, new CloseParenthesisToken());
						break;
					case string rawOperator when operatorRegex.IsMatch(rawOperator):
						last = DoubleLinkedListItem<ITreeOrToken>.AppendToOrCreate(last, new RawOperatorToken(rawOperator));
						break;
					case string number when numberPieceRegex.IsMatch(number):
						while (i + 1 < expressionText.Length && numberPieceRegex.IsMatch(expressionText.AsSpan(i + 1, 1)))
						{
							number += expressionText.Substring(i + 1, 1);
							++i;
						}
						last = DoubleLinkedListItem<ITreeOrToken>.AppendToOrCreate(last, new NumberNode(double.Parse(number, new CultureInfo("en-us"))));
						break;
					case string space when Regex.IsMatch(space, @"\s"):
						// ignore
						break;
					default:
						throw new ParseException($@"the symbol '{expressionText.Substring(i, 1)}' at position {i} is not recognized as maths!", expressionText);
				}
			}

			if (last is null) throw new ParseException($@"there was nothing in the calculation!");

			return last.First;
		}

		/// <summary>
		/// Normalizes the given input. This will mutate the given parameter.<br/>
		/// This means adding implicit multiplication and replacing '-'-operators with negations.
		/// </summary>
		/// <param name="tokensListFirst">A linked list of tokens. Will get mutated by this method.</param>
		/// <returns>The resulting linked list of tokens.</returns>
		/// <exception cref="ParseException">When there is a structural error in the given list of tokens.</exception>
		public static DoubleLinkedListItem<ITreeOrToken> Normalize(DoubleLinkedListItem<ITreeOrToken> tokensListFirst)
		{
			for (var tokenEntry = tokensListFirst; tokenEntry?.Next is not null; tokenEntry = tokenEntry.Next)
			{
				if ((tokenEntry.Value is NumberNode thisNumber) && (tokenEntry.Next.Value is NumberNode nextNumber))
					throw new ParseException($@"the two numbers {thisNumber.Value} and {nextNumber.Value} are not allowed to follow each other without an operation!!", tokensListFirst);
				else if (tokenEntry.Value is CloseParenthesisToken or NumberNode && tokenEntry.Next.Value is OpenParenthesisToken or NumberNode)
				{
					// add implied multiplication operator
					tokenEntry.InsertAfterThis(new RawOperatorToken("*"));
				}
				else if (tokenEntry.Value is RawOperatorToken { Value: "-" })
				{
					var previousIsNotPartOfSubtraction = tokenEntry.Previous?.Value is null or (not NumberNode and not CloseParenthesisToken);
					var nextIsNegatable = tokenEntry.Next.Value is NumberNode or OpenParenthesisToken;
					if (previousIsNotPartOfSubtraction && nextIsNegatable)
					{
						tokenEntry = tokenEntry.ReplaceThis(new NegationToken());
						if (tokenEntry.Previous is null) tokensListFirst = tokenEntry;
					}
				}
			}
			return tokensListFirst;
		}

		/// <summary>
		/// Verify the parentheses in the given input.
		/// </summary>
		/// <param name="tokensListFirst">A linked list of tokens.</param>
		/// <exception cref="ParseException">When there is an error in the structure of parentheses.</exception>
		private static void ValidateParentheses(DoubleLinkedListItem<ITreeOrToken> tokensListFirst)
		{
			int depth = 0;
			var position = 0;
			for (var token = tokensListFirst; token != null; token = token.Next)
			{
				position++;
				depth += token.Value switch { OpenParenthesisToken => 1, CloseParenthesisToken => -1, _ => 0 };
				if (depth < 0)
					throw new ParseException($@"the Parenthesis at position {position} is not allowed, there's not enough opened parentheses!", tokensListFirst);
				if (token.Value is CloseParenthesisToken && token.Previous!.Value is OpenParenthesisToken)
					throw new ParseException($@"there's nothing between the two parentheses at position {position - 1} and {position}!", tokensListFirst);
			}
			if (depth > 0)
				throw new ParseException("please close your parentheses!", tokensListFirst);
		}

		/// <summary>
		/// Builds the expression tree from the given tokens.
		/// </summary>
		/// <param name="tokensListFirst">A linked list of tokens.</param>
		private IExpressionNode CreateExpressionTree(DoubleLinkedListItem<ITreeOrToken> tokensListFirst)
		{
			tokensListFirst = tokensListFirst.InsertBeforeThis(new OpenParenthesisToken());
			_ = tokensListFirst.Last.InsertAfterThis(new CloseParenthesisToken());

			while (tokensListFirst != tokensListFirst.Last)
			{
				var parenthesisPair = FindParenthesisPair(tokensListFirst) ?? throw new ParseException("somehow a parenthesis went missing!");
				var remainingListItem = ProcessParenthesisPair(parenthesisPair.start, parenthesisPair.end);

				if (tokensListFirst == parenthesisPair.start) tokensListFirst = remainingListItem;
			}

			return (IExpressionNode)tokensListFirst.Value;
		}

		private static (DoubleLinkedListItem<ITreeOrToken> start, DoubleLinkedListItem<ITreeOrToken> end)? FindParenthesisPair(DoubleLinkedListItem<ITreeOrToken> tokensListFirst)
		{
			var closeToken = tokensListFirst.FindValueForward(t => t is CloseParenthesisToken);
			if (closeToken is null) return null;

			var openToken = closeToken.FindValueBackward(t => t is OpenParenthesisToken);
			if (openToken is null) throw new ParseException("somehow an opening parenthesis went missing!");
			return (openToken, closeToken);
		}

		/// <summary>
		/// Processes an entire parenthesis pair into a single linked list item with a tree node as its value.
		/// </summary>
		private DoubleLinkedListItem<ITreeOrToken> ProcessParenthesisPair(DoubleLinkedListItem<ITreeOrToken> parenthesisPairStart, DoubleLinkedListItem<ITreeOrToken> parenthesisPairEnd)
		{
			ProcessPowers(parenthesisPairStart.Next!, parenthesisPairEnd.Previous!);
			ProcessNegation(parenthesisPairStart.Next!, parenthesisPairEnd.Previous!);
			ProcessLeftToRightDyadicOperators(parenthesisPairStart.Next!, parenthesisPairEnd.Previous!, dyadicMultiplicationLevelOperatorConstructors);
			ProcessLeftToRightDyadicOperators(parenthesisPairStart.Next!, parenthesisPairEnd.Previous!, dyadicAdditionLevelOperatorConstructors);
			if (parenthesisPairStart.Next != parenthesisPairEnd.Previous)
				throw new ParseException("somehow there were multiple leftover nodes after parsing!");

			var remainingNode = parenthesisPairStart.Next!;
			parenthesisPairStart.RemoveThis();
			parenthesisPairEnd.RemoveThis();
			return remainingNode;
		}

		private static void ProcessNegation(DoubleLinkedListItem<ITreeOrToken> tokenListStart, DoubleLinkedListItem<ITreeOrToken> tokenListEnd)
		{
			for (DoubleLinkedListItem<ITreeOrToken>? item = tokenListStart; item != tokenListEnd.Next; item = item.Next)
			{
				if (item!.Value is NegationToken)
				{
					var nextItem = item.Next;
					if (nextItem is null || nextItem.Value is not IExpressionNode nextExpression)
					{
						throw new ParseException("somehow a negation was not attached to a number or a parenthesis!", tokenListStart);
					}
					nextItem.RemoveThis();
					var negationItem = item.ReplaceThis(new NegationNode(nextExpression));
					if (tokenListStart == item) tokenListStart = negationItem;
					if (tokenListEnd == nextItem) tokenListEnd = negationItem;
					item = negationItem;
				}
			}
		}

		private static void ProcessLeftToRightDyadicOperators(DoubleLinkedListItem<ITreeOrToken> tokenListStart, DoubleLinkedListItem<ITreeOrToken> tokenListEnd, IReadOnlyDictionary<string, Func<IExpressionNode, IExpressionNode, IExpressionNode>> operatorConstructors)
		{
			for (DoubleLinkedListItem<ITreeOrToken>? item = tokenListStart; item != tokenListEnd.Next; item = item.Next)
			{
				if (item!.Value is RawOperatorToken rawOperatorToken && operatorConstructors.ContainsKey(rawOperatorToken.Value))
				{
					var leftItem = item.Previous;
					var rightItem = item.Next;
					if (leftItem is null || leftItem.Value is not IExpressionNode previousExpression || rightItem is null || rightItem.Value is not IExpressionNode nextExpression)
					{
						throw new ParseException("somehow an operation was not attached to a number or a parenthesis!", tokenListStart);
					}
					leftItem.RemoveThis();
					rightItem.RemoveThis();
					var operatorNode = item.ReplaceThis(operatorConstructors[rawOperatorToken.Value](previousExpression, nextExpression));
					if (tokenListStart == leftItem) tokenListStart = operatorNode;
					if (tokenListEnd == rightItem) tokenListEnd = operatorNode;
					item = operatorNode;
				}
			}
		}

		private static void ProcessRightToLeftDyadicOperators(DoubleLinkedListItem<ITreeOrToken> tokenListStart, DoubleLinkedListItem<ITreeOrToken> tokenListEnd, IReadOnlyDictionary<string, Func<IExpressionNode, IExpressionNode, IExpressionNode>> operatorConstructors)
		{
			for (DoubleLinkedListItem<ITreeOrToken>? item = tokenListEnd; item != tokenListStart.Previous; item = item.Previous)
			{
				if (item!.Value is RawOperatorToken rawOperatorToken && operatorConstructors.ContainsKey(rawOperatorToken.Value))
				{
					var leftItem = item.Previous;
					var rightItem = item.Next;
					if (leftItem is null || leftItem.Value is not IExpressionNode previousExpression || rightItem is null || rightItem.Value is not IExpressionNode nextExpression)
					{
						throw new ParseException("somehow an operation was not attached to a number or a parenthesis!", tokenListStart);
					}
					leftItem.RemoveThis();
					rightItem.RemoveThis();
					var operatorNode = item.ReplaceThis(operatorConstructors[rawOperatorToken.Value](previousExpression, nextExpression));
					if (tokenListStart == leftItem) tokenListStart = operatorNode;
					if (tokenListEnd == rightItem) tokenListEnd = operatorNode;
					item = operatorNode;
				}
			}
		}

		private static void ProcessPowers(DoubleLinkedListItem<ITreeOrToken> tokenListStart, DoubleLinkedListItem<ITreeOrToken> tokenListEnd)
		{
			for (DoubleLinkedListItem<ITreeOrToken>? item = tokenListEnd; item != tokenListStart.Previous; item = item.Previous)
			{
				if (item!.Value is RawOperatorToken { Value: "^" })
				{
					var baseItem = item.Previous;
					var rawExponentItem = item.Next;

					// there may be unparsed negation in the exponent
					if (baseItem is null || baseItem.Value is not IExpressionNode previousExpression || rawExponentItem is null || rawExponentItem.Value is not IExpressionNode and not NegationToken)
					{
						throw new ParseException("somehow an operation was not attached to a number or a parenthesis!", tokenListStart);
					}

					// negation expression may be trapped in the exponent -> process here "early".
					// since rawExponentItem may be the final item in the list, do not search in that case.
					ProcessNegation(rawExponentItem, rawExponentItem.FindItemForward(endItem => endItem.Next is null || endItem.Next.Value is CloseParenthesisToken or RawOperatorToken)!);

					var exponentItem = item.Next;

					if (exponentItem is null || exponentItem.Value is not IExpressionNode nextExpression)
					{
						throw new ParseException("somehow an operation was not attached to a number or a parenthesis!", tokenListStart);
					}
					baseItem.RemoveThis();
					exponentItem.RemoveThis();
					var operatorNode = item.ReplaceThis(new PowNode(previousExpression, nextExpression));
					if (tokenListStart == baseItem) tokenListStart = operatorNode;
					if (tokenListEnd == exponentItem) tokenListEnd = operatorNode;
					item = operatorNode;
				}
			}
		}
	}
}
