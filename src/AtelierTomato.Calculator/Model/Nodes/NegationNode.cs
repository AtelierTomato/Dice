namespace AtelierTomato.Calculator.Model.Nodes
{
	public class NegationNode : IExpressionNode
	{
		public override string ToString()
		{
			return $@"-{Right}";
		}

		public IExpressionNode Right { get; init; }

		public NegationNode(IExpressionNode right)
		{
			this.Right = right;
		}
	}
}
