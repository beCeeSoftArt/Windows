namespace ndow.Extensions
{
    public static class WebBrowserExtensions
    {
        /// <summary>
        /// Determines whether [is valid URI] [the specified URI].
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns><c>true</c> if [is valid URI] [the specified URI]; otherwise, <c>false</c>.</returns>
        public static bool IsValidUri(this string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                return false;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out var tmp))
                return false;
            return tmp.Scheme == Uri.UriSchemeHttp
                   || tmp.Scheme == Uri.UriSchemeHttps
                   || tmp.Scheme == Uri.UriSchemeFile;
        }

        /// <summary>
        /// To the valid URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>Uri.</returns>
        public static Uri? ToValidUri(this string uri)
        {
            if (!uri.IsValidUri())
                return null;

            // Try create an working uri
            return Uri.TryCreate(uri, UriKind.Absolute, out var tmp)
                ? tmp
                : null;
        }


        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public static string? GetFileName(this Uri uri)
        {
            if (!string.IsNullOrEmpty(uri.AbsolutePath) && uri.AbsolutePath.IndexOf("javascript", StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                var lastUrlPart = uri.AbsolutePath.Substring(uri.AbsolutePath.LastIndexOf("/", StringComparison.Ordinal));

                var resultName = lastUrlPart
                    .Replace("/", "")
                    .Replace("?", "")
                    .Replace("'", "")
                    .Replace(@"fs=opencloud", @"");

                //if (resultName.Contains("end="))
                //    resultName = resultName.Substring(0, resultName.IndexOf("end=", StringComparison.OrdinalIgnoreCase));

                return resultName;
            }

            return string.Empty;
        }

    }
}
