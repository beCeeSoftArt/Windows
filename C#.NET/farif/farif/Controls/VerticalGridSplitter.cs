/* Copyright André Spitzner 1977 - 2020 */
using System.Windows;

namespace farif.Controls
{
    /// <summary>
    /// Class VerticalGridSplitter.
    /// Implements the <see cref="GridSplitterBase" />
    /// </summary>
    /// <seealso cref="GridSplitterBase" />
    public class VerticalGridSplitter : GridSplitterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridSplitterBase"/> class.
        /// </summary>
        public VerticalGridSplitter()
        {
            Width = 2;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Stretch;
        }
    }
}
