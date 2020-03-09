/* Copyright André Spitzner 1977 - 2020 */
namespace farif.Base
{
    /// <summary>
    /// Class LineEnding.
    /// </summary>
    public class LineEnding
    {
        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the name of the file usage.
        /// </summary>
        /// <value>The name of the file usage.</value>
        public string FileUsageName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineEnding"/> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="fileUsageName">Name of the file usage.</param>
        public LineEnding(string displayName, string fileUsageName)
        {
            DisplayName = displayName;
            FileUsageName = fileUsageName;
        }
    }
}
