using FluentAssertions;

namespace AtelierTomato.Calculator.Test
{
	public class DoubleLinkedListItemTests
	{
		[Fact]
		public void FirstAndLastTest()
		{
			var node = new DoubleLinkedListItem<int>(5);
			node.InsertBeforeThis(1);
			node.InsertBeforeThis(2);
			node.InsertBeforeThis(3);
			node.InsertBeforeThis(4);
			node.InsertAfterThis(9);
			node.InsertAfterThis(8);
			node.InsertAfterThis(7);
			node.InsertAfterThis(6);
			node.First.Value.Should().Be(1);
			node.Last.Value.Should().Be(9);
			node.First.Last.Should().Be(node.Last);
			node.Last.First.Should().Be(node.First);
		}

		[Fact]
		public void AppendToOrCreateOntoNullTest()
		{
			DoubleLinkedListItem<int>? target = null;
			var result = target.AppendToOrCreate(1);
			result.Should().NotBeNull().And.BeOfType<DoubleLinkedListItem<int>>();
			result.Value.Should().Be(1);
		}
		[Fact]
		public void AppendToOrCreateOntoListTest()
		{
			var givenList = new DoubleLinkedListItem<int>(0);
			var result = givenList.AppendToOrCreate(1);
			result.Should().NotBeNull();
			result.Value.Should().Be(1);
			result.Previous.Should().NotBeNull();
			result.Previous!.Value.Should().Be(0);
		}
	}

}
