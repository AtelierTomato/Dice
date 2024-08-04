namespace AtelierTomato.Calculator.Model.Nodes
{
	public class MultiplyNode : IExpressionNode
	{
		public override string ToString()
		{
			return $@"({Left} * {Right})";
		}

		public IExpressionNode Left { get; init; }
		public IExpressionNode Right { get; init; }

		public MultiplyNode(IExpressionNode left, IExpressionNode right)
		{
			this.Left = left;
			this.Right = right;
		}
	}
}
