/* Copyright André Spitzner 1977 - 2020 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace farif.Tools
{
    /// <summary>
    /// Class Utility.
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// The current user
        /// </summary>
        private static WindowsIdentity _CurrentUser;

        /// <summary>
        /// The current principal
        /// </summary>
        private static WindowsPrincipal _CurrentPrincipal;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private static void Initialize()
        {
            if (_CurrentUser != null
                || _CurrentPrincipal != null)
                return;

            _CurrentUser = WindowsIdentity.GetCurrent();
            _CurrentPrincipal = new WindowsPrincipal(_CurrentUser);
        }

        /// <summary>
        /// Determines whether [has file or directory access] [the specified right].
        /// </summary>
        /// <param name="right">The right.</param>
        /// <param name="acl">The acl.</param>
        /// <returns><c>true</c> if [has file or directory access] [the specified right]; otherwise, <c>false</c>.</returns>
        private static bool HasFileOrDirectoryAccess(
            FileSystemRights right,
            AuthorizationRuleCollection acl)
        {
            var allow = false;
            var inheritedAllow = false;
            var inheritedDeny = false;

            Initialize();

            for (var i = 0; i < acl.Count; i++)
            {
                var currentRule = (FileSystemAccessRule)acl[i];
                // If the current rule applies to the current user.
                if (currentRule == null
                    || _CurrentUser.User == null
                    || !_CurrentUser.User.Equals(currentRule.IdentityReference)
                    && !_CurrentPrincipal.IsInRole((SecurityIdentifier)currentRule.IdentityReference))
                    continue;

                if (currentRule.AccessControlType.Equals(AccessControlType.Deny))
                {
                    if ((currentRule.FileSystemRights & right) != right)
                        continue;

                    if (currentRule.IsInherited)
                        inheritedDeny = true;
                    else
                        // Non inherited "deny" takes overall precedence.
                        return false;
                }
                else if (currentRule.AccessControlType.Equals(AccessControlType.Allow))
                {
                    if ((currentRule.FileSystemRights & right) != right)
                        continue;

                    if (currentRule.IsInherited)
                        inheritedAllow = true;
                    else
                        allow = true;
                }
            }

            if (allow)
                // Non inherited "allow" takes precedence over inherited rules.
                return true;

            return inheritedAllow && !inheritedDeny;
        }

        /// <summary>
        /// Files the has permission.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="right">The right.</param>
        /// <returns><c>true</c> if successfully, <c>false</c> otherwise.</returns>
        public static bool FileHasPermission(
            string file,
            FileSystemRights right)
        {
            // Get the collection of authorization rules that apply to the file.
            var acl = new FileInfo(file).GetAccessControl()
                .GetAccessRules(true, true, typeof(SecurityIdentifier));
            return HasFileOrDirectoryAccess(right, acl);
        }

        /// <summary>
        /// Gets the file text lines.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static List<string> GetFileTextLines(string filePath, Encoding encoding = null, bool detectEncodingFromByteOrderMarks = false)
        {
            if (!FileHasPermission(filePath, FileSystemRights.Read))
                return null;

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var br = encoding == null ? new StreamReader(fs, detectEncodingFromByteOrderMarks) : new StreamReader(fs, encoding, detectEncodingFromByteOrderMarks);
            var lsFileLines = new List<string>();
            while (!br.EndOfStream)
                lsFileLines.Add(br.ReadLine());
            br.Close();
            return lsFileLines;
        }

        /// <summary>
        /// Gets the file text.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        /// <returns>System.String.</returns>
        public static string GetFileText(string filePath, Encoding encoding = null, bool detectEncodingFromByteOrderMarks = false)
        {
            if (!FileHasPermission(filePath, FileSystemRights.Read))
                return null;

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var br = encoding == null ? new StreamReader(fs, detectEncodingFromByteOrderMarks) : new StreamReader(fs, encoding, detectEncodingFromByteOrderMarks);
            var sFileText = br.ReadToEnd();
            br.Close();
            return sFileText;
        }

        /// <summary>
        /// Determines whether the specified text contains string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="compareOptions">The compare options.</param>
        /// <returns><c>true</c> if the specified text contains string; otherwise, <c>false</c>.</returns>
        public static bool ContainsString(string text, string searchText, CompareOptions compareOptions = CompareOptions.IgnoreCase)
        {
            if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(text, searchText, compareOptions) >= 0)
                return true;
            return false;
        }

        /// <summary>
        /// Determines whether [has line breaks] [the specified file path].
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns><c>true</c> if [has line breaks] [the specified file path]; otherwise, <c>false</c>.</returns>
        public static bool HasLineBreaks(string filePath, Encoding encoding = null)
        {
            var textToTest = GetFileText(filePath, encoding);
            return textToTest.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Length > 1;
        }

        /// <summary>
        /// Replaces a string in text string.
        /// Supports ordinal und ignore case
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="replaceText">The replace text.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns>System.String.</returns>
        public static string ReplaceString(
            string text,
            string searchText,
            string replaceText, StringComparison comparison)
        {
            var sb = new StringBuilder();

            var previousIndex = 0;
            var index = text.IndexOf(searchText, comparison);
            while (index != -1)
            {
                sb.Append(text.Substring(previousIndex, index - previousIndex));
                sb.Append(replaceText);
                index += searchText.Length;

                previousIndex = index;
                index = text.IndexOf(searchText, index, comparison);
            }
            sb.Append(text.Substring(previousIndex));

            return sb.ToString();
        }

        /// <summary>
        /// Gets the file data.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetFileData(string filePath)
        {
            if (!FileHasPermission(filePath, FileSystemRights.Read))
                return null;

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var br = new BinaryReader(fs);
            var bFileData = br.ReadBytes((int)fs.Length);
            br.Close();

            return bFileData;
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.Int32.</returns>
        public static int GetFileSize(string filePath)
        {
            var result = 0;
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            result = Convert.ToInt32(fs.Length);
            fs.Close();
            return result;
        }
        /// <summary>
        /// Sets the file data.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="data">The data.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>System.Boolean.</returns>
        public static bool SetFileData(string file, string data, Encoding encoding = null)
        {
            if (!FileHasPermission(file, FileSystemRights.Write))
            {
                return false;
            }

            var fileInfo = new FileInfo(file);

            if (encoding != null)
                using (var sw = new StreamWriter(new FileStream(file, FileMode.Create, FileAccess.ReadWrite), encoding))
                {
                    var stringBuilder = new StringBuilder(data);
                    sw.Write(stringBuilder.ToString());
                }
            else
            {
                var streamWriter = fileInfo.CreateText();
                streamWriter.Write(data);
                streamWriter.Close();
            }

            return true;
        }

        /// <summary>
        /// Detects the text encoding.
        /// Function to detect the encoding for UTF-7, UTF-8/16/32 (bom, no bom, little
        /// & big endian), and local default codepage, and potentially other codepages.
        /// 'checkSize' = number of bytes to check of the byte array (to save processing). Higher
        /// value is slower, but more reliable (especially UTF-8 with special characters
        /// later on may appear to be ASCII initially). If checkSize = 0, then checkSize
        /// becomes the length of the byte array (for maximum reliability). 'text' is simply
        /// the string with the discovered encoding applied to the byte array.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="text">The text.</param>
        /// <param name="checkSize">The checkSize.</param>
        /// <returns></returns>
        public static Encoding DetectTextEncoding(byte[] bytes, out string text, int checkSize = 1000)
        {
            // First check the low hanging fruit by checking if a
            // BOM/signature exists (sourced from http://www.unicode.org/faq/utf_bom.html#bom4)
            if (bytes.Length >= 4 && bytes[0] == 0x00 && bytes[1] == 0x00 && bytes[2] == 0xFE && bytes[3] == 0xFF)
            {
                text = Encoding.GetEncoding("utf-32BE").GetString(bytes, 4, bytes.Length - 4);
                return Encoding.GetEncoding("utf-32BE");
            }
            // UTF-32, big-endian 
            if (bytes.Length >= 4 && bytes[0] == 0xFF && bytes[1] == 0xFE && bytes[2] == 0x00 && bytes[3] == 0x00)
            {
                text = Encoding.UTF32.GetString(bytes, 4, bytes.Length - 4);
                return Encoding.UTF32;
            }
            // UTF-32, little-endian
            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            {
                text = Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);
                return Encoding.BigEndianUnicode;
            }
            // UTF-16, big-endian
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            {
                text = Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
                return Encoding.Unicode;
            }
            // UTF-16, little-endian (UTF-8)
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                text = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
                return Encoding.UTF8;
            }
            // UTF-7
            if (bytes.Length >= 3 && bytes[0] == 0x2b && bytes[1] == 0x2f && bytes[2] == 0x76)
            {
                text = Encoding.UTF7.GetString(bytes, 3, bytes.Length - 3);
                return Encoding.UTF7;
            }

            // If the code reaches here, no BOM/signature was found, so now
            // we need to 'touch' the byte array to see if can manually discover
            // the encoding. A high checkSize value is desired for UTF-8
            if (checkSize == 0 || checkSize > bytes.Length)
                checkSize = bytes.Length;    // Taster size can't be bigger than the filesize obviously.

            // Some text files are encoded in UTF8, but have no BOM/signature. Hence
            // the below manually checks for a UTF8 pattern. This code is based off
            // the top answer at: http://stackoverflow.com/questions/6555015/check-for-invalid-utf8
            // For our purposes, an unnecessarily strict (and terser/slower)
            // implementation is shown at: http://stackoverflow.com/questions/1031645/how-to-detect-utf-8-in-plain-c
            // For the below, false positives should be exceedingly rare (and would
            // be either slightly malformed UTF-8 (which would suit our purposes
            // anyway) or 8-bit extended ASCII/UTF-16/32 at a vanishingly long shot).
            var i = 0;
            var utf8 = false;
            while (i < checkSize - 4)
            {
                if (bytes[i] <= 0x7F)
                {
                    i += 1;
                    continue;
                }     // If all characters are below 0x80, then it is valid UTF8, but UTF8 is not 'required' (and therefore the text is more desirable to be treated as the default codepage of the computer). Hence, there's no "utf8 = true;" code unlike the next three checks.
                if (bytes[i] >= 0xC2 && bytes[i] <= 0xDF && bytes[i + 1] >= 0x80 && bytes[i + 1] < 0xC0)
                {
                    i += 2;
                    utf8 = true;
                    continue;
                }
                if (bytes[i] >= 0xE0 && bytes[i] <= 0xF0 && bytes[i + 1] >= 0x80 && bytes[i + 1] < 0xC0 &&
                    bytes[i + 2] >= 0x80 && bytes[i + 2] < 0xC0)
                {
                    i += 3;
                    utf8 = true;
                    continue;
                }
                if (bytes[i] >= 0xF0 && bytes[i] <= 0xF4 && bytes[i + 1] >= 0x80 && bytes[i + 1] < 0xC0 &&
                    bytes[i + 2] >= 0x80 && bytes[i + 2] < 0xC0 && bytes[i + 3] >= 0x80 && bytes[i + 3] < 0xC0)
                {
                    i += 4;
                    utf8 = true;
                    continue;
                }
                utf8 = false;
                break;
            }

            // UTF-8
            if (utf8)
            {
                text = Encoding.UTF8.GetString(bytes);
                return Encoding.UTF8;
            }

            // The next check is a heuristic attempt to detect UTF-16 without a BOM.
            // We simply look for zeroes in odd or even byte places, and if a certain
            // threshold is reached, the code is 'probably' UF-16.          
            var threshold = 0.1; // proportion of chars step 2 which must be zeroed to be diagnosed as utf-16. 0.1 = 10%
            var count = 0;
            for (var n = 0; n < checkSize; n += 2)
                if (bytes[n] == 0)
                    count++;
            if (((double)count) / checkSize > threshold)
            {
                text = Encoding.BigEndianUnicode.GetString(bytes);
                return Encoding.BigEndianUnicode;
            }
            count = 0;
            for (var n = 1; n < checkSize; n += 2)
                if (bytes[n] == 0)
                    count++;

            // (little-endian)
            if (((double)count) / checkSize > threshold)
            {
                text = Encoding.Unicode.GetString(bytes);
                return Encoding.Unicode;
            }

            // Finally, a long shot - let's see if we can find "charset=xyz" or
            // "encoding=xyz" to identify the encoding:
            for (var n = 0; n < checkSize - 9; n++)
            {
                if (
                    ((bytes[n + 0] == 'c' || bytes[n + 0] == 'C') && (bytes[n + 1] == 'h' || bytes[n + 1] == 'H') && (bytes[n + 2] == 'a' || bytes[n + 2] == 'A') && (bytes[n + 3] == 'r' || bytes[n + 3] == 'R') && (bytes[n + 4] == 's' || bytes[n + 4] == 'S') && (bytes[n + 5] == 'e' || bytes[n + 5] == 'E') && (bytes[n + 6] == 't' || bytes[n + 6] == 'T') && (bytes[n + 7] == '=')) ||
                    ((bytes[n + 0] == 'e' || bytes[n + 0] == 'E') && (bytes[n + 1] == 'n' || bytes[n + 1] == 'N') && (bytes[n + 2] == 'c' || bytes[n + 2] == 'C') && (bytes[n + 3] == 'o' || bytes[n + 3] == 'O') && (bytes[n + 4] == 'd' || bytes[n + 4] == 'D') && (bytes[n + 5] == 'i' || bytes[n + 5] == 'I') && (bytes[n + 6] == 'n' || bytes[n + 6] == 'N') && (bytes[n + 7] == 'g' || bytes[n + 7] == 'G') && (bytes[n + 8] == '='))
                    )
                {
                    if (bytes[n + 0] == 'c' || bytes[n + 0] == 'C')
                        n += 8;
                    else
                        n += 9;

                    if (bytes[n] == '"' || bytes[n] == '\'')
                        n++;

                    var oldn = n;
                    while (n < checkSize &&
                           (bytes[n] == '_' || bytes[n] == '-' || (bytes[n] >= '0' && bytes[n] <= '9') ||
                            (bytes[n] >= 'a' && bytes[n] <= 'z') || (bytes[n] >= 'A' && bytes[n] <= 'Z')))
                    {
                        n++;
                    }
                    var nb = new byte[n - oldn];
                    Array.Copy(bytes, oldn, nb, 0, n - oldn);
                    try
                    {
                        var internalEnc = Encoding.ASCII.GetString(nb);

                        // A.Spitzner: It's not needed to throw
                        // by filtering the crap
                        if (internalEnc == "System"
                            || internalEnc == "CharSet")
                            break;

                        text = Encoding.GetEncoding(internalEnc).GetString(bytes);
                        return Encoding.GetEncoding(internalEnc);
                    }
                    catch
                    {
                        // If C# doesn't recognize the name of the encoding, break.
                        break;
                    }
                }
            }


            // If all else fails, the encoding is probably (though certainly not
            // definitely) the user's local codepage! One might present to the user a
            // list of alternative encodings as shown here: http://stackoverflow.com/questions/8509339/what-is-the-most-common-encoding-of-each-language
            // A full list can be found using Encoding.GetEncodings();
            text = Encoding.Default.GetString(bytes);
            return Encoding.Default;
        }

        /// <summary>
        /// Begins the invoke.
        /// </summary>
        /// <param name="executeAction">The execute action.</param>
        /// <param name="priority">The priority.</param>
        public static void BeginInvoke(Action executeAction, DispatcherPriority priority = DispatcherPriority.Background)
        {
            if (Application.Current?.Dispatcher == null)
                return;

            Application.Current.Dispatcher.BeginInvoke(priority, executeAction);
        }

        /// <summary>
        /// Invokes the specified execute action.
        /// </summary>
        /// <param name="executeAction">The execute action.</param>
        public static void Invoke(Action executeAction)
        {
            try
            {
                if (Application.Current?.Dispatcher == null
                    || executeAction == null)
                    return;

                if (Application.Current.Dispatcher.CheckAccess())
                    executeAction();
                else
                    Application.Current.Dispatcher.Invoke(executeAction);
            }
            catch (TaskCanceledException)
            {
                // Ignore
            }
        }

        /// <summary>
        /// Executes the in thread.
        /// </summary>
        /// <param name="executeAction">The execute action.</param>
        /// <param name="apartmentState">State of the apartment.</param>
        /// <param name="waitUntilExecuted">if set to <c>true</c> [wait until executed].</param>
        public static void ExecuteInThread(
            Action executeAction,
            ApartmentState apartmentState = ApartmentState.STA,
            bool waitUntilExecuted = true)
        {
            if (executeAction == null)
                return;

            var resetEvent = new AutoResetEvent(false);
            Task.Run(() =>
            {
                executeAction();

                resetEvent.Set();
            });

            if (waitUntilExecuted)
                resetEvent.WaitOne();
        }

        /// <summary>
        /// The is console present
        /// </summary>
        private static bool? _IsConsolePresent;

        /// <summary>
        /// Gets the is console present.
        /// </summary>
        /// <returns><c>true</c> if true, <c>false</c> otherwise.</returns>
        public static bool GetIsConsolePresent()
        {
            try
            {
                if (_IsConsolePresent != null)
                    return _IsConsolePresent.Value;

                var windowHeight = Console.WindowHeight;
                _IsConsolePresent = true;
                return true;
            }
            catch
            {
                _IsConsolePresent = false;
                return false;
            }
        }
    }
}
