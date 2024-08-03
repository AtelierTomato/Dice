namespace AtelierTomato.Calculator.Model.Nodes
{
	public class PowNode : IExpressionNode
	{
		public override string ToString()
		{
			return $@"({Left} ^ {Right})";
		}

		public IExpressionNode Left { get; init; }
		public IExpressionNode Right { get; init; }

		public PowNode(IExpressionNode left, IExpressionNode right)
		{
			this.Left = left;
			this.Right = right;
		}
	}
}
