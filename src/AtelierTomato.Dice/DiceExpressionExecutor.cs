using AtelierTomato.Calculator;
using AtelierTomato.Calculator.Model.Nodes;
using AtelierTomato.Dice.Model;
using AtelierTomato.Dice.Model.Nodes;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Dice
{
	public class DiceExpressionExecutor
	{
		private readonly DiceRequestExecutor diceRequestExecutor;
		private readonly DiceOptions diceOptions;

		public DiceExpressionExecutor(DiceRequestExecutor diceRequestExecutor, IOptions<DiceOptions> diceOptions)
		{
			this.diceRequestExecutor = diceRequestExecutor;
			this.diceOptions = diceOptions.Value;
		}

		/// <summary>
		/// Calculates the numeric result of the given expression tree without any additional detail.
		/// </summary>
		/// <param name="expressionTree">The root node of an expression tree.</param>
		/// <returns>The numeric result.</returns>
		public (double NumericResult, string ExecutionLog) Execute(IExpressionNode expressionTree)
		{
			// todo do not access dice options here - this is for a higher layer to handle (especially since we do not have database access here)
			var queryExecutionLogBuilder = new QueryExecutionLogBuilder(diceOptions.DefaultDiceQueryFormat, diceOptions.DefaultDiceExpressionFormat, diceOptions.DefaultDiceOutputCutoff);
			var numericResult = ExecuteCore(expressionTree, queryExecutionLogBuilder);
			return (numericResult, queryExecutionLogBuilder.Build());
		}

		public double ExecuteCore(IExpressionNode expressionTree, QueryExecutionLogBuilder queryExecutionLogBuilder) => expressionTree switch
		{
			NumberNode numberNode => numberNode.Value,
			NegationNode negationNode => -ExecuteCore(negationNode.Right, queryExecutionLogBuilder),
			AddNode addNode => ExecuteCore(addNode.Left, queryExecutionLogBuilder) + ExecuteCore(addNode.Right, queryExecutionLogBuilder),
			SubtractNode subtractNode => ExecuteCore(subtractNode.Left, queryExecutionLogBuilder) - ExecuteCore(subtractNode.Right, queryExecutionLogBuilder),
			MultiplyNode multiplyNode => ExecuteCore(multiplyNode.Left, queryExecutionLogBuilder) * ExecuteCore(multiplyNode.Right, queryExecutionLogBuilder),
			DivideNode divideNode => ExecuteCore(divideNode.Left, queryExecutionLogBuilder) / ExecuteCore(divideNode.Right, queryExecutionLogBuilder),
			PowNode powNode => Math.Pow(ExecuteCore(powNode.Left, queryExecutionLogBuilder), ExecuteCore(powNode.Right, queryExecutionLogBuilder)),
			DiceNode diceNode => this.ExecuteDiceNode(diceNode, queryExecutionLogBuilder),
			_ => throw new NotImplementedException(),
		};

		private double ExecuteDiceNode(DiceNode diceNode, QueryExecutionLogBuilder queryExecutionLogBuilder)
		{
			var quantity = Numberize(diceNode.Quantity, queryExecutionLogBuilder);
			var sides = Numberize(diceNode.Sides, queryExecutionLogBuilder);

			if (quantity is null || sides is null) throw new ParseException("Quantity and Sides of a die should never be null!");

			var diceRequest = new DiceRequest(
				quantity.SourceValue < 0 != sides.SourceValue < 0,
				quantity,
				sides,
				Numberize(diceNode.ExplodeThreshold, queryExecutionLogBuilder),
				Numberize(diceNode.ExplodeIterations, queryExecutionLogBuilder),
				diceNode.ExplodeIsInfinite,
				Numberize(diceNode.RerollThreshold, queryExecutionLogBuilder),
				Numberize(diceNode.RerollIterations, queryExecutionLogBuilder),
				diceNode.RerollIsInfinite,
				Numberize(diceNode.DropLowestAmount, queryExecutionLogBuilder),
				Numberize(diceNode.KeepHighestAmount, queryExecutionLogBuilder),
				Numberize(diceNode.TargetThreshold, queryExecutionLogBuilder),
				Numberize(diceNode.FailureThreshold, queryExecutionLogBuilder),
				diceNode.SortDescending,
				diceNode.DiceDisplayBehavior,
				diceNode.DiceVerbosity);

			return this.diceRequestExecutor.Execute(diceRequest, queryExecutionLogBuilder);
		}

		private DicePartValue? Numberize(IExpressionNode? expressionNode, QueryExecutionLogBuilder queryExecutionLogBuilder)
		{
			if (expressionNode is null) return null;
			else return new DicePartValue(ExecuteCore(expressionNode, queryExecutionLogBuilder));
		}
	}
}
