namespace AtelierTomato.Calculator
{
	public class DoubleLinkedListItem<T>
	{
		public T Value { get; init; }

		public DoubleLinkedListItem(T value) => Value = value;

		public DoubleLinkedListItem<T> First
		{
			get
			{
				var item = this;
				while (item.Previous != null) item = item.Previous;
				return item;
			}
		}
		public DoubleLinkedListItem<T> Last
		{
			get
			{
				var item = this;
				while (item.Next != null) item = item.Next;
				return item;
			}
		}

		public DoubleLinkedListItem<T>? Next { get; private set; }
		public DoubleLinkedListItem<T>? Previous { get; private set; }

		public DoubleLinkedListItem<T> InsertAfterThis(T value)
		{
			var newItem = new DoubleLinkedListItem<T>(value);
			newItem.Next = this.Next;
			newItem.Previous = this;
			if (this.Next != null)
			{
				this.Next.Previous = newItem;
			}
			this.Next = newItem;
			return newItem;
		}

		public DoubleLinkedListItem<T> InsertBeforeThis(T value)
		{
			var newItem = new DoubleLinkedListItem<T>(value);
			newItem.Previous = this.Previous;
			newItem.Next = this;
			if (this.Previous != null)
			{
				this.Previous.Next = newItem;
			}
			this.Previous = newItem;
			return newItem;
		}

		/// <summary>
		/// Unlinks this item from the previous item.
		/// </summary>
		public void UnlinkPrevious()
		{
			if (this.Previous is not null) this.Previous.Next = null;
			this.Previous = null;
		}

		/// <summary>
		/// Unlinks this item from the next item.
		/// </summary>
		public void UnlinkNext()
		{
			if (this.Next is not null) this.Next.Previous = null;
			this.Next = null;
		}

		/// <summary>
		/// Removes this item and returns the next.
		/// </summary>
		/// <returns>The next item.</returns>
		public DoubleLinkedListItem<T>? RemoveThis()
		{
			var previous = this.Previous;
			var next = this.Next;

			if (this.Previous is not null)
			{
				this.Previous.Next = next;
				this.Previous = null;
			}

			if (this.Next is not null)
			{
				this.Next.Previous = previous;
				this.Next = null;
			}

			return next;
		}

		/// <summary>
		/// Replaces this item with a new one.
		/// </summary>
		/// <returns>The new item</returns>
		public DoubleLinkedListItem<T> ReplaceThis(T value)
		{
			var newItem = new DoubleLinkedListItem<T>(value);
			newItem.Next = this.Next;
			if (this.Next != null)
			{
				this.Next.Previous = newItem;
			}
			newItem.Previous = this.Previous;
			if (this.Previous != null)
			{
				this.Previous.Next = newItem;
			}
			return newItem;
		}

		/// <summary>
		/// Searches the first item in the list starting with this item that has a value that matches the given <paramref name="predicate"/>.
		/// </summary>
		/// <returns>null, if no result was found.</returns>
		public DoubleLinkedListItem<T>? FindValueForward(Predicate<T> predicate)
		{
			for (var item = this; item != null; item = item.Next)
			{
				if (predicate(item.Value)) return item;
			}
			return null;
		}

		/// <summary>
		/// Searches the first item in the list starting with this item that matches the given <paramref name="predicate"/>.
		/// </summary>
		/// <returns>null, if no result was found.</returns>
		public DoubleLinkedListItem<T>? FindItemForward(Predicate<DoubleLinkedListItem<T>> predicate)
		{
			for (var item = this; item != null; item = item.Next)
			{
				if (predicate(item)) return item;
			}
			return null;
		}

		/// <summary>
		/// Searches the first item in the list ending with this item that has a value that matches the given <paramref name="predicate"/>.
		/// </summary>
		/// <returns>null, if no result was found.</returns>
		public DoubleLinkedListItem<T>? FindValueBackward(Predicate<T> predicate)
		{
			for (var item = this; item != null; item = item.Previous)
			{
				if (predicate(item.Value)) return item;
			}
			return null;
		}

		/// <summary>
		/// Searches the first item in the list ending with this item that matches the given <paramref name="predicate"/>.
		/// </summary>
		/// <returns>null, if no result was found.</returns>
		public DoubleLinkedListItem<T>? FindItemBackward(Predicate<DoubleLinkedListItem<T>> predicate)
		{
			for (var item = this; item != null; item = item.Previous)
			{
				if (predicate(item)) return item;
			}
			return null;
		}

		/// <summary>
		/// Searches the first item in the next-chain of this item that has a value that matches the given <paramref name="predicate"/>.
		/// </summary>
		/// <returns>null, if no result was found.</returns>
		public DoubleLinkedListItem<T>? FindNextValue(Predicate<T> predicate)
		{
			for (var item = this.Next; item != null; item = item.Next)
			{
				if (predicate(item.Value)) return item;
			}
			return null;
		}

		/// <summary>
		/// Searches the first item in the next-chain of this item that matches the given <paramref name="predicate"/>.
		/// </summary>
		/// <returns>null, if no result was found.</returns>
		public DoubleLinkedListItem<T>? FindNextItem(Predicate<DoubleLinkedListItem<T>> predicate)
		{
			for (var item = this.Next; item != null; item = item.Next)
			{
				if (predicate(item)) return item;
			}
			return null;
		}

		/// <summary>
		/// Searches the first item in the previous-chain of this item that has a value that matches the given <paramref name="predicate"/>.
		/// </summary>
		/// <returns>null, if no result was found.</returns>
		public DoubleLinkedListItem<T>? FindPreviousValue(Predicate<T> predicate)
		{
			for (var item = this.Previous; item != null; item = item.Previous)
			{
				if (predicate(item.Value)) return item;
			}
			return null;
		}

		/// <summary>
		/// Searches the first item in the previous-chain of this item that matches the given <paramref name="predicate"/>.
		/// </summary>
		/// <returns>null, if no result was found.</returns>
		public DoubleLinkedListItem<T>? FindPreviousItem(Predicate<DoubleLinkedListItem<T>> predicate)
		{
			for (var item = this.Previous; item != null; item = item.Previous)
			{
				if (predicate(item)) return item;
			}
			return null;
		}

		/// <summary>
		/// Links the two given items together, unlinking any existing Next/Previous relations.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public static void Link(DoubleLinkedListItem<T> left, DoubleLinkedListItem<T> right)
		{
			if (left.FindPreviousItem(p => p == right) is not null) throw new ArgumentException("Right may not be before left!");
			if (right.FindNextItem(p => p == left) is not null) throw new ArgumentException("Left may not be after right!");

			if (left.Next is not null)
				left.Next.Previous = null;
			if (right.Previous is not null)
				right.Previous.Next = null;

			left.Next = right;
			right.Previous = left;
		}

		public static DoubleLinkedListItem<T> AppendToOrCreate(DoubleLinkedListItem<T>? last, T value)
		{
			if (last is null) return new DoubleLinkedListItem<T>(value);
			if (last.Next is not null) throw new ArgumentException("Can only append to ends of lists.", nameof(last));
			return last.InsertAfterThis(value);
		}

		public static List<T> ConvertToList(DoubleLinkedListItem<T> item)
		{
			var result = new List<T>();
			for (var current = item.First; current is not null; current = current.Next)
				result.Add(current.Value);
			return result;
		}
	}
}
