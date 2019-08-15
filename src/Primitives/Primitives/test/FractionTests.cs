namespace ClickView.Extensions.Primitives.Tests
{
    using System;
    using Xunit;

    public class FractionTests
    {
        [Fact]
        public void Can_Parse_With_Denominator()
        {
            const string str = "100/11";

            Assert.True(Fraction.TryParse(str, out var fraction));

            Assert.Equal(100, fraction.Numerator);
            Assert.Equal(11, fraction.Denominator);
        }

        [Fact]
        public void Can_Parse_Without_Denominator()
        {
            const string str = "100";

            Assert.True(Fraction.TryParse(str, out var fraction));

            Assert.Equal(100, fraction.Numerator);
            Assert.Equal(1, fraction.Denominator);
        }

        [Fact]
        public void Can_Simplify()
        {
            var fraction = new Fraction(2, 4);
            Assert.Equal(1, fraction.Numerator);
            Assert.Equal(2, fraction.Denominator);
        }


        [Fact]
        public void Are_Equal()
        {
            Assert.Equal(new Fraction(123, 123), new Fraction(123, 123));
            Assert.Equal(new Fraction(2, 4), new Fraction(4, 8));
        }

        [Fact]
        public void Can_Parse_WithSemicolon()
        {
            const string str = "100:11";

            Assert.True(Fraction.TryParse(str, out var fraction));

            Assert.Equal((double)100 / 11, fraction);
            Assert.Equal(100, fraction.Numerator);
            Assert.Equal(11, fraction.Denominator);
        }

        [Fact]
        public void ToString_With_Denominator()
        {
            var fraction = new Fraction(100, 99);

            Assert.Equal("100/99", fraction.ToString());
        }

        [Fact]
        public void ToString_Without_Denominator()
        {
            var fraction = new Fraction(100);

            Assert.Equal("100", fraction.ToString());
        }

        [Fact]
        public void Implicit_Operator_Double()
        {
            var fraction = new Fraction(100, 11);

            Assert.Equal((double)100 / 11, fraction);
        }

        [Fact]
        public void Constructor_ZeroDenominator_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Fraction(100, 0));
        }
    }
}
