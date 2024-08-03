using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace AtelierTomato.Dice.Test
{
	public class RollTests
	{
		[Fact]
		public void TenCoinFlipsTest()
		{
			const string input = "10d2";
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(1, 2))
				.Returns(1)
				.Returns(1)
				.Returns(1)
				.Returns(1)
				.Returns(1)
				.Returns(2)
				.Returns(2)
				.Returns(2)
				.Returns(2)
				.Returns(2)
				.Throws(() => new InvalidOperationException("All mocked dice from 1 to 2 were exhausted."));

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(15);
			result.ExecutionLog.TrimEnd().Should().Be("`10d2`: `[1, 1, 1, 1, 1, 2, 2, 2, 2, 2]` Sum: 15");
		}

		[Fact]
		public void InfinityDieTest()
		{
			const string input = "(1/0)d2";
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(double.PositiveInfinity);
			result.ExecutionLog.TrimEnd().Should().Be("`∞d2`: ∞");
		}

		[Fact]
		public void NegativesTest()
		{
			const string input = "-10d2";
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(1, 2))
				.Returns(1)
				.Returns(1)
				.Returns(1)
				.Returns(1)
				.Returns(1)
				.Returns(2)
				.Returns(2)
				.Returns(2)
				.Returns(2)
				.Returns(2)
				.Throws(() => new InvalidOperationException("All mocked dice from 1 to 2 were exhausted."));

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(-15);
			result.ExecutionLog.TrimEnd().Should().Be("`10d2`: `[1, 1, 1, 1, 1, 2, 2, 2, 2, 2]` Sum: 15");
		}

		[Fact]
		public void SimpleExplosionTest()
		{
			const string input = "1d20e20;10";
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(1, 20))
				.Returns(20)
				.Returns(20)
				.Returns(7)
				.Throws(() => new InvalidOperationException("All mocked dice from 1 to 20 were exhausted."));

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(47);
			result.ExecutionLog.TrimEnd().Should().Be("`1d20e20;10`: 47");
		}

		[Theory]
		[InlineData("1d1e1")]
		[InlineData("1d1e1;10")]
		public void DifferentExplosionInputMethodsTest(string input)
		{
			var diceRng = new DiceRng();
			var options = Options.Create(new DiceOptions { DefaultExplosionRecursions = 10, DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(11);
			result.ExecutionLog.TrimEnd().Should().Be($"`{input}`: 11");
		}

		[Fact]
		public void SimpleInfiniteExplosionTest()
		{
			const string input = "1d20ie20";
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(1, 20))
				.Returns(20)
				.Returns(20)
				.Returns(20)
				.Returns(20)
				.Returns(20)
				.Returns(20)
				.Returns(20)
				.Returns(20)
				.Returns(20)
				.Returns(20)
				.Returns(20)
				.Returns(20)
				.Returns(7)
				.Throws(() => new InvalidOperationException("All mocked dice from 1 to 20 were exhausted."));

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(247);
			result.ExecutionLog.TrimEnd().Should().Be($"`{input}`: 247");

		}

		[Fact]
		public void ExplosionResultsInInfinityTest()
		{
			const string input = "1d1ie1";
			var diceRng = new DiceRng();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.ExecutionLog.TrimEnd().Should().Be($"`{input}`: ∞");
		}

		[Theory]
		[InlineData("1d20r15")]
		[InlineData("1d20r15;10")]
		public void RerollTest(string input)
		{
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultRerolls = 10, DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(1, 20))
				.Returns(5)
				.Returns(8)
				.Returns(15)
				.Returns(16)
				.Throws(() => new InvalidOperationException("All mocked dice from 1 to 20 were exhausted."));

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(16);
			result.ExecutionLog.TrimEnd().Should().Be($"`{input}`: 16");
		}

		[Theory]
		[InlineData("1d6ir4", 4)]
		[InlineData("10d6ir4", 40)]
		[InlineData("5d20ir15", 75)]
		[InlineData("1d1000000ir999999", 999999)]
		public void InfiniteRerollTest(string input, int exclusiveMinimum)
		{
			var diceRng = new DiceRng();
			var options = Options.Create(new DiceOptions { DefaultExplosionRecursions = 10, DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().BeGreaterThan(exclusiveMinimum);
		}

		[Fact]
		public void OrderTest()
		{
			const string input = "10d20o";
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(1, 20))
				.Returns(10)
				.Returns(19)
				.Returns(12)
				.Returns(1)
				.Returns(18)
				.Returns(8)
				.Returns(8)
				.Returns(4)
				.Returns(18)
				.Returns(15)
				.Throws(() => new InvalidOperationException("All mocked dice from 1 to 20 were exhausted."));

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(113);
			result.ExecutionLog.TrimEnd().Should().Be("`10d20o`: `[19, 18, 18, 15, 12, 10, 8, 8, 4, 1]` Sum: 113");
		}

		[Fact]
		public void KeepHighestTest()
		{
			const string input = "10d20k6";
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(1, 20))
				.Returns(10)
				.Returns(19)
				.Returns(12)
				.Returns(1)
				.Returns(18)
				.Returns(8)
				.Returns(8)
				.Returns(4)
				.Returns(18)
				.Returns(15)
				.Throws(() => new InvalidOperationException("All mocked dice from 1 to 20 were exhausted."));

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(92);
			result.ExecutionLog.TrimEnd().Should().Be("`10d20k6`: `[10, 19, 12, 18, 18, 15]` Sum: 92");
		}

		[Fact]
		public void DropAndKeepActualNumbersTest()
		{
			const string input = "10d20p6k6";
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(1, 20))
				.Returns(10)
				.Returns(19)
				.Returns(12)
				.Returns(1)
				.Returns(18)
				.Returns(8)
				.Returns(8)
				.Returns(4)
				.Returns(18)
				.Returns(15)
				.Throws(() => new InvalidOperationException("All mocked dice from 1 to 20 were exhausted."));

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(70);
			result.ExecutionLog.TrimEnd().Should().Be("`10d20p6k6`: `[19, 18, 18, 15]` Sum: 70");
		}

		[Fact]
		public void KeepAndInfinitiesTest()
		{
			const string input = "(1/0)d20k6";
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(1, 20))
				.Returns(10)
				.Returns(19)
				.Returns(12)
				.Returns(1)
				.Returns(18)
				.Returns(8)
				.Throws(() => new InvalidOperationException("All mocked dice from 1 to 20 were exhausted."));

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(68);
			result.ExecutionLog.TrimEnd().Should().Be("`∞d20k6`: `[10, 19, 12, 1, 18, 8]` Sum: 68");
		}

		[Fact]
		public void TargetAndFailureTest()
		{
			const string input = "15d20t15f5";
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(1, 20))
				.Returns(10)
				.Returns(19)
				.Returns(12)
				.Returns(1)
				.Returns(18)
				.Returns(8)
				.Returns(8)
				.Returns(4)
				.Returns(18)
				.Returns(15)
				.Returns(20)
				.Returns(16)
				.Returns(18)
				.Returns(2)
				.Returns(10)
				.Throws(() => new InvalidOperationException("All mocked dice from 1 to 20 were exhausted."));

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(4);
			result.ExecutionLog.TrimEnd().Should().Be("`15d20t15f5`: `[10, 19, 12, 1, 18, 8, 8, 4, 18, 15, 20, 16, 18, 2, 10]` Hit: 4");
		}

		[Fact]
		public void TargetInfinityTest()
		{
			const string input = "(1/0)d20t20";
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(Double.PositiveInfinity);
			result.ExecutionLog.TrimEnd().Should().Be("`∞d20t20`: Hit: ∞");
		}

		[Theory]
		[InlineData("1d20+10", 20)]
		[InlineData("1d(10+10)", 10)]
		[InlineData("1d20*10", 100)]
		[InlineData("1d20-20", -10)]
		[InlineData("1d20^2", 100)]
		[InlineData("1d20/2", 5)]
		[InlineData("3+2d20", 23)]
		[InlineData("(3+2)d20", 50)]
		[InlineData("(10/5)d(5*4)", 20)]
		public void DiceWithMathsTest(string input, int output)
		{
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(1, 20))
				.Returns(10)
				.Returns(10)
				.Returns(10)
				.Returns(10)
				.Returns(10)
				.Throws(() => new InvalidOperationException("All mocked dice from 1 to 20 were exhausted."));

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(output);
		}

		[Theory]
		[InlineData("1d6d6", 10)]
		[InlineData("(1d6)d6", 10)]
		[InlineData("1d(2d6)", 6)]
		[InlineData("(2d6)d(1d6)", 21)]
		public void NestedDiceTest(string input, int output)
		{
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(1, 6))
				.Returns(2)
				.Returns(4)
				.Returns(6)
				.Returns(6)
				.Returns(5)
				.Returns(4)
				.Returns(3)
				.Returns(2)
				.Returns(1)
				.Throws(() => new InvalidOperationException("All mocked dice from 1 to 6 were exhausted."));

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(output);
		}

		[Fact]
		public void InfiniteExplosionAndRerollTest()
		{
			const string input = "1d20ie20ir100";
			var diceRng = Mock.Of<IDiceRng>();
			var options = Options.Create(new DiceOptions { DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(20, 20))
				.Returns(20)
				.Returns(20)
				.Returns(20)
				.Returns(20)
				.Returns(20)
				.Throws(() => new InvalidOperationException("All mocked dice from 20 to 20 were exhausted."));

			Mock.Get(diceRng)
				.SetupSequence(d => d.GenerateInteger(1, 20))
				.Returns(20)
				.Returns(14)
				.Returns(20)
				.Returns(14)
				.Throws(() => new InvalidOperationException("All mocked dice from 1 to 20 were exhausted."));

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(134);
		}

		[Fact]
		public void DropAndKeepOnesTest()
		{
			const string input = "30d1k20p15";
			var diceRng = new DiceRng();
			var options = Options.Create(new DiceOptions { DefaultExplosionRecursions = 10, DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(15);
		}

		[Fact]
		public void DropOnesTest()
		{
			const string input = "20d1p5";
			var diceRng = new DiceRng();
			var options = Options.Create(new DiceOptions { DefaultExplosionRecursions = 10, DefaultDiceExpressionFormat = Settings.DiceExpressionFormat.AllRolls, DefaultDiceQueryFormat = Settings.DiceQueryFormat.AllDice, });

			var diceRequestExecutor = new DiceRequestExecutor(diceRng, options);
			var expressionExecutor = new DiceExpressionExecutor(diceRequestExecutor, options);
			var expressionParser = new DiceExpressionParser();

			var result = expressionExecutor.Execute(expressionParser.Parse(input));

			result.NumericResult.Should().Be(15);
		}
	}
}
