using LogAnalyzer.Infrastructure;
using LogAnalyzer.Infrastructure.Analysis;
using LogAnalyzer.UI.WinForms.Controllers;
using LogsAnalyzer.Infrastructure;
using LogsAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Infrastructure.Factory;
using LogsAnalyzer.Renderers.WinForms.Factory;
using LogsAnalyzer.Renderers.WinForms.TreeView;
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
        delegate void SetTextCallback(TextBoxBase textbox, string message);
        delegate void ExpandNodesCallback(TreeView treeView);
        delegate void AddNodeCallback(TreeView treeview, TreeNode parentNode, TreeNode node);
        #endregion

        private AutoResetEvent _formShownEventDone = new AutoResetEvent(false);

        private TreeViewFilterer _treeViewFilterer;

        private Dictionary<Type, BaseTreeViewRenderer> _rendererList = new Dictionary<Type, BaseTreeViewRenderer>();

        public AnalysisResultsForm(AnalysisArgs analysisArgs, LogSourceDefinition logSources, string renderersConfigFile) {
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

            var renderersBuilder = new RenderersBuilder(renderersConfigFile);
            var renderers = renderersBuilder.BuildRenderers();
            foreach (var r in renderers) {
                _rendererList.Add(r.AnalyzerType, r);
            }

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
            setEnabled(tabControl1, isFormReady);
            setEnabled(filterTextBox, isFormReady);
            setEnabled(filterButton, isFormReady);
            setEnabled(clearFilterButton, isFormReady);
            setEnabled(resultsTreeView, isFormReady);
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

        private void setText(TextBoxBase textbox, string text) {
            if (textbox.InvokeRequired) {
                SetTextCallback cb = new SetTextCallback(setText);
                this.Invoke(cb, new object[] { textbox, text });
            }
            else {
                textbox.Text = text;
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

            try {
                runAnalyzers();
                renderResults();
                expandNodes(resultsTreeView);
            }
            catch(Exception exc) {
                MessageBox.Show($"Error occurred while analyzing logs or rendering analysis results: {Environment.NewLine}{exc.Message}", "Error", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            setText(filterTextBox, string.Empty);

            FormState = FormStateEnum.Ready;
        }

        private void runAnalyzers() {
            var logReader = new LogReader(Analyzers, AnalyzerChains);
            logReader.OnReadProgress += LogReader_OnReadProgress;
            int counter = 1, total = LogSources.SourceFiles.Count;
            foreach (string file in LogSources.SourceFiles) {
                try {
                    setText(filterTextBox, $"Reading logs from {file} ({counter}/{total}) ...");
                    using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
                        logReader.ReadSource(file, stream);
                    }
                }
                catch (Exception exc) {
                    setText(filterTextBox, exc.Message);
                }
                counter++;
            }
        }

        private List<Type> _analyzersWithoutRenderers = new List<Type>();

        private void renderResults() {
            _analyzersWithoutRenderers.Clear();

            Analyzers.ForEach(a => renderAnalysisResults(a));
            AnalyzerChains.ForEach(ac => ac.Analyzers.ForEach(a => renderAnalysisResults(a)));

            if (_analyzersWithoutRenderers.Any()) {
                MessageBox.Show("Results for the following analyzer(s) will not be shown because of missing TreeViewRenderer implementation: " +
                                Environment.NewLine +
                                string.Join(Environment.NewLine, _analyzersWithoutRenderers.Select(a => a.ToString())
                                                                                           .Select(a => "- " + a.Substring(a.LastIndexOf('.') + 1))
                                                                                           .ToArray()),
                                "Missing Analyzer TreeView Renderer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void renderAnalysisResults(BaseLogAnalyzer analyzer) {
            if (_rendererList.ContainsKey(analyzer.GetType())) {
                renderAnalysisResults(_rendererList[analyzer.GetType()], analyzer);
            }
            else {
                if (_rendererList.ContainsKey(typeof(BaseLogAnalyzer))) {
                    renderAnalysisResults(_rendererList[typeof(BaseLogAnalyzer)], analyzer);
                }
                else { 
                    _analyzersWithoutRenderers.Add(analyzer.GetType());
                }
            }
        }

        private void renderAnalysisResults(BaseTreeViewRenderer renderer, BaseLogAnalyzer analyzer) {
            renderer.SetAnalyzer(analyzer);
            var node = renderer.Render();
            foreach (var nodeKey in renderer.ContextMenuStrips.Keys) {
                nodeKey.ContextMenuStrip = renderer.ContextMenuStrips[nodeKey];
            }
            addNode(resultsTreeView, null, node);
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
                TreeNode firstNode = null;
                foreach (TreeNode node in treeview.Nodes) {
                    if (firstNode == null) firstNode = node;
                    node.Expand();
                }
                if (firstNode != null) {
                    treeview.SelectedNode = firstNode;
                    firstNode.EnsureVisible();
                }
            }
        }

        private void LogReader_OnReadProgress(LogReader reader, ReadProgressEventArgs args) {
            if (args.LineNumber % 10000 == 0) {
                setText(filterTextBox, $"Analyzing line {args.LineNumber} of {args.SourceName} ...{Environment.NewLine}");
            }
        }

        private void closeButton_Click(object sender, EventArgs e) {
            Close();
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
            applyFilter();
        }

        private void clearFilterButton_Click(object sender, EventArgs e) {
            clearFilter();
        }

        private void applyFilter() {
            if (_treeViewFilterer == null) _treeViewFilterer = new TreeViewFilterer(resultsTreeView);

            _treeViewFilterer.Filter(filterTextBox.Text);
            if (!string.IsNullOrEmpty(filterTextBox.Text)) {
                resultsTreeView.ExpandAll();
            }
        }

        private void filterTextBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                applyFilter();
                return;
            }
            if (e.KeyCode == Keys.Escape) {
                clearFilter();
                return;
            }
        }

        private void clearFilter() {
            filterTextBox.Clear();
            applyFilter();
        }

    }
}
