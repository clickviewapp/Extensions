namespace ClickView.Extensions.Primitives
{
    using System;

    public struct Fraction
    {
        public long Numerator { get; }
        public long Denominator { get; }

        public static readonly Fraction Empty = new Fraction();

        public Fraction(long numerator)
        {
            //cant be simplified
            Numerator = numerator;
            Denominator = 1;
        }

        public Fraction(long numerator, long denominator)
        {
            if (denominator == 0)
                throw new ArgumentException("Invalid denominator value " + denominator, nameof(denominator));

#if NET
            (Numerator, Denominator) = Simplify(numerator, denominator);
#else
            var result = Simplify(numerator, denominator);
            Numerator = result.Item1;
            Denominator = result.Item2;
#endif
        }

        public static bool TryParse(string str, out Fraction fraction)
        {
            return TryParse(str, ['/', ':'], out fraction);
        }

        public static bool TryParse(string str, char[] separators, out Fraction fraction)
        {
            fraction = new Fraction();

            if (string.IsNullOrWhiteSpace(str))
                return false;

            var pieces = str.Split(separators);
            if (pieces.Length < 1 || pieces.Length > 2)
                return false;

            long numerator = 0;
            long denominator = 1;

            if (pieces.Length >= 1)
            {
                if (!long.TryParse(pieces[0], out numerator))
                    return false;
            }

            if (pieces.Length >= 2)
            {
                if (!long.TryParse(pieces[1], out denominator))
                    return false;
            }

            if (denominator == 0)
            {
                if (numerator == 0)
                {
                    fraction = Empty;
                    return true;
                }

                return false;
            }

            fraction = new Fraction(numerator, denominator);
            return true;
        }

        public override string ToString()
        {
            if (Denominator == 1)
                return Numerator.ToString();

            return Numerator + "/" + Denominator;
        }

        public static implicit operator double(Fraction fraction)
        {
            if (fraction == Empty)
                return 0;

            return (double) fraction.Numerator / fraction.Denominator;
        }

        public static bool operator ==(Fraction a, Fraction b)
        {
            return a.Numerator == b.Numerator && a.Denominator == b.Denominator;
        }

        public static bool operator !=(Fraction a, Fraction b)
        {
            return !(a == b);
        }

        public bool Equals(Fraction other)
        {
            return Numerator == other.Numerator && Denominator == other.Denominator;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            return obj is Fraction fraction && Equals(fraction);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Numerator.GetHashCode() * 397) ^ Denominator.GetHashCode();
            }
        }

        private static Tuple<long, long> Simplify(long numerator, long denominator)
        {
            if (denominator == 1)
                return new Tuple<long, long>(numerator, denominator);

            if (denominator < 0)
            {
                numerator = -numerator;
                denominator = -denominator;
            }

            var gcd = GreatestCommonDivisor(numerator, denominator);
            return new Tuple<long, long>(numerator / gcd, denominator / gcd);
        }

        public static long GreatestCommonDivisor(long p, long q)
        {
            while (true)
            {
                if (q == 0) return p;

                var r = p % q;

                p = q;
                q = r;
            }
        }
    }
}
