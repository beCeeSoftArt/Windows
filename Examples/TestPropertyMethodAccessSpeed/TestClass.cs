
namespace TestPropertyMethodAccessSpeed
{
	/// <summary>
	/// Test class
	/// </summary>
	public class TestClass
	{
		/// <summary>
		/// Gets or sets the property string.
		/// </summary>
		/// <value>
		/// The property string.
		/// </value>
		public string PropertyString { get; set; }

		/// <summary>
		/// The property string backing store
		/// </summary>
		private string _PropertyStringBackingStore;

		/// <summary>
		/// Gets or sets the property string backing store.
		/// </summary>
		/// <value>
		/// The property string backing store.
		/// </value>
		public string PropertyStringBackingStore
		{
			get { return _PropertyStringBackingStore; }
			set
			{
				if (_PropertyStringBackingStore != null
					&& value != null
					&& _PropertyStringBackingStore.Equals(value))
					return;

				_PropertyStringBackingStore = value;
			}
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <returns></returns>
		public string GetProperty()
		{
			return _PropertyStringBackingStore;
		}

		/// <summary>
		/// Sets the property.
		/// </summary>
		/// <param name="newString">The new string.</param>
		public void SetProperty(string newString)
		{
			if (_PropertyStringBackingStore.Equals(newString))
				return;

			_PropertyStringBackingStore = newString;
		}
	}
}
