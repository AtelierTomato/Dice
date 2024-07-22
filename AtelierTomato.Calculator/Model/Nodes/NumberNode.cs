namespace AtelierTomato.Calculator.Model.Nodes
{
	public class NumberNode : IExpressionNode
	{
		public override string ToString()
		{
			return $@"{Value:0.########}";
		}

		public double Value { get; init; }

		public NumberNode(double value)
		{
			Value = value;
		}
	}
}
