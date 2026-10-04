namespace ndow.Extensions
{
    internal static class Units
    {
        /// <summary>
        /// Prefixes for numbers
        /// Since a long can only hold 9.2E18 (ie., exa territory) we
        /// don't need zetta, yotta, xona, weka, vunda, uda, treda, sorta, 
        /// rinta, quexa, pepta, ocha, nena, minga, or luma
        /// Just so you know...
        /// </summary>
        private static readonly string[] SiUnitSizes =
        {
            " ",
            "Kilo",
            "Mega",
            "Giga",
            "Tera",
            "Peta",
            "Exa",
            "Zetta",
            "Yotta",
            "Xona",
            "Weka",
            "vunda",
            "Uda",
            "Treda",
            "Sorta",
            "Rinta",
            "Quexa",
            "Pepta",
            "Ocha",
            "Nena",
            "Minga",
            "Luma"
        };

        /// <summary>
        /// Format a set of bytes into a human readable format
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="unit">The unit.</param>
        /// <param name="shortNameUnit">if set to <c>true</c> [short name for unit].</param>
        /// <param name="shortName">if set to <c>true</c> [short name].</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ApplicationException">Given 64 bit number is bigger than they should be.</exception>
        /// <exception cref="System.ApplicationException">64 bit numbers are bigger than they should be...</exception>
        public static string ToReadableSiUnit(this long value, string unit = "", bool shortNameUnit = false, bool shortName = false, bool onlyValue = false)
        {
            if (value == 0)
                return $"{value} {(shortNameUnit ? new string(unit.ToCharArray(0, 1)) : unit)}";

            // Get the exponent
            // Since log n(x) = y means "Multiply 'n' by itself 'y' times to get 'x'",
            // the integer part of the log base 10 of any number is the exponent.
            // (This is called the "characteristic" in math parlance)
            // Since we are using longs which don't have a fractional part, this can't be negative.
            int exponent = (int)Math.Log10(value);
            int group = exponent / 3;
            if (group >= SiUnitSizes.Length)
            {
                throw new ApplicationException("Given 64 bit number is bigger than they should be.");
            }
            double divisor = Math.Pow(10, group * 3);

            var displayValue = value / divisor;

            if (onlyValue)
                return $"{displayValue:0.0}";

            return $"{value / divisor:0.0} {GetSiUnitSizesText(@group, shortName)}{(shortNameUnit ? new string(unit.ToCharArray(0, 1)) : unit)}";
        }

        /// <summary>
        /// Gets the si unit sizes text.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="shortName">if set to <c>true</c> [short name].</param>
        /// <returns>System.String.</returns>
        private static string GetSiUnitSizesText(int index, bool shortName = false)
        {
            return !shortName
                ? SiUnitSizes[index]
                : SiUnitSizes[index][0].ToString();
        }

        /// <summary>
        /// To the readable si unit.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="unit">The unit.</param>
        /// <param name="shortName">if set to <c>true</c> [short name].</param>
        /// <returns>System.String.</returns>
        public static string ToReadableSiUnit(this int value, string unit = "", bool shortName = false)
        {
            return ToReadableSiUnit((long)value, unit, shortName);
        }
    }
}
