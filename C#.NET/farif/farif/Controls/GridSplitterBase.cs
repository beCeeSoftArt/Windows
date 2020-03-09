/* Copyright André Spitzner 1977 - 2020 */
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace farif.Controls
{
    /// <summary>
    /// Class VerticalGridSplitter.
    /// Implements the <see cref="System.Windows.Controls.GridSplitter" /></summary>
    /// <seealso cref="System.Windows.Controls.GridSplitter" />
    public class GridSplitterBase : System.Windows.Controls.GridSplitter
    {
        /// <summary>
        /// The keyboard move splitter
        /// </summary>
        private readonly MethodInfo _KeyboardMoveSplitterMethodInfo = typeof(GridSplitterBase).GetMethod(
            "KeyboardMoveSplitter",
            BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Initializes a new instance of the <see cref="GridSplitterBase"/> class.
        /// </summary>
        public GridSplitterBase()
        {
            MouseEnter += OnGridSplitterMouseEnter;
            MouseLeave += OnGridSplitterMouseLeave;
            DragDelta += OnGridSplitterDragDelta;
        }

        /// <summary>
        /// Handles the <see cref="E:GridSplitterMouseEnter" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void OnGridSplitterMouseEnter(object sender, MouseEventArgs e)
        {
            Debug.WriteLine($"{nameof(GridSplitterBase)}::{nameof(OnGridSplitterMouseEnter)}");
            if (Cursor != Cursors.Wait)
                Mouse.OverrideCursor = HorizontalAlignment == HorizontalAlignment.Center ? Cursors.SizeWE : Cursors.SizeNS;
        }

        /// <summary>
        /// Handles the <see cref="E:GridSplitterMouseLeave" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void OnGridSplitterMouseLeave(object sender, MouseEventArgs e)
        {
            Debug.WriteLine($"{nameof(GridSplitterBase)}::{nameof(OnGridSplitterMouseLeave)}");
            if (Cursor != Cursors.Wait)
                Mouse.OverrideCursor = Cursors.Arrow;
        }

        /// <summary>
        /// Handles the <see cref="E:GridSplitterDragDelta" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs" /> instance containing the event data.</param>
        private void OnGridSplitterDragDelta(object sender, DragDeltaEventArgs e)
        {
            Debug.WriteLine($"{nameof(GridSplitterBase)}::{nameof(OnGridSplitterDragDelta)}: {nameof(DragDeltaEventArgs.HorizontalChange)}: {e.HorizontalChange}, {nameof(DragDeltaEventArgs.VerticalChange)}: {e.VerticalChange}");
            if (Math.Abs(e.VerticalChange) > 10
             || Math.Abs(e.HorizontalChange) > 10)
            {
                // Splitter has stopped resizing grid rows:
                // BugFix: use keyboard for continuing resizing until user releases mouse
                _KeyboardMoveSplitterMethodInfo.Invoke(sender, new object[] { e.HorizontalChange, e.VerticalChange });
            }
        }
    }
}
