/* Copyright André Spitzner 1977 - 2020 */
using System.Collections.Generic;

namespace farif.Base
{
    /// <summary>
    /// Class LineEndingConstants.
    /// </summary>
    public class LineEndingConstants
    {
        /// <summary>
        /// The line endings
        /// </summary>
        public static readonly List<LineEnding> LineEndings = new List<LineEnding>
        {
            new LineEnding("\\r", "\r"),
            new LineEnding("\\n", "\n"),
            new LineEnding("\\r\\n", "\r\n"),
        };

    }
}
