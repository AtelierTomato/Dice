using AtelierTomato.Calculator.Model.Nodes;

namespace AtelierTomato.Calculator
{
	public class ExpressionExecutor : IExpressionExecutor
	{
		/// <summary>
		/// Calculates the numeric result of the given expression tree without any additional detail.
		/// </summary>
		/// <param name="expressionTree">The root node of an expression tree.</param>
		/// <returns>The numeric result.</returns>
		public double Calculate(IExpressionNode expressionTree) => expressionTree switch
		{
			NumberNode numberNode => numberNode.Value,
			NegationNode negationNode => -Calculate(negationNode.Right),
			AddNode addNode => Calculate(addNode.Left) + Calculate(addNode.Right),
			SubtractNode subtractNode => Calculate(subtractNode.Left) - Calculate(subtractNode.Right),
			MultiplyNode multiplyNode => Calculate(multiplyNode.Left) * Calculate(multiplyNode.Right),
			DivideNode divideNode => Calculate(divideNode.Left) / Calculate(divideNode.Right),
			PowNode powNode => Math.Pow(Calculate(powNode.Left), Calculate(powNode.Right)),
			_ => throw new NotImplementedException(),
		};
	}
}
