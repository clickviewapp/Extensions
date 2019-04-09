namespace ClickView.Extensions.Primitives.Tests
{
    using Xunit;

    public class FractionTests
    {
        [Fact]
        public void Can_Parse_With_Denominator()
        {
            const string str = "100/11";

            Assert.True(Fraction.TryParse(str, out var fraction));

            Assert.Equal((double)100/11, fraction);
            Assert.Equal(str, fraction.ToString());
            Assert.Equal(100, fraction.Numerator);
            Assert.Equal(11, fraction.Denominator);
        }

        [Fact]
        public void Can_Parse_Without_Denominator()
        {
            const string str = "100";

            Assert.True(Fraction.TryParse(str, out var fraction));

            Assert.Equal((double)100, fraction);
            Assert.Equal("100", fraction.ToString());
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
    }
}
