using AtelierTomato.Dice.Model;
using AtelierTomato.Dice.Settings;
using FluentAssertions;

namespace AtelierTomato.Dice.Test
{
	public class QueryExecutionLogBuilderTests
	{

		[Fact]
		public void AllAllNoDiceTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.AllRolls, default);

			var diceRequest = new DiceRequest(
				false,
				0,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(diceRequest, [], null, 0);

			var result = target.Build();

			result.Should().Be("`0d6`: 0, there's no dice!");
		}

		[Fact]
		public void AllAllOneDieTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.AllRolls, default);

			var diceRequest = new DiceRequest(
				false,
				1,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(diceRequest, [6], null, 6);

			var result = target.Build();

			result.Should().Be("`1d6`: 6");
		}

		[Fact]
		public void AllAllManyDiceTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.AllRolls, default);

			var diceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(diceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var result = target.Build();

			result.Should().Be("`20d6`: `[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2]` Sum: 70");
		}

		[Fact]
		public void AllAllVerboseManyDiceTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.AllRolls, default);

			var diceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				DiceVerbosity.Verbose);
			target.AddExpression(diceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var result = target.Build();

			result.Should().Be("`20d6v`: `[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2]` Sum: 70");
		}

		[Fact]
		public void AllAllQuietManyDiceTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.AllRolls, default);

			var diceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				DiceVerbosity.Quiet);
			target.AddExpression(diceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var result = target.Build();

			result.Should().Be("`20d6q`: 70");
		}

		[Fact]
		public void AllAllMultipleRollsTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.AllRolls, default);

			var multipleDiceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(multipleDiceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var singleDiceRequest = new DiceRequest(
				false,
				1,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(singleDiceRequest,
				[6,], null, 6);

			var result = target.Build();

			result.Should().Be(@"`20d6`: `[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2]` Sum: 70
`1d6`: 6");
		}

		[Fact]
		public void AllAllMultipleRollsDisplayBehaviorOverrideTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.AllRolls, default);

			var multipleDiceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(multipleDiceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var singleDiceRequest = new DiceRequest(
				false,
				1,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				DiceDisplayBehavior.Hide,
				default);
			target.AddExpression(singleDiceRequest,
				[6,], null, 6);

			var result = target.Build();

			result.Should().Be("`20d6`: `[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2]` Sum: 70");
		}

		[Fact]
		public void AllAllMultipleRollsTargetTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.AllRolls, default);

			var multipleDiceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				6,
				default,
				false,
				default,
				default);
			target.AddExpression(multipleDiceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], 2, 2);

			var singleDiceRequest = new DiceRequest(
				false,
				1,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				6,
				default,
				false,
				default,
				default);
			target.AddExpression(singleDiceRequest,
				[6,], 1, 1);

			var result = target.Build();

			result.Should().Be(@"`20d6t6`: `[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2]` Hit: 2
`1d6t6`: `[6]` Hit: 1");
		}

		[Fact]
		public void AllTotalOnlyNoDiceTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.TotalOnly, default);

			var diceRequest = new DiceRequest(
				false,
				0,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(diceRequest, [], null, 0);

			var result = target.Build();

			result.Should().Be("`0d6`: 0, there's no dice!");
		}

		[Fact]
		public void AllTotalOneDieTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.TotalOnly, default);

			var diceRequest = new DiceRequest(
				false,
				1,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(diceRequest, [6], null, 6);

			var result = target.Build();

			result.Should().Be("`1d6`: 6");
		}

		[Fact]
		public void AllTotalManyDiceTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.TotalOnly, default);

			var diceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(diceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var result = target.Build();

			result.Should().Be("`20d6`: 70");
		}

		[Fact]
		public void AllTotalVerboseManyDiceTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.TotalOnly, default);

			var diceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				DiceVerbosity.Verbose);
			target.AddExpression(diceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var result = target.Build();

			result.Should().Be("`20d6v`: `[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2]` Sum: 70");
		}

		[Fact]
		public void AllTotalQuietManyDiceTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.TotalOnly, default);

			var diceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				DiceVerbosity.Quiet);
			target.AddExpression(diceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var result = target.Build();

			result.Should().Be("`20d6q`: 70");
		}

		[Fact]
		public void AllTotalMultipleRollsTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.TotalOnly, default);

			var multipleDiceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(multipleDiceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var singleDiceRequest = new DiceRequest(
				false,
				1,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(singleDiceRequest,
				[6,], null, 6);

			var result = target.Build();

			result.Should().Be(@"`20d6`: 70
`1d6`: 6");
		}

		[Fact]
		public void AllTotalMultipleRollsTargetTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.TotalOnly, default);

			var multipleDiceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				6,
				default,
				false,
				default,
				default);
			target.AddExpression(multipleDiceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], 2, 2);

			var singleDiceRequest = new DiceRequest(
				false,
				1,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				6,
				default,
				false,
				default,
				default);
			target.AddExpression(singleDiceRequest,
				[6,], 1, 1);

			var result = target.Build();

			result.Should().Be(@"`20d6t6`: Hit: 2
`1d6t6`: Hit: 1");
		}

		[Fact]
		public void AllLimitedAboveManyDiceTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.LimitedRolls, 19);

			var diceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(diceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var result = target.Build();

			result.Should().Be("`20d6`: 70");
		}

		[Fact]
		public void AllLimitedBelowManyDiceTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.LimitedRolls, 21);

			var diceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(diceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var result = target.Build();

			result.Should().Be("`20d6`: `[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2]` Sum: 70");
		}

		[Fact]
		public void AllLimitedAboveVerboseManyDiceTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.LimitedRolls, 19);

			var diceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				DiceVerbosity.Verbose);
			target.AddExpression(diceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var result = target.Build();

			result.Should().Be("`20d6v`: `[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2]` Sum: 70");
		}

		[Fact]
		public void AllLimitedBelowQuietManyDiceTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.LimitedRolls, 21);

			var diceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				DiceVerbosity.Quiet);
			target.AddExpression(diceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var result = target.Build();

			result.Should().Be("`20d6q`: 70");
		}

		[Fact]
		public void ResultOnlyTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.ResultOnly, DiceExpressionFormat.AllRolls, default);

			var multipleDiceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(multipleDiceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var singleDiceRequest = new DiceRequest(
				false,
				1,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(singleDiceRequest,
				[6,], null, 6);

			var result = target.Build();

			result.Should().BeEmpty();
		}

		[Fact]
		public void ResultOnlyDisplayBehaviorOverrideTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.ResultOnly, DiceExpressionFormat.AllRolls, default);

			var multipleDiceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				DiceDisplayBehavior.Show,
				default);
			target.AddExpression(multipleDiceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var singleDiceRequest = new DiceRequest(
				false,
				1,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(singleDiceRequest,
				[6,], null, 6);

			var result = target.Build();

			result.Should().Be("`20d6s`: `[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2]` Sum: 70");
		}

		[Fact]
		public void LastDieAllMultipleRollsTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.LastDie, DiceExpressionFormat.AllRolls, default);

			var multipleDiceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(multipleDiceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var singleDiceRequest = new DiceRequest(
				false,
				1,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(singleDiceRequest,
				[6,], null, 6);

			var result = target.Build();

			result.Should().Be(@"`1d6`: 6");
		}

		[Fact]
		public void LastDieAllLastHiddenMultipleRollsTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.LastDie, DiceExpressionFormat.AllRolls, default);

			var multipleDiceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(multipleDiceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var singleDiceRequest = new DiceRequest(
				false,
				1,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				DiceDisplayBehavior.Hide,
				default);
			target.AddExpression(singleDiceRequest,
				[6,], null, 6);

			var result = target.Build();

			result.Should().BeEmpty();
		}

		[Fact]
		public void LastDieAllOtherShownMultipleRollsTest()
		{
			var target = new QueryExecutionLogBuilder(DiceQueryFormat.LastDie, DiceExpressionFormat.AllRolls, default);

			var multipleDiceRequest = new DiceRequest(
				false,
				20,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				DiceDisplayBehavior.Show,
				default);
			target.AddExpression(multipleDiceRequest,
				[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2], null, 70);

			var singleDiceRequest = new DiceRequest(
				false,
				1,
				6,
				default,
				default,
				false,
				default,
				default,
				false,
				default,
				default,
				default,
				default,
				false,
				default,
				default);
			target.AddExpression(singleDiceRequest,
				[6,], null, 6);

			var result = target.Build();

			result.Should().Be(@"`20d6s`: `[1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2]` Sum: 70
`1d6`: 6");
		}

		[Fact]
		public void QueryInheritIsInvalidTest()
		{
			var action = () => _ = new QueryExecutionLogBuilder(DiceQueryFormat.Inherit, DiceExpressionFormat.AllRolls, default);

			action.Should().Throw<ArgumentException>();
		}

		[Fact]
		public void ExpressionInheritIsInvalidTest()
		{
			var action = () => _ = new QueryExecutionLogBuilder(DiceQueryFormat.AllDice, DiceExpressionFormat.Inherit, default);

			action.Should().Throw<ArgumentException>();
		}
	}
}
