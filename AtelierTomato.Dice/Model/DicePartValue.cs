namespace AtelierTomato.Dice.Model
{
	/// <summary>
	/// Represents a value used for representing numbers when dealing with dice.
	/// </summary>
	public class DicePartValue : IEquatable<DicePartValue>, IComparable<DicePartValue>
	{

		public DicePartValue(int value)
		{
			Value = Math.Abs(value);
			SourceValue = value;
		}

		public DicePartValue(double value)
		{
			this.SourceValue = value;
			if (double.IsNaN(value))
			{
				throw new ArgumentException("This dice part is not a number!", nameof(value));
			} else if (double.IsInfinity(value))
			{
				this.Value = null;
			} else if (Math.Abs(value) > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(value), "This dice part is too big but not infinite!");
			} else
			{
				this.Value = Math.Abs((int)Math.Round(value, 0));
			}
		}

		public static readonly DicePartValue Infinity = new DicePartValue(double.PositiveInfinity);

		public int? Value { get; init; }
		public double SourceValue { get; init; }
		public bool IsInfinite { get => Value is null; }

		public int CompareTo(DicePartValue? other)
		{
			if (other is null) return 1;
			if (this.IsInfinite && other.IsInfinite) return 0;
			if (this.IsInfinite) return 1;
			if (other.IsInfinite) return -1;
			return this.Value!.Value.CompareTo(other.Value!.Value);
		}

		public bool Equals(DicePartValue? other)
		{
			if (other is null) return false;
			if (this.IsInfinite || other.IsInfinite) return this.IsInfinite == other.IsInfinite;
			return this.Value.Equals(other.Value);
		}

		public static bool operator <(DicePartValue left, DicePartValue right)
		{
			return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
		}

		public static bool operator <=(DicePartValue left, DicePartValue right)
		{
			return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
		}

		public static bool operator >(DicePartValue left, DicePartValue right)
		{
			return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
		}

		public static bool operator >=(DicePartValue left, DicePartValue right)
		{
			return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
		}

		public static implicit operator DicePartValue(int value) => new DicePartValue(value);
		public static implicit operator DicePartValue(double value) => new DicePartValue(value);

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (ReferenceEquals(obj, null))
			{
				return false;
			}

			return Equals(obj as DicePartValue);
		}

		public override int GetHashCode()
		{
			return Value ?? -1;
		}

		public static bool operator ==(DicePartValue left, DicePartValue right)
		{
			if (ReferenceEquals(left, null))
			{
				return ReferenceEquals(right, null);
			}

			return left.Equals(right);
		}

		public static bool operator !=(DicePartValue left, DicePartValue right)
		{
			return !(left == right);
		}

		public override string ToString()
		{
			return this.IsInfinite ? "∞" : $@"{this.Value:0.########}";
		}
	}
}
