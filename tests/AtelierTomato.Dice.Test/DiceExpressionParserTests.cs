using AtelierTomato.Calculator;
using AtelierTomato.Calculator.Model;
using AtelierTomato.Calculator.Model.Nodes;
using AtelierTomato.Dice.Model.Nodes;
using FluentAssertions;

namespace AtelierTomato.Dice.Test
{
	public class DiceExpressionParserTests
	{
		[Fact]
		public void TokenizeSimpleTest()
		{
			var target = new DiceExpressionParser();
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
			var target = new DiceExpressionParser();
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
		public void ParseSimpleDiceTest()
		{
			var target = new DiceExpressionParser();
			string input = "12d20";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<DiceNode>();
			var die = (DiceNode)result;

			die.Quantity.Should().BeOfType<NumberNode>();
			die.Sides.Should().BeOfType<NumberNode>();
			var quantity = (NumberNode)die.Quantity;
			var sides = (NumberNode)die.Sides;
			quantity.Value.Should().Be(12);
			sides.Value.Should().Be(20);
		}

		[Fact]
		public void ParseModifiedDiceTest()
		{
			var target = new DiceExpressionParser();
			string input = "12d20e19ir2t11o";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<DiceNode>();
			var die = (DiceNode)result;

			die.Quantity.Should().BeOfType<NumberNode>();
			die.Sides.Should().BeOfType<NumberNode>();
			die.ExplodeThreshold.Should().NotBeNull().And.BeOfType<NumberNode>();
			die.RerollThreshold.Should().NotBeNull().And.BeOfType<NumberNode>();
			die.TargetThreshold.Should().NotBeNull().And.BeOfType<NumberNode>();
			var quantity = (NumberNode)die.Quantity;
			var sides = (NumberNode)die.Sides;
			var explodeT = (NumberNode)die.ExplodeThreshold!;
			var rerollT = (NumberNode)die.RerollThreshold!;
			var targetT = (NumberNode)die.TargetThreshold!;
			quantity.Value.Should().Be(12);
			sides.Value.Should().Be(20);
			explodeT.Value.Should().Be(19);
			rerollT.Value.Should().Be(2);
			targetT.Value.Should().Be(11);
			die.ExplodeIsInfinite.Should().BeFalse();
			die.RerollIsInfinite.Should().BeTrue();
			die.SortDescending.Should().BeTrue();
		}

		[Fact]
		public void ParseModifiedWithSemicolonsDiceTest()
		{
			var target = new DiceExpressionParser();
			string input = "12d20e19;1r2;5t11";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<DiceNode>();
			var die = (DiceNode)result;

			die.Quantity.Should().BeOfType<NumberNode>();
			die.Sides.Should().BeOfType<NumberNode>();
			die.ExplodeThreshold.Should().NotBeNull().And.BeOfType<NumberNode>();
			die.RerollThreshold.Should().NotBeNull().And.BeOfType<NumberNode>();
			die.TargetThreshold.Should().NotBeNull().And.BeOfType<NumberNode>();
			var quantity = (NumberNode)die.Quantity;
			var sides = (NumberNode)die.Sides;
			var explodeT = (NumberNode)die.ExplodeThreshold!;
			var explodeI = (NumberNode)die.ExplodeIterations!;
			var rerollT = (NumberNode)die.RerollThreshold!;
			var rerollI = (NumberNode)die.RerollIterations!;
			var targetT = (NumberNode)die.TargetThreshold!;
			quantity.Value.Should().Be(12);
			sides.Value.Should().Be(20);
			explodeT.Value.Should().Be(19);
			explodeI.Value.Should().Be(1);
			rerollT.Value.Should().Be(2);
			rerollI.Value.Should().Be(5);
			targetT.Value.Should().Be(11);
			die.ExplodeIsInfinite.Should().BeFalse();
			die.RerollIsInfinite.Should().BeFalse();
		}

		[Fact]
		public void ParseNegationFirstTest()
		{
			var target = new DiceExpressionParser();
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
			var target = new DiceExpressionParser();
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
		public void ParseNegationDiceTest()
		{
			var target = new DiceExpressionParser();
			string input = "-3d-6";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<NegationNode>();
			var outerNegation = (NegationNode)result;
			outerNegation.Right.Should().NotBeNull().And.BeOfType<DiceNode>();
			var die = (DiceNode)outerNegation.Right;

			die.Quantity.Should().BeOfType<NumberNode>();
			die.Sides.Should().BeOfType<NegationNode>();

			var quantity = (NumberNode)die.Quantity;
			quantity.Value.Should().Be(3);

			var sidesNegation = (NegationNode)die.Sides;
			sidesNegation.Right.Should().NotBeNull().And.BeOfType<NumberNode>();
			var sidesNumber = (NumberNode)sidesNegation.Right;
			sidesNumber.Value.Should().Be(6);
		}

		[Fact]
		public void ParseOrderTest()
		{
			var target = new DiceExpressionParser();
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
		public void ParseDiceOrderTest()
		{
			var target = new DiceExpressionParser();
			string input = "1d2d3";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<DiceNode>();
			var outerDie = (DiceNode)result;

			outerDie.Quantity.Should().BeOfType<DiceNode>();
			var innerDie = (DiceNode)outerDie.Quantity;

			innerDie.Quantity.Should().BeOfType<NumberNode>();
			innerDie.Sides.Should().BeOfType<NumberNode>();
			var leftSubNumber = (NumberNode)innerDie.Quantity;
			var rightSubNumber = (NumberNode)innerDie.Sides;
			leftSubNumber.Value.Should().Be(1);
			rightSubNumber.Value.Should().Be(2);

			outerDie.Sides.Should().BeOfType<NumberNode>();
			var rightNumber = (NumberNode)outerDie.Sides;
			rightNumber.Value.Should().Be(3);
		}

		[Fact]
		public void ParseParenthesizedOrderTest()
		{
			var target = new DiceExpressionParser();
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
			var target = new DiceExpressionParser();
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
			var target = new DiceExpressionParser();
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

		[Fact]
		public void ParseDiceAddToResultTest()
		{
			var target = new DiceExpressionParser();
			string input = "1d6+2";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<AddNode>();
			var addition = (AddNode)result;
			addition.Right.Should().NotBeNull().And.BeOfType<NumberNode>();
			var rightNumber = (NumberNode)addition.Right;
			rightNumber.Value.Should().Be(2);
			addition.Left.Should().NotBeNull().And.BeOfType<DiceNode>();
			var die = (DiceNode)addition.Left;

			die.Quantity.Should().BeOfType<NumberNode>();
			die.Sides.Should().BeOfType<NumberNode>();

			var quantity = (NumberNode)die.Quantity;
			quantity.Value.Should().Be(1);

			var sidesNumber = (NumberNode)die.Sides;
			sidesNumber.Value.Should().Be(6);
		}

		[Fact]
		public void ParseDiceDoNotDestroyExponentsTest()
		{
			var target = new DiceExpressionParser();
			string input = "1d6+-1^2";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<AddNode>();
			var addition = (AddNode)result;
			addition.Right.Should().NotBeNull().And.BeOfType<NegationNode>();
			var negation = (NegationNode)addition.Right;
			negation.Right.Should().NotBeNull().And.BeOfType<PowNode>();
			var pow = (PowNode)negation.Right;
			pow.Left.Should().NotBeNull().And.BeOfType<NumberNode>();
			pow.Right.Should().NotBeNull().And.BeOfType<NumberNode>();
			var powLeft = (NumberNode)pow.Left;
			powLeft.Value.Should().Be(1);
			var powRight = (NumberNode)pow.Right;
			powRight.Value.Should().Be(2);
			addition.Left.Should().NotBeNull().And.BeOfType<DiceNode>();
			var die = (DiceNode)addition.Left;

			die.Quantity.Should().BeOfType<NumberNode>();
			die.Sides.Should().BeOfType<NumberNode>();

			var quantity = (NumberNode)die.Quantity;
			quantity.Value.Should().Be(1);

			var sidesNumber = (NumberNode)die.Sides;
			sidesNumber.Value.Should().Be(6);
		}

		[Theory]
		[InlineData("1d6")]
		[InlineData("1d6e5r2")]
		[InlineData("1d(1d6)")]
		[InlineData("(1d6)d6")]
		[InlineData("12d6e(6 - 1)t7")]
		public void ParseDiceKeepTermsIntactText(string input)
		{
			var target = new DiceExpressionParser();
			var result = target.Parse(input);
			result.ToString().Should().Be(input);
		}

		[Theory]
		[InlineData("1d(1d6h)o")]
		[InlineData("(1d20h)d(1d20h)")]
		[InlineData("(1d20)d(1d20)")]
		[InlineData("(1d20h)d(1d20)")]
		[InlineData("(1d20)d(1d20h)")]
		[InlineData("(1d20e19)d20")]
		public void ParseNestedDiceTest(string input)
		{
			var target = new DiceExpressionParser();
			var result = target.Parse(input);
			result.ToString().Should().Be(input);
		}

		[Theory]
		[InlineData("1d6", "1d6")]
		[InlineData("1d6h", "1d6h")]
		[InlineData("1d6ht1", "1d6t1h")]
		[InlineData("1d6hh", "1d6h")]
		[InlineData("1d6s", "1d6s")]
		[InlineData("1d6se6", "1d6e6s")]
		[InlineData("1d6ss", "1d6s")]
		[InlineData("1d6sh", "1d6")]
		[InlineData("1d6soh", "1d6o")]
		public void ParseDiceDisplayBehaviorTest(string input, string expected)
		{
			var target = new DiceExpressionParser();

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<DiceNode>();

			result.ToString().Should().Be(expected);
		}

		[Theory]
		[InlineData("1d6", "1d6")]
		[InlineData("1d6q", "1d6q")]
		[InlineData("1d6qt1", "1d6t1q")]
		[InlineData("1d6qq", "1d6q")]
		[InlineData("1d6v", "1d6v")]
		[InlineData("1d6ve6", "1d6e6v")]
		[InlineData("1d6vv", "1d6v")]
		[InlineData("1d6vq", "1d6")]
		[InlineData("1d6voq", "1d6o")]
		public void ParseDiceVerbosityTest(string input, string expected)
		{
			var target = new DiceExpressionParser();

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<DiceNode>();

			result.ToString().Should().Be(expected);
		}

		[Fact]
		public void ParseKeepDropDiceTest()
		{
			var target = new DiceExpressionParser();
			string input = "12d20k10p15";

			var result = target.Parse(input);

			result.Should().NotBeNull().And.BeAssignableTo<IExpressionNode>();
			result.Should().BeOfType<DiceNode>();
			var die = (DiceNode)result;

			die.Quantity.Should().BeOfType<NumberNode>();
			die.Sides.Should().BeOfType<NumberNode>();
			die.KeepHighestAmount.Should().NotBeNull().And.BeOfType<NumberNode>();
			die.DropLowestAmount.Should().NotBeNull().And.BeOfType<NumberNode>();
			var quantity = (NumberNode)die.Quantity;
			var sides = (NumberNode)die.Sides;
			var keepHighestAmount = (NumberNode)die.KeepHighestAmount!;
			var dropLowestAmount = (NumberNode)die.DropLowestAmount!;
			quantity.Value.Should().Be(12);
			sides.Value.Should().Be(20);
			keepHighestAmount.Value.Should().Be(10);
			dropLowestAmount.Value.Should().Be(15);
		}
	}
}
