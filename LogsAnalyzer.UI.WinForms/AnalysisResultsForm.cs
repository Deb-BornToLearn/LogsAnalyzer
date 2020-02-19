using LogAnalyzer.Analyzers.Bookings;
using LogAnalyzer.Analyzers.Bookings.Models;
using LogAnalyzer.Analyzers.Errors;
using LogAnalyzer.Analyzers.Errors.Database;
using LogAnalyzer.Analyzers.Errors.Smtp;
using LogAnalyzer.Infrastructure;
using LogAnalyzer.Infrastructure.Analysis;
using LogAnalyzer.UI.WinForms.Controllers;
using LogsAnalyzer.Infrastructure;
using LogsAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Infrastructure.Factory;
using LogsAnalyzer.Renderers.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace LogAnalyzer.UI.WinForms {
    public partial class AnalysisResultsForm : Form {
        public enum FormStateEnum {
            Ready,
            AnalyzingLogsInProgress
        }

        private BaseLogSourceListController<TreeView> _logSourceListController;
        private BaseLogAnalyzerListController<TreeView> _logAnalyzerListController;

        public List<BaseLogAnalyzer> Analyzers;
        public List<AnalyzerShortCircuitChain> AnalyzerChains;

        public readonly AnalysisArgs AnalysisArgs;
        public readonly LogSourceDefinition LogSources;

        #region Cross-thread callback delegates
        delegate void enableControlCallback(Control control, bool enabled);
        delegate void AppendTextCallback(TextBoxBase textbox, string message);
        delegate void SetTextCallback(TextBoxBase textbox, string message);
        delegate void ScrollToTopCallback(TextBoxBase textbox);
        delegate void ExpandNodesCallback(TreeView treeView);
        delegate void AddNodeCallback(TreeView treeview, TreeNode parentNode, TreeNode node);
        #endregion

        private AutoResetEvent _formShownEventDone = new AutoResetEvent(false);

        private Dictionary<Type, Action<BaseLogAnalyzer, TreeView>> _renderers =
                                      new Dictionary<Type, Action<BaseLogAnalyzer, TreeView>>();

        public AnalysisResultsForm(AnalysisArgs analysisArgs, LogSourceDefinition logSources) {
            InitializeComponent();

            _logSourceListController = new LogSourceTreeViewController<TreeView>(logFilesList);
            _logAnalyzerListController = new LogAnalyzerListTreeViewController<TreeView>(analyzersList);

            AnalysisArgs = analysisArgs;
            LogSources = logSources;

            Analyzers = buildAnalyzers(analysisArgs);
            AnalyzerChains = buildAnalyzerChains(analysisArgs);

            populateLists();
            formCaptionTextbox.Text = this.Text;
            FormState = FormStateEnum.Ready;

            _renderers.Add(typeof(BookingAnalyzer), renderBookings);
            _renderers.Add(typeof(SmtpErrorAnalyzer), renderSmtpError);
            _renderers.Add(typeof(UnreachableServerErrorAnalyzer), renderUnreachableDbError);

            runAnalysisThread();
        }



        private void runAnalysisThread() {
            ThreadStart work = new ThreadStart(() => AnalyzeLogs());
            var thread = new Thread(work);
            thread.Start();
        }

        private List<AnalyzerShortCircuitChain> buildAnalyzerChains(AnalysisArgs analysisArgs) {
            var analyzerChains = new List<AnalyzerShortCircuitChain>();
            foreach (var chainConfig in analysisArgs.AnalyzerChainConfigurations) {
                var analyzerChain = new AnalyzerShortCircuitChain(chainConfig.DisplayName);
                var analyzerChainBuilder = new AnalyzersBuilder(chainConfig.AnalyzerConfigurations.Where(ac => ac.Enabled).ToList());
                analyzerChain.Analyzers.AddRange(analyzerChainBuilder.BuildAnalyzers());
                analyzerChains.Add(analyzerChain);
            }
            return analyzerChains;
        }

        private List<BaseLogAnalyzer> buildAnalyzers(AnalysisArgs analysisArgs) {
            var analyzerBuilder = new AnalyzersBuilder(analysisArgs.AnalyzerConfigurations);
            return analyzerBuilder.BuildAnalyzers();
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
            _logAnalyzerListController.AddAnalyzers(AnalysisArgs.AnalyzerConfigurations);
            _logAnalyzerListController.AddAnalyzerChains(AnalysisArgs.AnalyzerChainConfigurations);

            LogSources.SourceFiles.ForEach(f => _logSourceListController.AddFile(f));
            addSourceFoldersWithoutGettingFilesFromFileSystem();
        }

        private void addSourceFoldersWithoutGettingFilesFromFileSystem() {
            LogSources.SourceFolders.ForEach(f => _logSourceListController.AddFolder(f, false));
        }


        private void AnalyzeLogs() {
            // Wait for form to finish initialization before firing off worker thread;
            // otherwise, intermittent cross-thread errors will crop up.
            _formShownEventDone.WaitOne();

            FormState = FormStateEnum.AnalyzingLogsInProgress;

            var logReader = new LogReader(Analyzers, AnalyzerChains);
            logReader.OnReadProgress += LogReader_OnReadProgress;
            int counter = 1, total = LogSources.SourceFiles.Count;
            foreach (string file in LogSources.SourceFiles) {
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

            Analyzers.ForEach(a => appendText(resultsTextbox, $"{a.AnalysesToString()}{Environment.NewLine}"));
            AnalyzerChains.ForEach(c => c.Analyzers.ForEach(a => appendText(resultsTextbox, $"{a.AnalysesToString()}{Environment.NewLine}")));

            scrollToTop(resultsTextbox);

            foreach (var analyzer in Analyzers) {
                if (_renderers.ContainsKey(analyzer.GetType())) {
                    _renderers[analyzer.GetType()](analyzer, resultsTreeView);
                }
            }
            foreach (var chain in AnalyzerChains) {
                foreach (var analyzer in chain.Analyzers) {
                    if (_renderers.ContainsKey(analyzer.GetType())) {
                        _renderers[analyzer.GetType()](analyzer, resultsTreeView);
                    }
                }
            }
            expandNodes(resultsTreeView);
            FormState = FormStateEnum.Ready;
        }
        private void renderSmtpError(BaseLogAnalyzer analyzer, TreeView treeView) {
            var renderer = new SmtpErrorTreeViewRenderer();
            renderer.SetAnalyzer(analyzer);
            addNode(treeView, null, renderer.Render());
        }

        private void renderBookings(BaseLogAnalyzer analyzer, TreeView treeView) {
            var renderer = new BookingAnalysisTreeViewRenderer();
            renderer.SetAnalyzer(analyzer);
            addNode(treeView, null, renderer.Render());
        }
        private void renderUnreachableDbError(BaseLogAnalyzer analyzer, TreeView treeView) {
            var renderer = new UnreachableDbServerErrorTreeViewRenderer();
            renderer.SetAnalyzer(analyzer);
            addNode(treeView, null, renderer.Render());
        }

        private void addNode(TreeView treeview, TreeNode parentNode, TreeNode node) {
            if (treeview.InvokeRequired) {
                var cb = new AddNodeCallback(addNode);
                Invoke(cb, new object[] { treeview, parentNode, node });
            }
            else {
                if (parentNode == null) {
                    treeview.Nodes.Add(node);
                }
                else {
                    parentNode.Nodes.Add(node);
                }
            }
        }
        private void expandNodes(TreeView treeview) {
            if (treeview.InvokeRequired) {
                var cb = new ExpandNodesCallback(expandNodes);
                Invoke(cb, new object[] { treeview });
            }
            else {
                treeview.ExpandAll();
            }
        }

        private void LogReader_OnReadProgress(LogReader reader, ReadProgressEventArgs args) {
            if (args.LineNumber % 1000 == 0) {
                appendText(resultsTextbox, $"Analyzing line {args.LineNumber} ...{Environment.NewLine}");
            }
        }

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
            openContainingFolderCommand.Enabled = logFilesList.SelectedNode != null;
        }

        private void openContainingFolderCommand_Click(object sender, EventArgs e) {
            //_logSourceListController.GetSelectedText() ???
            if (logFilesList.SelectedNode != null) {
                var folder = Path.GetDirectoryName(logFilesList.SelectedNode.Text);
                Process.Start("explorer.exe", folder);
            }
        }

        private void AnalysisResultsForm_Shown(object sender, EventArgs e) {
            _formShownEventDone.Set();
        }

        private void filterButton_Click(object sender, EventArgs e) {

        }
    }
}
