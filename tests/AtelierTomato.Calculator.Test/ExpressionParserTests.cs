using AtelierTomato.Calculator.Model;
using AtelierTomato.Calculator.Model.Nodes;
using FluentAssertions;

namespace AtelierTomato.Calculator.Test
{
	public class MathsOnlyExpressionParserTests
	{

		[Fact]
		public void TokenizeSimpleTest()
		{
			var target = new ExpressionParser();
			string input = "1+2";

			var result = target.Tokenize(input);
			result.Should().NotBeNull().And.BeOfType<DoubleLinkedListItem<ITreeOrToken>>();

			var resultAsList = result.ConvertToList();

			resultAsList.Should().HaveCount(3);
			resultAsList[0].Should().BeOfType<NumberNode>();
			resultAsList[1].Should().BeOfType<RawOperatorToken>();
			resultAsList[2].Should().BeOfType<NumberNode>();
		}

		[Fact]
		public void ParseSimpleTest()
		{
			var target = new ExpressionParser();
			string input = "1+2";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<AddNode>();
			var addition = (AddNode)result;

			addition.Left.Should().BeOfType<NumberNode>();
			addition.Right.Should().BeOfType<NumberNode>();
			var leftNumber = (NumberNode)addition.Left;
			var rightNumber = (NumberNode)addition.Right;
			leftNumber.Value.Should().Be(1);
			rightNumber.Value.Should().Be(2);
		}

		[Fact]
		public void ParseNegationFirstTest()
		{
			var target = new ExpressionParser();
			string input = "-1+2";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<AddNode>();
			var addition = (AddNode)result;

			addition.Left.Should().BeOfType<NegationNode>();
			addition.Right.Should().BeOfType<NumberNode>();
			var leftNegation = (NegationNode)addition.Left;
			leftNegation.Right.Should().BeOfType<NumberNode>();
			var negatedNumber = (NumberNode)leftNegation.Right;
			negatedNumber.Value.Should().Be(1);
			var rightNumber = (NumberNode)addition.Right;
			rightNumber.Value.Should().Be(2);
		}

		[Fact]
		public void ParseNegationSecondTest()
		{
			var target = new ExpressionParser();
			string input = "1+-2";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<AddNode>();
			var addition = (AddNode)result;

			addition.Left.Should().BeOfType<NumberNode>();
			addition.Right.Should().BeOfType<NegationNode>();
			var leftNumber = (NumberNode)addition.Left;
			leftNumber.Value.Should().Be(1);
			var rightNegation = (NegationNode)addition.Right;
			rightNegation.Right.Should().BeOfType<NumberNode>();
			var negatedNumber = (NumberNode)rightNegation.Right;
			negatedNumber.Value.Should().Be(2);
		}

		[Fact]
		public void ParseOrderTest()
		{
			var target = new ExpressionParser();
			string input = "1-3+2";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<AddNode>();
			var addition = (AddNode)result;

			addition.Left.Should().BeOfType<SubtractNode>();
			var subtraction = (SubtractNode)addition.Left;

			subtraction.Left.Should().BeOfType<NumberNode>();
			subtraction.Right.Should().BeOfType<NumberNode>();
			var leftSubNumber = (NumberNode)subtraction.Left;
			var rightSubNumber = (NumberNode)subtraction.Right;
			leftSubNumber.Value.Should().Be(1);
			rightSubNumber.Value.Should().Be(3);

			addition.Right.Should().BeOfType<NumberNode>();
			var rightNumber = (NumberNode)addition.Right;
			rightNumber.Value.Should().Be(2);
		}

		[Fact]
		public void ParseParenthesizedOrderTest()
		{
			var target = new ExpressionParser();
			string input = "1-(3+2)";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<SubtractNode>();
			var subtraction = (SubtractNode)result;

			subtraction.Left.Should().BeOfType<NumberNode>();
			var leftNumber = (NumberNode)subtraction.Left;
			leftNumber.Value.Should().Be(1);

			subtraction.Right.Should().BeOfType<AddNode>();
			var addition = (AddNode)subtraction.Right;

			addition.Left.Should().BeOfType<NumberNode>();
			addition.Right.Should().BeOfType<NumberNode>();
			var leftAddNumber = (NumberNode)addition.Left;
			var rightAddNumber = (NumberNode)addition.Right;
			leftAddNumber.Value.Should().Be(3);
			rightAddNumber.Value.Should().Be(2);
		}

		[Fact]
		public void ParsePrecedenceTest()
		{
			var target = new ExpressionParser();
			string input = "1-3/2";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<SubtractNode>();
			var subtraction = (SubtractNode)result;

			subtraction.Left.Should().BeOfType<NumberNode>();
			var leftNumber = (NumberNode)subtraction.Left;
			leftNumber.Value.Should().Be(1);

			subtraction.Right.Should().BeOfType<DivideNode>();
			var division = (DivideNode)subtraction.Right;

			division.Left.Should().BeOfType<NumberNode>();
			division.Right.Should().BeOfType<NumberNode>();
			var leftAddNumber = (NumberNode)division.Left;
			var rightAddNumber = (NumberNode)division.Right;
			leftAddNumber.Value.Should().Be(3);
			rightAddNumber.Value.Should().Be(2);
		}

		[Fact]
		public void ParsePrecedenceParenthesisTest()
		{
			var target = new ExpressionParser();
			string input = "(1-3)/2";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<DivideNode>();
			var division = (DivideNode)result;

			division.Left.Should().BeOfType<SubtractNode>();
			var subtraction = (SubtractNode)division.Left;

			subtraction.Left.Should().BeOfType<NumberNode>();
			subtraction.Right.Should().BeOfType<NumberNode>();
			var leftSubNumber = (NumberNode)subtraction.Left;
			var rightSubNumber = (NumberNode)subtraction.Right;
			leftSubNumber.Value.Should().Be(1);
			rightSubNumber.Value.Should().Be(3);

			division.Right.Should().BeOfType<NumberNode>();
			var rightNumber = (NumberNode)division.Right;
			rightNumber.Value.Should().Be(2);
		}
	}
}
