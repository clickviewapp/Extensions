namespace ClickView.Extensions.Primitives.Extensions
{
    using System;

    public static class IntExtensions
    {
        /// <summary>
        /// Get the length of an Int value
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// http://www.java2s.com/Code/CSharp/Data-Types/Getthedigitlengthofanintvalue.htm
        public static int Length(this int i)
        {
            if (i < 0)
                throw new ArgumentOutOfRangeException();

            if (i == 0)
                return 1;

            return (int)Math.Floor(Math.Log10(i)) + 1;
        }
    }
}
