/* Copyright André Spitzner 1977 - 2020 */
using System.Windows;

namespace farif.Controls
{
    /// <summary>
    /// Class HorizontalGridSplitter.
    /// Implements the <see cref="GridSplitterBase" />
    /// </summary>
    /// <seealso cref="GridSplitterBase" />
    public class HorizontalGridSplitter : GridSplitterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridSplitterBase"/> class.
        /// </summary>
        public HorizontalGridSplitter()
        {
            Height = 2;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Center;
        }
    }
}
