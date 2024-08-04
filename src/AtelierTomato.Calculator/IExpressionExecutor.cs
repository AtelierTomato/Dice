using AtelierTomato.Calculator.Model.Nodes;

namespace AtelierTomato.Calculator
{
	public interface IExpressionExecutor
	{
		double Calculate(IExpressionNode expressionTree);
	}
}