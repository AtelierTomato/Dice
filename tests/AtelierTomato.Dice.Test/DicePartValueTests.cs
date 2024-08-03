using AtelierTomato.Dice.Model;
using FluentAssertions;

namespace AtelierTomato.Dice.Test
{
	public class DicePartValueTests
	{
		[Fact]
		public void ConstructIntTest()
		{
			var target1 = new DicePartValue(7);
			target1.IsInfinite.Should().BeFalse();
			target1.Value.Should().Be(7);
			target1.SourceValue.Should().Be(7);

			var target2 = new DicePartValue(0);
			target2.IsInfinite.Should().BeFalse();
			target2.Value.Should().Be(0);
			target2.SourceValue.Should().Be(0);

			var target3 = new DicePartValue(-7);
			target3.IsInfinite.Should().BeFalse();
			target3.Value.Should().Be(7);
			target3.SourceValue.Should().Be(-7);
		}

		[Fact]
		public void ConstructDoubleTest()
		{
			var target1 = new DicePartValue(7d);
			target1.IsInfinite.Should().BeFalse();
			target1.Value.Should().Be(7);
			target1.SourceValue.Should().Be(7d);

			var target2 = new DicePartValue(0d);
			target2.IsInfinite.Should().BeFalse();
			target2.Value.Should().Be(0);
			target2.SourceValue.Should().Be(0d);

			var target3 = new DicePartValue(-7d);
			target3.IsInfinite.Should().BeFalse();
			target3.Value.Should().Be(7);
			target3.SourceValue.Should().Be(-7d);

			var target4 = new DicePartValue(1.23d);
			target4.IsInfinite.Should().BeFalse();
			target4.Value.Should().Be(1);
			target4.SourceValue.Should().Be(1.23d);

			var target5 = new DicePartValue(Double.PositiveInfinity);
			target5.IsInfinite.Should().BeTrue();
			target5.Value.Should().BeNull();
			target5.SourceValue.Should().Be(Double.PositiveInfinity);

			var target6 = new DicePartValue(Double.NegativeInfinity);
			target6.IsInfinite.Should().BeTrue();
			target6.Value.Should().BeNull();
			target6.SourceValue.Should().Be(Double.NegativeInfinity);
		}

		[Fact]
		public void ConstructInvalidDoubleTest()
		{
			Func<DicePartValue> constructNaN = () => new DicePartValue(double.NaN);
			constructNaN.Should().Throw<ArgumentException>();

			Func<DicePartValue> constructHugePositive = () => new DicePartValue((double)int.MaxValue + 1);
			constructHugePositive.Should().Throw<ArgumentException>();

			Func<DicePartValue> constructHugeNegative = () => new DicePartValue((double)int.MinValue);
			constructHugeNegative.Should().Throw<ArgumentException>();
		}

		[Fact]
		public void IntConversionTest()
		{
			((DicePartValue)7).Should().Be(new DicePartValue(7));
			((DicePartValue)0).Should().Be(new DicePartValue(0));
			((DicePartValue)(-7)).Should().Be(new DicePartValue(-7));
		}

		[Fact]
		public void DoubleConversionTest()
		{
			((DicePartValue)7d).Should().Be(new DicePartValue(7d));
			((DicePartValue)0d).Should().Be(new DicePartValue(0d));
			((DicePartValue)(-7d)).Should().Be(new DicePartValue(-7d));
			((DicePartValue)1.23d).Should().Be(new DicePartValue(1.23d));
			((DicePartValue)(double.PositiveInfinity)).Should().Be(DicePartValue.Infinity);
		}
	}
}