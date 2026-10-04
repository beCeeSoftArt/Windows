namespace ndow.Extensions
{
    internal static class StreamExtension
    {
        public enum TimeOutMethod
        {
            /// <summary>
            /// Timeout is calculated independently by background waiting task.
            /// </summary>
            IndependentWaiting,

            /// <summary>
            /// Timeout is calculated by the new cancellation token linked with source operation token.
            /// This methods works well only if async operation periodically checks cancellation token.
            /// </summary>
            LinkedCancelattion,

            /// <summary>
            /// Timeout is calculated by Stream itself.
            /// </summary>
            StreamTimeout,
        }

        /// <summary>
        /// CopyToAsync extension method.
        /// Method reads data from source stream in chunks. A chunk is a memory buffer of the required size.
        /// Method limits buffer filling operation by timeout.
        /// If the timeout expires before the buffer is full is complete, the method throws a TimeoutException.
        /// </summary>
        /// <param name="source">Source stream</param>
        /// <param name="destination">Destination stream</param>
        /// <param name="readBufferSize">Size in bytes of buffer for reading opeartion</param>
        /// <param name="readBufferTimeOut">Timeout of reading the buffer</param>
        /// <param name="timeOutMethod">Timeout calculation method</param>
        /// <param name="cancellationToken">Operation cancellation method</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="TimeoutException"></exception>
        public static Task CopyToAsync(
            this Stream source,
            Stream destination,
            int readBufferSize,
            TimeSpan readBufferTimeOut,
            TimeOutMethod timeOutMethod = TimeOutMethod.IndependentWaiting,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = default(Task);
            var token = (cancellationToken != default(CancellationToken)) ? cancellationToken : CancellationToken.None;

            switch (timeOutMethod)
            {
                case TimeOutMethod.IndependentWaiting:
                    res = CopyToIndependentWaitingAsync(source, destination, readBufferSize, readBufferTimeOut, token);
                    break;

                case TimeOutMethod.LinkedCancelattion:
                    res = CopyToLinkedCancellationAsync(source, destination, readBufferSize, readBufferTimeOut, token);
                    break;

                case TimeOutMethod.StreamTimeout:
                    res = CopyToStreamTimeoutAsync(source, destination, readBufferSize, readBufferTimeOut, token);
                    break;

                default:
                    throw new ArgumentException(nameof(timeOutMethod));
            }

            return res;
        }

        #region Helper Methods

        static async Task CopyToIndependentWaitingAsync(Stream source, Stream destination, int readBufferSize, TimeSpan readBufferTimeOut, CancellationToken cancellationToken)
        {
            var chunk = new byte[readBufferSize];

            var read = 0;
            do
            {
                var readingTask = source.ReadAsync(chunk, 0, chunk.Length, cancellationToken);
                var waitingTask = Task.Delay(readBufferTimeOut, cancellationToken);
                var tasks = new Task[] { readingTask, waitingTask };

                var taskIndex = Task.WaitAny(tasks, cancellationToken);
                if (tasks[taskIndex].IsCanceled)
                {
                    // normal task cancellation
                    throw new TaskCanceledException(tasks[taskIndex]);
                }
                else if (tasks[taskIndex].Exception != default(Exception))
                {
                    // it re-throws a task exception with task stack information
                    await tasks[taskIndex];
                }

                if (taskIndex != 0)
                {
                    throw new TimeoutException();
                }

                read = readingTask.Result;
                if (read > 0)
                {
                    await destination.WriteAsync(chunk, 0, read, cancellationToken).ConfigureAwait(false);
                }
            }
            while (read != 0);
        }

        static async Task CopyToLinkedCancellationAsync(Stream source, Stream destination, int readBufferSize, TimeSpan readBufferTimeOut, CancellationToken cancellationToken)
        {
            var chunk = new byte[readBufferSize];

            var read = 0;
            do
            {
                try
                {
                    using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                    {
                        cts.CancelAfter(readBufferTimeOut);

                        read = await source.ReadAsync(chunk, 0, chunk.Length, cts.Token).ConfigureAwait(false);
                        if (read > 0)
                        {
                            await destination.WriteAsync(chunk, 0, read, cancellationToken).ConfigureAwait(false);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }
                    else
                    {
                        throw new TimeoutException();
                    }
                }
            }
            while (read != 0);
        }

        static async Task CopyToStreamTimeoutAsync(Stream source, Stream destination, int readBufferSize, TimeSpan readBufferTimeOut, CancellationToken cancellationToken)
        {
            var oldReadTimeout = source.ReadTimeout;
            source.ReadTimeout = (int)readBufferTimeOut.TotalMilliseconds;

            try
            {
                await source.CopyToAsync(destination, readBufferSize, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                source.ReadTimeout = oldReadTimeout;
            }
        }

        #endregion
    }
}
