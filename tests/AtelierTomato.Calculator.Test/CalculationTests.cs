using FluentAssertions;

namespace AtelierTomato.Calculator.Test
{
	public class CalculationTests
	{
		[Theory]
		[InlineData("1+2", 3)]
		[InlineData("1+2+3+4", 10)]
		[InlineData("1+2+3+4+5", 15)]
		[InlineData("3-1", 2)]
		[InlineData("3-1+6", 8)]
		[InlineData("3+1-6", -2)]
		[InlineData("1000+2", 1002)]
		public void AdditionAndSubtractionTests(string input, double output)
		{
			var mathsOnlyExpressionExecutor = new ExpressionExecutor();
			var mathsOnlyExpressionParser = new ExpressionParser();
			var result = mathsOnlyExpressionExecutor.Calculate(mathsOnlyExpressionParser.Parse(input));
			result.Should().Be(output);
		}

		[Theory]
		[InlineData("2+(2+10)+4", 18)]
		[InlineData("2+((3-1)+10)+4", 18)]
		[InlineData("2(3+4)", 14)]

		public void ParenthesesTests(string input, double output)
		{
			var mathsOnlyExpressionExecutor = new ExpressionExecutor();
			var mathsOnlyExpressionParser = new ExpressionParser();
			var result = mathsOnlyExpressionExecutor.Calculate(mathsOnlyExpressionParser.Parse(input));
			result.Should().Be(output);
		}

		[Theory]
		[InlineData("3*2", 6)]
		[InlineData("4×8", 32)]
		[InlineData("2.5∙10", 25)]
		[InlineData("70/5", 14)]
		[InlineData("8÷4", 2)]
		[InlineData("8*(2+10)+4", 100)]
		[InlineData("2+(2*10)", 22)]
		[InlineData("100*4", 400)]
		public void MultiplicationAndDivisionTests(string input, double output)
		{
			var mathsOnlyExpressionExecutor = new ExpressionExecutor();
			var mathsOnlyExpressionParser = new ExpressionParser();
			var result = mathsOnlyExpressionExecutor.Calculate(mathsOnlyExpressionParser.Parse(input));
			result.Should().Be(output);
		}

		[Theory]
		[InlineData("3^2", 9)]
		[InlineData("5^3", 125)]
		[InlineData("-5^2", -25)]
		[InlineData("2^-1", 0.5)]
		[InlineData("(1/2)^-2^2", 16)]
		[InlineData("16^(1/2)", 4)]
		[InlineData("2*12^2+8", 296)]
		[InlineData("4^2^3", 65536)]
		public void PowTests(string input, double output)
		{
			var mathsOnlyExpressionExecutor = new ExpressionExecutor();
			var mathsOnlyExpressionParser = new ExpressionParser();
			var result = mathsOnlyExpressionExecutor.Calculate(mathsOnlyExpressionParser.Parse(input));
			result.Should().Be(output);
		}

		[InlineData("3+(1-6)", -2)]
		[Theory]
		[InlineData("(1+(4-3))+(1-6)", -3)]
		[InlineData("-1+2", 1)]
		[InlineData("3*-1", -3)]
		[InlineData("3--1", 4)]
		[InlineData("3+-1", 2)]
		[InlineData("3/-1.5", -2)]
		[InlineData("3×-3", -9)]
		[InlineData("3∙-2", -6)]
		[InlineData("10÷-2", -5)]
		[InlineData("10^-2", 0.01)]
		public void NegativeNumberTests(string input, double output)
		{
			var mathsOnlyExpressionExecutor = new ExpressionExecutor();
			var mathsOnlyExpressionParser = new ExpressionParser();
			var result = mathsOnlyExpressionExecutor.Calculate(mathsOnlyExpressionParser.Parse(input));
			result.Should().Be(output);
		}

		[Theory]
		[InlineData("-1^2+-1^2", -2)]
		public void ExponentNegationLengthBehaviorTest(string input, double output)
		{
			var mathsOnlyExpressionExecutor = new ExpressionExecutor();
			var mathsOnlyExpressionParser = new ExpressionParser();
			var result = mathsOnlyExpressionExecutor.Calculate(mathsOnlyExpressionParser.Parse(input));
			result.Should().Be(output);
		}
	}
}
