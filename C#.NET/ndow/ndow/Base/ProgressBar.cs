using System.Text;

namespace ndow.Base
{
    /// <summary>
    /// An ASCII progress bar
    /// </summary>
    public class ProgressBar : IDisposable, IProgress<double>
    {
        /// <summary>
        /// The number of blocks in the progress bar
        /// </summary>
        private int _BlockCount;

        /// <summary>
        /// The animation interval for the progress bar
        /// </summary>
        private readonly TimeSpan _AnimationInterval = TimeSpan.FromSeconds(1.0 / 8);

        /// <summary>
        /// The animation sequence for the progress indicator
        /// </summary>
        private const string _Animation = @"|/-\";

        /// <summary>
        /// Whether to show the progress bar or just the animation
        /// </summary>
        private bool _ShowProgressBar = true;

        /// <summary>
        /// The timer used to update the progress bar
        /// </summary>
        private readonly Timer _Timer;

        /// <summary>
        /// Current progress value (between 0 and 1)
        /// </summary>
        private double _CurrentProgress = 0;

        /// <summary>
        /// The current text displayed in the console
        /// </summary>
        private string _CurrentText = string.Empty;

        /// <summary>
        /// The text to display alongside the progress bar
        /// </summary>
        private string? _ShowText = string.Empty;

        /// <summary>
        /// Indicates whether the progress bar has been disposed
        /// </summary>
        private bool _Disposed = false;

        /// <summary>
        /// The current index in the animation sequence
        /// </summary>
        private int _AnimationIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        /// <param name="showProgressBar">Whether to show the progress bar</param>
        /// <param name="blockCount">The number of blocks in the progress bar</param>
        public ProgressBar(bool showProgressBar = true, int blockCount = 20)
        {
            _ShowProgressBar = showProgressBar;
            _BlockCount = blockCount;
            _Timer = new Timer(TimerHandler);

            // A progress bar is only for temporary display in a console window.
            // If the console output is redirected to a file, draw nothing.
            // Otherwise, we'll end up with a lot of garbage in the target file.
            if (!Console.IsOutputRedirected)
                ResetTimer();
        }

        /// <summary>
        /// Reports a progress update to the progress bar.
        /// </summary>
        /// <param name="value"></param>
        public void Report(double value)
        {
            // Make sure value is in [0..1] range
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref _CurrentProgress, value);
        }

        /// <summary>
        /// Reports a progress update to the progress bar.
        /// </summary>
        /// <param name="value">The progress value (between 0 and 1)</param>
        /// <param name="text">The text to display</param>
        public void SetProgress(double value, string? text = null)
        {
            Report(value);
            Interlocked.Exchange(ref _ShowText, text);
        }

        /// <summary>
        /// Timer callback to update the progress bar display
        /// </summary>
        /// <param name="state"></param>
        private void TimerHandler(object? state)
        {
            lock (_Timer)
            {
                if (_Disposed) return;

                string text = !string.IsNullOrEmpty(_ShowText) ? _ShowText : string.Empty;
                if (_ShowProgressBar)
                {
                    int progressBlockCount = (int)(_CurrentProgress * _BlockCount);
                    int percent = (int)(_CurrentProgress * 100);
                    text = $"{_Animation[_AnimationIndex++ % _Animation.Length]}" +
                        $" {percent.ToString().PadLeft(3, ' ')}% " +
                        $"[{new string('#', progressBlockCount)}{new string('-', _BlockCount - progressBlockCount)}]" +
                        $" {text}";
                }
                else
                {
                    text += _Animation[_AnimationIndex++ % _Animation.Length].ToString();
                }

                UpdateText(text);

                ResetTimer();
            }
        }

        /// <summary>
        /// Updates the text displayed in the console.
        /// </summary>
        /// <param name="text">The text to display</param>
        private void UpdateText(string text)
        {
            // Get length of common portion
            int commonPrefixLength = 0;
            int commonLength = Math.Min(_CurrentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == _CurrentText[commonPrefixLength])
            {
                commonPrefixLength++;
            }

            // Backtrack to the first differing character
            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', _CurrentText.Length - commonPrefixLength);

            // Output new suffix
            outputBuilder.Append(text.Substring(commonPrefixLength));

            // If the new text is shorter than the old one: delete overlapping characters
            int overlapCount = _CurrentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.Write(outputBuilder);
            _CurrentText = text;
        }

        /// <summary>
        /// Resets the timer to trigger the next update after the animation interval.
        /// </summary>
        private void ResetTimer()
        {
            _Timer.Change(_AnimationInterval, TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (_Timer)
            {
                _Disposed = true;
                UpdateText(string.Empty);
            }
        }
    }
}