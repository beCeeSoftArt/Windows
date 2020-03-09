/* Copyright André Spitzner 1977 - 2020 */
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace farif.ViewModel
{
    /// <summary>
    /// Class CheckItem.
    /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class CheckItem : INotifyPropertyChanged
    {
        #region .Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckItem"/> class.
        /// </summary>
        public CheckItem()
        {
            IsChecked = false;
            IsSelected = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckItem" /> class.
        /// </summary>
        /// <param name="keyValue">The key value.</param>
        /// <param name="display">The display.</param>
        /// <param name="isChecked">if set to <c>true</c> [is checked].</param>
        /// <param name="isSelected">if set to <c>true</c> [is selected].</param>
        public CheckItem(object keyValue, string display, bool isChecked = false, bool isSelected = false)
        {
            IsChecked = isChecked;
            IsSelected = isSelected;
            KeyValue = keyValue;
            Display = display;
        }

        #endregion

        #region Property - KeyValue

        /// <summary>
        /// The key value
        /// </summary>
        private object keyValue;

        /// <summary>
        /// Gets or sets the key value.
        /// </summary>
        /// <value>The key value.</value>
        public object KeyValue
        {
            get => keyValue;
            set
            {
                if (Equals(keyValue, value))
                    return;

                keyValue = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region Property - Index

        /// <summary>
        /// The index
        /// </summary>
        private int index;

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get => index;
            set
            {
                if (index.Equals(value))
                    return;

                index = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region Property - IsChecked

        /// <summary>
        /// The is checked
        /// </summary>
        private bool isChecked;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked.
        /// </summary>
        /// <value><c>true</c> if this instance is checked; otherwise, <c>false</c>.</value>
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                if (isChecked.Equals(value))
                    return;

                isChecked = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region IsSelected

        /// <summary>
        /// The is selected
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected.Equals(value))
                    return;

                isSelected = value;

                OnPropertyChanged();
            }
        }


        #endregion

        #region Property - Display

        /// <summary>
        /// The display
        /// </summary>
        private string display;

        /// <summary>
        /// Gets or sets the display.
        /// </summary>
        /// <value>The display.</value>
        public string Display
        {
            get => display;
            set
            {
                if (string.Equals(display, value, StringComparison.Ordinal))
                    return;

                display = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Tritt ein, wenn sich ein Eigenschaftswert ändert.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

}
