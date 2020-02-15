using LogAnalyzer.UI.WinForms.Controllers;
using LogsAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Infrastructure.Configuration;
using LogsAnalyzer.Infrastructure.Factory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LogAnalyzer.UI.WinForms {
    public partial class Main : Form {

        private BaseLogSourceListController<TreeView> _logSourceListController;
        private BaseLogAnalyzerListController<TreeView> _logAnalyzerListController;
        private TreeViewAfterCheckController _treeviewAfterCheckController;

        public Main() {
            InitializeComponent();
            _logSourceListController = new LogSourceTreeViewController<TreeView>(logFilesList);
            _logAnalyzerListController = new LogAnalyzerListTreeViewController<TreeView>(analyzersList);
            _treeviewAfterCheckController = new TreeViewAfterCheckController();

            loadAnalyzersToList();
        }

        private void loadAnalyzersToList() {
            var analyzersConfigurationSource = loadConfigurationSource();
            var analyzerConfigs = analyzersConfigurationSource.GetAnalyzerConfigurations();
            _logAnalyzerListController.AddAnalyzers(analyzerConfigs);

            var analyzerChainConfigs = analyzersConfigurationSource.GetAnalyzerChainConfigurations();
            _logAnalyzerListController.AddAnalyzerChains(analyzerChainConfigs);
        }

        private IConfigurationSource loadConfigurationSource() {
            var configFile = Path.Combine(Application.StartupPath, "LogAnalyzer.config");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFile);
            return new AnalyzerConfigurationXmlSource(xmlDoc);
        }

        private void addLogFileMenuItem_Click(object sender, EventArgs e) {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;

            openFileDialog.Filter = "All files (*.*)|*.*|Log files (*.log)|*.log|Text files (*.txt)|*.txt";
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK) {
                foreach (var filename in openFileDialog.FileNames) {
                    _logSourceListController.AddFile(filename);

                }
                if (MessageBox.Show("Start logs analysis now?", "Ready to start analysis",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    runAnalysis();
                }
            }

        }

        private void logFilesListMenu_Opened(object sender, EventArgs e) {
            var hasFile = _logSourceListController.HasFile();
            var hasSelectedFile = _logSourceListController.HasSelectedFile();

            removeLogFileMenuItem.Enabled = hasSelectedFile;
            removeAllLogsMenuItem.Enabled = hasFile;
            saveSelectedCollectionFileMenuItem.Enabled = hasSelectedFile;
            saveAllToCollectionMenuItem.Enabled = hasFile;
        }

        private void removeLogFileMenuItem_Click(object sender, EventArgs e) {
            _logSourceListController.RemoveSelectedItems();
        }

        private void analyzeButton_Click(object sender, EventArgs e) {
            runAnalysis();
        }

        private void runAnalysis() {
            if (!_logSourceListController.HasSelectedFile()) {
                MessageBox.Show("Please check one or more log files to analyze, or add files/folders by right-clicking on Log files list",
                                "No log file to analyze", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_logAnalyzerListController.IsAnyAnalyzerSelected()) {
                MessageBox.Show("Please check one or more analyzers to run",
                                "Select analyzer(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AnalysisArgs analysisArgs = _logAnalyzerListController.BuildAnalysisArgs();
            var logSources = _logSourceListController.GetSelectedLogSources();
            var resultsForm = new AnalysisResultsForm(analysisArgs, logSources);
            resultsForm.Show();
        }


        private List<BaseLogAnalyzer> loadAnalyzers() {
            var analyzerConfigs = _logAnalyzerListController.GetSelectedAnalyzerConfigurations();
            if (!analyzerConfigs.Any()) {
                MessageBox.Show("Please select one or more log analyzers to run");
                return null;
            }

            var analyzerBuilder = new AnalyzersBuilder(analyzerConfigs);
            return analyzerBuilder.BuildAnalyzers();
        }

        private void removeAllLogsMenuItem_Click(object sender, EventArgs e) {
            _logSourceListController.RemoveAllItems();
        }

        private void saveSelectedCollectionFileMenuItem_Click(object sender, EventArgs e) {
            //foreach (string selectedFile in logFilesList.SelectedItems) { 

            //}
        }

        private void addFolderMenuItem_Click(object sender, EventArgs e) {
            var selectFolderDialog = createOpenFolderDialog();
            if (selectFolderDialog.ShowDialog() == DialogResult.OK) {
                var folder = Path.GetDirectoryName(selectFolderDialog.FileName);
                _logSourceListController.AddFolder(folder, true);
            }
        }

        private OpenFileDialog createOpenFolderDialog() {
            return new OpenFileDialog {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection."
            };
        }

        private void logFilesList_AfterCheck(object sender, TreeViewEventArgs e) {
            _treeviewAfterCheckController.OnAfterCheck(sender, e);
        }

        private void analyzersList_AfterCheck(object sender, TreeViewEventArgs e) {
            _treeviewAfterCheckController.OnAfterCheck(sender, e);
        }
    }
}

