using LogsAnalyzer.Infrastructure;
using LogsAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Infrastructure.Configuration;
using LogsAnalyzer.Infrastructure.Factory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace LogAnalyzer.UI.WinForms {
    public partial class AnalysisResultsForm : Form {
        // TODO: Add filtering by selected analyzer(s)
        // TODO: Add filtering by selected log file(s)
        // TODO: Create AnalysisResultsPresenter
        public enum FormStateEnum {
            Ready,
            AnalyzingLogsInProgress
        }

        public List<BaseLogAnalyzer> Analyzers;
        public readonly List<AnalyzerConfiguration> AnalyzerConfigurations;
        public readonly List<string> LogFiles;
        public AnalysisResultsForm(List<AnalyzerConfiguration> analyzerConfigurations, List<string> logFiles) {
            InitializeComponent();
            AnalyzerConfigurations = analyzerConfigurations;
            var analyzerBuilder = new AnalyzersBuilder(AnalyzerConfigurations);
            Analyzers = analyzerBuilder.BuildAnalyzers();
            LogFiles = logFiles;

            formCaptionTextbox.Text = this.Text;
            populateLists();

            FormState = FormStateEnum.Ready;

            ThreadStart work = new ThreadStart(() => AnalyzeLogs());
            var thread = new Thread(work);
            thread.Start();

        }

        private FormStateEnum _formState;
        public FormStateEnum FormState {
            get {
                return _formState;
            }
            protected set {
                onWillFormStateChange(value);
                _formState = value;
                onDidFormStateChanged();
            }
        }

        private void onWillFormStateChange(FormStateEnum incomingState) {

        }

        private void onDidFormStateChanged() {
            var isFormReady = FormState == FormStateEnum.Ready;
            setEnabled(closeButton, isFormReady);
            setEnabled(wordWrapCheckbox, isFormReady);
            setEnabled(tabControl1, isFormReady);
        }

        delegate void enableControlCallback(Control control, bool enabled);
        private void setEnabled(Control control, bool enabled) {
            if (control.InvokeRequired) {
                var cb = new enableControlCallback(setEnabled);
                Invoke(cb, control, enabled);
            }
            else {
                control.Enabled = enabled;
            }
        }

        private void populateLists() {
            foreach (var analyzer in AnalyzerConfigurations) {
                analyzersList.Items.Add(analyzer, true);
            }
            foreach (var file in LogFiles) {
                logFilesList.Items.Add(file, true);
            }
        }

        private void AnalyzeLogs() {
            FormState = FormStateEnum.AnalyzingLogsInProgress;

            var logReader = new LogReader(Analyzers);
            int counter = 1, total = LogFiles.Count;
            foreach (string file in LogFiles) {
                try {
                    appendText(resultsTextbox, $"Reading logs from {file} ({counter}/{total}) ...{Environment.NewLine}");
                    using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
                        logReader.ReadSource(file, stream);
                    }
                }
                catch (Exception exc) {
                    appendText(resultsTextbox, exc.Message);
                }
                counter++;
            }

            setText(resultsTextbox, string.Empty);

            foreach (var analyzer in Analyzers) {
                appendText(resultsTextbox, analyzer.AnalysesToString());
            }

            scrollToTop(resultsTextbox);

            FormState = FormStateEnum.Ready;
        }

        delegate void AppendTextCallback(TextBoxBase textbox, string message);
        delegate void SetTextCallback(TextBoxBase textbox, string message);
        delegate void ScrollToTopCallback(TextBoxBase textbox);

        private void scrollToTop(TextBoxBase textbox) {
            if (textbox.InvokeRequired) {
                var cb = new ScrollToTopCallback(scrollToTop);
                Invoke(cb, new object[] { textbox });
            }
            else {
                textbox.SelectionStart = 0;
                textbox.ScrollToCaret();
            }
        }

        private void appendText(TextBoxBase textbox, string text) {
            if (textbox.InvokeRequired) {
                AppendTextCallback cb = new AppendTextCallback(appendText);
                Invoke(cb, new object[] { textbox, text });
            }
            else {
                resultsTextbox.AppendText(text);
            }
        }

        private void setText(TextBoxBase textbox, string text) {
            if (textbox.InvokeRequired) {
                AppendTextCallback cb = new AppendTextCallback(setText);
                this.Invoke(cb, new object[] { textbox, text });
            }
            else {
                resultsTextbox.Text = text;
            }
        }

        private void closeButton_Click(object sender, EventArgs e) {
            Close();
        }

        private void wordWrapCheckbox_CheckedChanged(object sender, EventArgs e) {
            resultsTextbox.WordWrap = wordWrapCheckbox.Checked;
        }

        private void setFormCaptionButton_Click(object sender, EventArgs e) {
            Text = formCaptionTextbox.Text;
        }

        private void logFileListContextMenu_Opened(object sender, EventArgs e) {
            openContainingFolderCommand.Enabled = logFilesList.SelectedIndices.Count > 0;
        }

        private void openContainingFolderCommand_Click(object sender, EventArgs e) {
            foreach (var item in logFilesList.SelectedItems) {
                var folder = Path.GetDirectoryName(item as string);
                Process.Start("explorer.exe", folder);
            }
        }
    }
}
