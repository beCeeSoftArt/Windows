/* Copyright André Spitzner 1977 - 2020 */
using farif.Base;
using farif.Properties;
using farif.Tools;
using farif.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace farif
{
    /// <summary>
    /// Class MainWindow.
    /// Implements the <see cref="System.Windows.Window" />
    /// Implements the <see cref="System.Windows.Markup.IComponentConnector" />
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class MainWindow
    {
        /// <summary>
        /// The current file count
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private int _CurrentFileCount;

        /// <summary>
        /// The file count
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private int _FileCount;

        /// <summary>
        /// The file filter item source
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private ObservableCollection<CheckItem> _FileFilterItemSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnMainWindowLoaded;
        }

        /// <summary>
        /// Handles the <see cref="E:MainWindowLoaded" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckBoxRepairLineEndings.IsChecked = Settings.Default.IsRepairLfCr;

            // Create list for file filter
            var tempList = Settings.Default.FileFilter.OfType<string>().Select(n =>
            {
                var splitText = n.Split(';');
                if (splitText.Length > 1)
                {
                    bool.TryParse(splitText[0], out var isChecked);
                    return new CheckItem(null, splitText[1])
                    {
                        IsChecked = isChecked
                    };
                }

                return new CheckItem(null, splitText[1])
                {
                    IsChecked = true
                };

            }).ToList();
            _FileFilterItemSource = new ObservableCollection<CheckItem>();
            foreach (var checkItem in tempList)
            {
                checkItem.PropertyChanged += OnCheckItemPropertyChanged;
                _FileFilterItemSource.Add(checkItem);
            }

            // Initialize Created file filter list to UI
            CheckListBoxFileFilter.ItemsSource = _FileFilterItemSource;

            // Initialize Is repair line endings checked
            CheckBoxRepairLineEndings.IsChecked = Settings.Default.IsRepairLfCrEnabledState;
            CheckBoxRepairLineEndings.Checked += OnCheckBoxRepairLineEndingsCheckedChanged;
            CheckBoxRepairLineEndings.Unchecked += OnCheckBoxRepairLineEndingsCheckedChanged;

            // Initialize Line endings
            ComboBoxLineEndings.ItemsSource = LineEndingConstants.LineEndings;
            ComboBoxLineEndings.DisplayMemberPath = nameof(LineEnding.DisplayName);

            // Initialize Select last selected line ending
            if (!string.IsNullOrEmpty(Settings.Default.SelectedLineEnding))
                ComboBoxLineEndings.SelectedItem = LineEndingConstants.LineEndings
                    .FirstOrDefault(n =>
                        string.Equals(n.DisplayName, Settings.Default.SelectedLineEnding, StringComparison.Ordinal));
            // Initialize otherwise select first entry 
            else if (LineEndingConstants.LineEndings.Count > 0)
                ComboBoxLineEndings.SelectedIndex = 0;
            ComboBoxLineEndings.SelectionChanged += OnComboBoxLineEndingsSelectionChanged;

            // Initialize Input folder
            if (!string.IsNullOrEmpty(Settings.Default.SelectedInputPath))
                TextBoxInputPath.Text = Settings.Default.SelectedInputPath;
            TextBoxInputPath.TextChanged += OnTextBoxInputPathTextChanged;

            // Initialize Try detect encoding
            CheckBoxIsTryDetectEncoding.IsChecked = Settings.Default.IsTryDetectEncoding;
            CheckBoxIsTryDetectEncoding.Checked += OnCheckBoxIsTryDetectEncodingCheckedChanged;
            CheckBoxIsTryDetectEncoding.Unchecked += OnCheckBoxIsTryDetectEncodingCheckedChanged;

            // Initialize Include Subfolder
            CheckBoxIsIncludeSubFolders.IsChecked = Settings.Default.IsIncludeSubFolders;
            CheckBoxIsIncludeSubFolders.Checked += OnCheckBoxIsIncludeSubFoldersCheckedChanged;
            CheckBoxIsIncludeSubFolders.Unchecked += OnCheckBoxIsIncludeSubFoldersCheckedChanged;

            // Initialize Is search and replace
            CheckBoxIsSearchAndReplace.IsChecked = Settings.Default.IsSearchAndReplace;
            CheckBoxIsSearchAndReplace.Checked += OnCheckBoxIsSearchAndReplaceCheckedChanged;
            CheckBoxIsSearchAndReplace.Unchecked += OnCheckBoxIsSearchAndReplaceCheckedChanged;

            // Initialize Is search and replace case sensitive
            CheckBoxIsSearchAndReplaceCaseSensitive.IsChecked = Settings.Default.IsSearchAndReplaceCaseSensitive;
            CheckBoxIsSearchAndReplaceCaseSensitive.Checked += OnCheckBoxIsSearchAndReplaceCaseSensitiveCheckedChanged;
            CheckBoxIsSearchAndReplaceCaseSensitive.Unchecked += OnCheckBoxIsSearchAndReplaceCaseSensitiveCheckedChanged;

            // Initialize Last replace text
            TextBoxReplaceText.Text = Settings.Default.LastReplaceText;
            TextBoxReplaceText.TextChanged += OnTextBoxReplaceTextTextChanged;

            // Initialize Last search text
            TextBoxSearchText.Text = Settings.Default.LastSearchText;
            TextBoxSearchText.TextChanged += OnTextBoxSearchTextChanged;

            // Update enable state of controls
            SetEnabledStates(false);
        }

        /// <summary>
        /// Handles the <see cref="E:CheckBoxIsSearchAndReplaceCheckedChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCheckBoxIsSearchAndReplaceCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!CheckBoxIsSearchAndReplace.IsChecked.HasValue)
                return;

            Settings.Default.IsSearchAndReplace = CheckBoxIsSearchAndReplace.IsChecked.Value;
            Settings.Default.Save();
        }

        /// <summary>
        /// Handles the <see cref="E:CheckBoxIsSearchAndReplaceCaseSensitiveCheckedChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCheckBoxIsSearchAndReplaceCaseSensitiveCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!CheckBoxIsSearchAndReplaceCaseSensitive.IsChecked.HasValue)
                return;

            Settings.Default.IsSearchAndReplaceCaseSensitive = CheckBoxIsSearchAndReplaceCaseSensitive.IsChecked.Value;
            Settings.Default.Save();
        }

        /// <summary>
        /// Handles the <see cref="E:TextBoxReplaceTextTextChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
        private void OnTextBoxReplaceTextTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Settings.Default.LastReplaceText = TextBoxReplaceText.Text;
            Settings.Default.Save();
        }

        /// <summary>
        /// Handles the <see cref="E:TextBoxSearchTextChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
        private void OnTextBoxSearchTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Settings.Default.LastSearchText = TextBoxSearchText.Text;
            Settings.Default.Save();
        }

        /// <summary>
        /// Handles the <see cref="E:CheckBoxIsIncludeSubFoldersCheckedChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCheckBoxIsIncludeSubFoldersCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!CheckBoxIsIncludeSubFolders.IsChecked.HasValue)
                return;

            Settings.Default.IsIncludeSubFolders = CheckBoxIsIncludeSubFolders.IsChecked.Value;
            Settings.Default.Save();
        }

        /// <summary>
        /// Handles the <see cref="E:CheckBoxIsTryDetectEncodingCheckedChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCheckBoxIsTryDetectEncodingCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!CheckBoxIsTryDetectEncoding.IsChecked.HasValue)
                return;

            Settings.Default.IsTryDetectEncoding = CheckBoxIsTryDetectEncoding.IsChecked.Value;
            Settings.Default.Save();
        }

        /// <summary>
        /// Handles the <see cref="E:CheckBoxRepairLineEndingsCheckedChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCheckBoxRepairLineEndingsCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!CheckBoxRepairLineEndings.IsChecked.HasValue)
                return;

            Settings.Default.IsRepairLfCrEnabledState = CheckBoxRepairLineEndings.IsChecked.Value;
            Settings.Default.Save();
        }

        /// <summary>
        /// Handles the <see cref="E:TextBoxInputPathTextChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
        private void OnTextBoxInputPathTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Settings.Default.SelectedInputPath = TextBoxInputPath.Text;
            Settings.Default.Save();
        }

        /// <summary>
        /// Handles the <see cref="E:ComboBoxLineEndingsSelectionChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnComboBoxLineEndingsSelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!(ComboBoxLineEndings.SelectedItem is LineEnding selectedLineEnding))
                return;

            Settings.Default.SelectedLineEnding = selectedLineEnding.DisplayName;
            Settings.Default.Save();
        }

        /// <summary>
        /// Handles the <see cref="E:CheckItemPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnCheckItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                // Save selected file filter when changed
                case nameof(CheckItem.IsChecked):

                    Settings.Default.FileFilter.Clear();

                    foreach (var checkItem in _FileFilterItemSource)
                        Settings.Default.FileFilter.Add($"{checkItem.IsChecked};{checkItem.Display}");

                    Settings.Default.Save();

                    break;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:SelectInputPathClick" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnSelectInputPathClick(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = TextBoxInputPath.Text,
                Description = @"Please select an input path."
            };

            var result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                TextBoxInputPath.Text = folderBrowserDialog.SelectedPath;
        }

        /// <summary>
        /// Handles the <see cref="E:StartProcessingClick" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnStartProcessingClick(object sender, RoutedEventArgs e)
        {
            SetEnabledStates(true);

            ProcessFiles();
        }

        /// <summary>
        /// Sets the enabled states.
        /// </summary>
        /// <param name="isProcessing">if set to <c>true</c> [is processing].</param>
        private void SetEnabledStates(bool isProcessing)
        {
            ButtonStartProcessing.IsEnabled = !isProcessing;
            ButtonStopProcessing.IsEnabled = isProcessing;
            ButtonAddFilter.IsEnabled = !isProcessing;
            ButtonRemoveFilter.IsEnabled = !isProcessing;

            CheckBoxRepairLineEndings.IsEnabled = !isProcessing;
            CheckBoxIsTryDetectEncoding.IsEnabled = !isProcessing;
            CheckListBoxFileFilter.IsEnabled = !isProcessing;
            CheckBoxIsSearchAndReplace.IsEnabled = !isProcessing;
            CheckBoxIsSearchAndReplaceCaseSensitive.IsEnabled = !isProcessing;

            ComboBoxLineEndings.IsEnabled = !isProcessing;

            TextBoxInputPath.IsEnabled = !isProcessing;
            TextBoxOutput.IsEnabled = !isProcessing;
            TextBoxReplaceText.IsEnabled = !isProcessing;
            TextBoxSearchText.IsEnabled = !isProcessing;
            TextBoxAddFilter.IsEnabled = !isProcessing;
        }

        /// <summary>
        /// Handles the <see cref="E:StopProcessingClick" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnStopProcessingClick(object sender, RoutedEventArgs e)
        {
            Utility.BeginInvoke(() =>
            {
                SetEnabledStates(false);
            });
        }

        /// <summary>
        /// Adds the output.
        /// </summary>
        /// <param name="output">The output.</param>
        private void AddOutput(string output)
        {
            Debug.WriteLine($"{nameof(AddOutput)}: \"{output}\"");

            // Hybrid, when application is in console started
            if (Utility.GetIsConsolePresent())
            {
                Console.WriteLine(output);
                return;
            }

            Utility.BeginInvoke(() =>
            {
                TextBoxOutput.AppendText($"{output}\r\n");
                TextBoxOutput.ScrollToEnd();
                GroupBoxOutput.Header = $"Output (Processed files {_CurrentFileCount}/{_FileCount})";
            });
        }

        /// <summary>
        /// Processes the files.
        /// </summary>
        private void ProcessFiles()
        {
            // Do that parallel in another thread to do not block UI thread
            Utility.ExecuteInThread(() =>
            {
                Utility.Invoke(() =>
                {
                    TextBoxOutput.Text = "";
                });

                var files = ReadInputDirectory();

                if (files != null
                    && files.Count > 0)
                {
                    AddOutput($"{files.Count} files found to process.");
                    AddOutput("Start processing");

                    // Process files
                    foreach (var file in files)
                    {
                        _CurrentFileCount++;
                        ProcessFile(file);
                    }

                    AddOutput($"{files.Count} files processed.");
                }

                // reenable UI
                Utility.BeginInvoke(() =>
                {
                    SetEnabledStates(false);
                });
            }, waitUntilExecuted: false);
        }

        /// <summary>
        /// Reads the input directory.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        private List<string> ReadInputDirectory()
        {
            if (!Directory.Exists(Settings.Default.SelectedInputPath))
            {
                AddOutput($"Directory \"{Settings.Default.SelectedInputPath}\" not found, processing is canceled. Please check input folder.");
                return null;
            }

            AddOutput($"Read input folder \"{Settings.Default.SelectedInputPath}\"");

            _CurrentFileCount = 0;
            _FileCount = 0;

            var activeFileFilters = _FileFilterItemSource.Where(n => n.IsChecked).ToList();

            var sb = new StringBuilder();
            var files = Directory.GetFiles(
                    Settings.Default.SelectedInputPath,
                    "*.*",
                    Settings.Default.IsIncludeSubFolders
                        ? SearchOption.AllDirectories
                        : SearchOption.TopDirectoryOnly)
                .Where(n =>
                {
                    foreach (var activeFileFilter in activeFileFilters)
                        if (n.ToLower().EndsWith(activeFileFilter.Display.ToLower()))
                        {
                            var fileInfo = new FileInfo(n);
                            if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly
                                || (fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden
                                || (fileInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                            {
                                sb.AppendLine(
                                    $"File \"{n}\" will be automatically ignored, because file is readonly, hidden or system file.");
                                return false;
                            }

                            sb.AppendLine($"File \"{n}\" will be processed.");

                            return true;
                        }

                    return false;
                })
                .ToList();

            _FileCount = files.Count;
            AddOutput(sb.ToString());

            return files;
        }

        /// <summary>
        /// Processes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private void ProcessFile(string filePath)
        {
            var sb = new StringBuilder();
            try
            {
                sb.AppendLine($"{_CurrentFileCount:D8} Processing: \"{filePath}\"");

                Encoding encoding = null;
                string fileTextContent = null;
                // Detect file encoding if requested
                if (Settings.Default.IsTryDetectEncoding)
                {
                    encoding = Utility.DetectTextEncoding(
                        Utility.GetFileData(filePath),
                        out _,
                        Utility.GetFileSize(filePath));

                    sb.AppendLine(encoding != null
                        ? $"\t\tEncoding \"{encoding.EncodingName}\" detected."
                        : "\t\tNo encoding detected.");
                }

                // Check if line endings should be unified
                if (Settings.Default.IsRepairLfCrEnabledState
                    && !string.IsNullOrEmpty(Settings.Default.SelectedLineEnding))
                {
                    // Get selected line ending
                    // for rebuild file content later
                    var selectedLineEnding =
                        LineEndingConstants.LineEndings.FirstOrDefault(n =>
                            n.DisplayName == Settings.Default.SelectedLineEnding);

                    // Check if LineEnding is selected
                    if (selectedLineEnding != null)
                    {
                        // Check if there any line endings contained in file
                        var result = Utility.HasLineBreaks(filePath, encoding);
                        if (result)
                        {
                            // If file contains line break, so replace all line endings with selected
                            var fileTextLines = Utility.GetFileTextLines(filePath, encoding);
                            if (fileTextLines != null
                                && fileTextLines.Count > 0)
                            {
                                // Rebuild file content
                                fileTextContent =
                                    string.Join(selectedLineEnding.FileUsageName, fileTextLines);

                                // Save file back
                                Utility.SetFileData(filePath, fileTextContent, encoding);

                                sb.AppendLine($"\t\tLine ending of \"{selectedLineEnding.DisplayName}\" harmonization processed.");
                            }
                            else
                                sb.AppendLine("\t\tFile contains no lines.");
                        }
                        else
                            sb.AppendLine("\t\tFile contains no line endings.");
                    }
                    else
                        sb.AppendLine("\t\tLine ending harmonization canceled, no line ending is selected.");

                }

                // Then check if something is to replace
                if (Settings.Default.IsSearchAndReplace)
                {
                    // Check if search text has content
                    if (string.IsNullOrEmpty(Settings.Default.LastSearchText))
                    {
                        sb.AppendLine("\t\tSearch text can not be empty. File ignored.");
                        return;
                    }

                    // Load file content if not loaded yet
                    if (fileTextContent == null)
                        fileTextContent = Utility.GetFileText(filePath, encoding);

                    if (!string.IsNullOrEmpty(fileTextContent)
                        && encoding != null)
                    {
                        // Convert file text to Utf32 byte array
                        var decodedFileTextBufferUtf32 = Encoding.Convert(
                            encoding,
                            Encoding.UTF32,
                            encoding.GetBytes(fileTextContent));

                        // Convert search text to Utf32 byte array
                        var decodedSearchTextBufferUtf32 = Encoding.Convert(
                            Encoding.UTF32,
                            Encoding.UTF8,
                            Encoding.UTF8.GetBytes(Settings.Default.LastSearchText));

                        // Convert replace text to Utf32 byte array
                        var decodedReplaceTextBufferUtf32 = Encoding.Convert(
                            Encoding.UTF32,
                            Encoding.UTF8,
                            Encoding.UTF8.GetBytes(Settings.Default.LastReplaceText));

                        // After encoding convert file text back to Utf32 string 
                        var decodedFileTextUtf32 = Encoding.UTF32.GetString(decodedFileTextBufferUtf32);

                        // After encoding convert search text back to Utf32 string 
                        var decodedSearchTextUtf32 = Encoding.UTF32.GetString(decodedSearchTextBufferUtf32);

                        // After encoding convert replace text back to Utf32 string 
                        var decodedReplaceTextUtf32 = Encoding.UTF32.GetString(decodedReplaceTextBufferUtf32);

                        // Check if file content contains search text 
                        if (!Utility.ContainsString(
                            decodedFileTextUtf32,
                            decodedSearchTextUtf32,
                            Settings.Default.IsSearchAndReplaceCaseSensitive
                                ? CompareOptions.Ordinal
                                : CompareOptions.IgnoreCase))
                        {
                            sb.AppendLine("\t\tFile does not contain search text. Search and replace operation ignored.");
                            return;
                        }

                        // Replace all occurrences of search text with replace text
                        var decodedResultTextUtf32 =
                            Utility.ReplaceString(
                                decodedFileTextUtf32,
                                decodedSearchTextUtf32,
                                decodedReplaceTextUtf32,
                                Settings.Default.IsSearchAndReplaceCaseSensitive
                                    ? StringComparison.Ordinal
                                    : StringComparison.OrdinalIgnoreCase);

                        // Convert changed file text Utf32 to destination encoding byte array
                        var resultFileTextBufferDestinationEncoding = Encoding.Convert(
                            encoding,
                            Encoding.UTF32,
                            Encoding.UTF32.GetBytes(decodedResultTextUtf32));

                        // Convert file byte array with destination encoding back to string
                        var resultFileTextDestinationEncoding =
                            encoding.GetString(resultFileTextBufferDestinationEncoding);

                        // Save file text back
                        Utility.SetFileData(filePath, resultFileTextDestinationEncoding, encoding);

                        sb.AppendLine("\t\tSearch and replace operation processed.");
                    }

                }

                sb.AppendLine("\t\tFile processed.");
            }
            catch (Exception e)
            {
                AddOutput(e.ToString());
            }
            finally
            {
                AddOutput(sb.ToString());
            }
        }

        /// <summary>
        /// Handles the <see cref="E:AddFilterClick" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnAddFilterClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxAddFilter.Text))
                return;

            // Create new extension filter 
            var itemToAdd = $"True;{TextBoxAddFilter.Text}";

            // Add and save configuration
            Settings.Default.FileFilter.Add(itemToAdd);
            Settings.Default.Save();

            // Create item for UI and display it
            var newFilterExtension = new CheckItem(null, TextBoxAddFilter.Text)
            {
                IsChecked = true
            };
            _FileFilterItemSource.Add(newFilterExtension);

            // Connect changed filter changed event
            newFilterExtension.PropertyChanged -= OnCheckItemPropertyChanged;

            CheckListBoxFileFilter.ScrollIntoView(newFilterExtension);
        }

        /// <summary>
        /// Handles the <see cref="E:RemoveFilterClick" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnRemoveFilterClick(object sender, RoutedEventArgs e)
        {
            if (!(CheckListBoxFileFilter.SelectedValue is CheckItem selectedValue))
                return;

            // Disconnect changed filter changed event
            selectedValue.PropertyChanged -= OnCheckItemPropertyChanged;

            // Create extension filter 
            var itemToRemove = $"{selectedValue.IsChecked};{selectedValue.Display}";

            // Remove and save configuration
            Settings.Default.FileFilter.Remove(itemToRemove);
            Settings.Default.Save();

            // Remove item from UI
            _FileFilterItemSource.Remove(selectedValue);
            TextBoxAddFilter.Text = "";
        }
    }
}