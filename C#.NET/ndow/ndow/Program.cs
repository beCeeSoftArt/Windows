using ndow.Base;
using ndow.Extensions;
using System.Diagnostics;

internal class Program
{
    #region Parameter

    /// <summary>
    /// Indicates whether to show only the progress bar without additional messages.
    /// </summary>
    private static bool _IsOnlyProgress;

    #endregion

    /// <summary>
    /// The SocketsHttpHandler instance used for configuring the HttpClient.
    /// </summary>
    private static SocketsHttpHandler? _Handler;

    /// <summary>
    /// The HttpClient instance used for downloading files.
    /// </summary>
    private static HttpClient? _HttpClient;

    private static void Main(string[] args)
    {
        var commandLineArgs = Os.CommandLineParameters;

        Uri? uri = null;
        if (commandLineArgs.Count > 0)
        {
            // First parameter is the application name
            var commandParameters = commandLineArgs[0];

            if (commandParameters.Count < 2)
            {
                Console.WriteLine("Invalid parameters.");
                return;
            }

            // Second parameter must contain an uri
            if (!commandParameters[1].IsValidUri())
            {
                Console.WriteLine("Invalid uri.");
                return;
            }
            else
                uri = commandParameters[1].ToValidUri();

            string? fileName = "";

            // Only First and Second parameter are given,
            // so we will use the file name from the uri.
            if (commandParameters.Count == 2)
                fileName = uri?.GetFileName();

            //  First, Second and Third parameter is given,
            //  so we will use the given file name from Third parameter.
            else if (commandParameters.Count == 3)
                fileName = commandParameters[2];

            if (!string.IsNullOrEmpty(fileName)
                && uri != null)
            {
                var fileInfo = new FileInfo(fileName);
                var userName = Os.GetParameterValue<string>(new List<string> { "-user:", "-u:" });
                var userPassword = Os.GetParameterValue<string>(new List<string> { "-password:", "-p:" });
                _IsOnlyProgress = Os.GetParameterValue<bool>(new List<string> { "-onlyprogress:", "-op:" });

                if (!_IsOnlyProgress)
                    Console.WriteLine($"Download from \"{uri.OriginalString}\" to \"{fileName}\"\r\n");


                _Handler = new SocketsHttpHandler()
                {
                    AllowAutoRedirect = true,
                    // Recreate every 15 minutes
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15),
                    MaxConnectionsPerServer = 2,
                    Expect100ContinueTimeout = TimeSpan.FromSeconds(1),
                    Credentials = !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userPassword) ? new System.Net.NetworkCredential(userName, userPassword) : null
                };

                _HttpClient = new HttpClient(_Handler)
                {
                    Timeout = TimeSpan.FromSeconds(30),
                    DefaultRequestHeaders = { ExpectContinue = true }
                };

                DownloadFileAsync(uri, fileName).GetAwaiter().GetResult();

                _HttpClient.Dispose();
                _Handler.Dispose();
            }
        }
        else
        {
            Console.WriteLine("Parameter usage:\r\n" +
                "ndow.exe <uri> [<file name>] [-user:<username>] [-password:<password>] [-onlyprogress:true|false]");
        }
    }


    private static void ClearLine(int lineNumber)
    {
        int currentLineCursor = Console.CursorTop; // Save current cursor position
        Console.SetCursorPosition(0, lineNumber);  // Move to target line
        Console.Write(new string(' ', Console.BufferWidth)); // Overwrite with spaces
        Console.SetCursorPosition(0, currentLineCursor); // Restore cursor
    }

    ///// <summary>
    ///// Downloads a file with retry and resume support.
    ///// </summary>
    //private static async Task DownloadWithRetryAsync(Uri uri, string destinationPath, int maxRetries)
    //{
    //    int attempt = 0;
    //    long existingLength = File.Exists(destinationPath) ? new FileInfo(destinationPath).Length : 0;

    //    while (attempt < maxRetries)
    //    {
    //        try
    //        {
    //            using var request = new HttpRequestMessage(HttpMethod.Get, uri.OriginalString);

    //            // If partial file exists, request the remaining bytes
    //            if (existingLength > 0)
    //            {
    //                request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(existingLength, null);
    //                Console.WriteLine($"Resuming from byte {existingLength}...");
    //            }

    //            using var response = await _HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

    //            response.EnsureSuccessStatusCode();

    //            using var stream = await response.Content.ReadAsStreamAsync();
    //            using var fileStream = new FileStream(destinationPath, FileMode.Append, FileAccess.Write, FileShare.None);

    //            await stream.CopyToAsync(fileStream);
    //            return; // Success
    //        }
    //        catch (HttpRequestException ex) when (ex.InnerException is SocketException)
    //        {
    //            attempt++;
    //            Console.WriteLine($"Connection lost. Retrying {attempt}/{maxRetries}...");
    //            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt))); // Exponential backoff
    //        }
    //        catch (TaskCanceledException)
    //        {
    //            attempt++;
    //            Console.WriteLine($"Timeout. Retrying {attempt}/{maxRetries}...");
    //            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
    //        }
    //    }

    //    throw new Exception("Max retries reached. Download failed.");
    //}

    /// <summary>
    /// Downloads a file from the specified URI to the given destination path with progress reporting.
    /// </summary>
    /// <param name="uri">The URI of the file to download.</param>
    /// <param name="destinationPath">The path where the file will be saved.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal static async Task DownloadFileAsync(Uri uri, string destinationPath)
    {
        try
        {
            if (_HttpClient == null)
                return;

            // 8 KB cache size
            var cacheSize = 8192;
            using var progressBar = new ProgressBar();
            using var response = await _HttpClient.GetAsync(uri.OriginalString, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                var totalBytes = response.Content.Headers.ContentLength;
                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);

                var buffer = new byte[cacheSize];
                long totalBytesRead = 0;
                int bytesRead;

                var cursorPosition = Console.GetCursorPosition();
                var moduloFactor = 1;
                long maximum = totalBytes.HasValue ? totalBytes.Value : 0;

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {

                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;

                    if (totalBytes.HasValue)
                    {
                        var currentProgress = totalBytesRead / (totalBytes.Value / 100);
                        double progress = (double)totalBytesRead / totalBytes.Value * 100;
                        double bytesToDownload = totalBytes.Value - totalBytesRead;
                        TimeSpan remainingTime = TimeSpan.Zero;
                        if (currentProgress % moduloFactor == 0 || totalBytesRead == bytesRead)
                        {
                            // Calculate speed
                            double seconds = stopwatch.Elapsed.TotalSeconds;
                            double speedKBps = 0;
                            if (seconds > 0)
                            {
                                speedKBps = (totalBytesRead / seconds);

                                remainingTime = TimeSpan.FromSeconds(bytesToDownload / (speedKBps));
                            }

                            progressBar.SetProgress(
                                progress / 100,
                                $"D: {totalBytesRead.ToReadableSiUnit("B", shortName: true)}/{totalBytes.Value.ToReadableSiUnit("B", shortName: true)}, " +
                                $"S: {((long)(speedKBps)).ToReadableSiUnit("B/s", shortName: true)}, T: {remainingTime:dd\\.hh\\:mm\\:ss} ");
                        }

                        //Thread.Sleep(500);
                    }
                }

                if (!_IsOnlyProgress)
                    Console.WriteLine($"Download of {(totalBytes.HasValue ? totalBytes.Value.ToReadableSiUnit("B", shortName: true) : "unknown size")} completed!");
            }
            else
            {
                Console.WriteLine($"Failed to download file {uri.OriginalString}. Status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while downloading the file: {ex.Message}"); Console.WriteLine(ex.ToString());
        }
    }
}