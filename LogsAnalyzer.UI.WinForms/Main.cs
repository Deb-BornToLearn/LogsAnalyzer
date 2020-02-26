using LogAnalyzer.Infrastructure;
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
using System.Xml.Serialization;

namespace LogAnalyzer.UI.WinForms {
    public partial class Main : Form {

        private BaseLogSourceListController<TreeView> _logSourceListController;
        private BaseLogAnalyzerListController<TreeView> _logAnalyzerListController;
        private TreeViewAfterCheckController _treeviewAfterCheckController;

        protected string AnalyzersConfigFile { get; set; }
        public Main() {
            InitializeComponent();
            _logSourceListController = new LogSourceTreeViewController<TreeView>(logFilesList);
            _logAnalyzerListController = new LogAnalyzerListTreeViewController<TreeView>(analyzersList);
            _treeviewAfterCheckController = new TreeViewAfterCheckController();

            try {
                loadAnalyzersToList();
            }
            catch (Exception exc) {
                MessageBox.Show(exc.Message, "Error Loading Form", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            AnalyzersConfigFile = configFile;
            return new AnalyzerConfigurationXmlSource(xmlDoc);
        }

        private void addLogFileMenuItem_Click(object sender, EventArgs e) {
            var openFileDialog = new OpenFileDialog {
                Multiselect = true,
                FilterIndex = 2
            };

            openFileDialog.Filter = "All files (*.*)|*.*|Log files (*.log)|*.log*|Text files (*.txt)|*.txt";
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK) {
                foreach (var filename in openFileDialog.FileNames) {
                    _logSourceListController.AddFile(filename);

                }
                displayStartAnalysisPrompt();
            }
        }

        private void displayStartAnalysisPrompt() {
            if (MessageBox.Show("Start logs analysis now?", "Ready to start analysis",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                runAnalysis();
            }
        }

        private void logFilesListMenu_Opened(object sender, EventArgs e) {
            var hasAny = _logSourceListController.HasAny();
            var hasSelectedAny = _logSourceListController.HasSelectedAny();
            var isSelectedItemFolder = _logSourceListController.IsSelectedItemFolder();

            removeLogFileMenuItem.Enabled = hasSelectedAny;
            removeAllLogsMenuItem.Enabled = hasAny;
            checkFolderOnlyMenuItem.Enabled = isSelectedItemFolder;
            saveSelectedCollectionFileMenuItem.Enabled = hasSelectedAny;
            saveAllToCollectionMenuItem.Enabled = hasAny;
        }

        private void removeLogFileMenuItem_Click(object sender, EventArgs e) {
            _logSourceListController.RemoveSelectedItems();
        }

        private void analyzeButton_Click(object sender, EventArgs e) {
            runAnalysis();
        }

        private void runAnalysis() {
            if (!_logSourceListController.HasSelectedAny()) {
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
            var logSources = _logSourceListController.BuildLogSourceDefinitionFromSelection();
            var resultsForm = new AnalysisResultsForm(analysisArgs, logSources, AnalyzersConfigFile);
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

        private void addFolderMenuItem_Click(object sender, EventArgs e) {
            var selectFolderDialog = createOpenFolderDialog();
            if (selectFolderDialog.ShowDialog() == DialogResult.OK) {
                var folder = Path.GetDirectoryName(selectFolderDialog.FileName);
                _logSourceListController.AddFolder(folder, true);
                displayStartAnalysisPrompt();
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
        private void saveSelectedCollectionFileMenuItem_Click(object sender, EventArgs e) {
            createLogDefinitionFile(_logSourceListController.BuildLogSourceDefinitionFromSelection);
        }

        private void saveAllToCollectionMenuItem_Click(object sender, EventArgs e) {
            createLogDefinitionFile(_logSourceListController.BuildLogSourceDefinition);
        }

        private void createLogDefinitionFile(Func<LogSourceDefinition> buildLogSourceDefinitionFunc) {
            var openFileDialog = new OpenFileDialog {
                CheckFileExists = false,
                FileName = "New Log Definition.xml"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                LogSourceDefinition logSourceDefinition = buildLogSourceDefinitionFunc();
                serializeLogSourceDefinition(logSourceDefinition, openFileDialog.FileName);
            }
        }

        private void serializeLogSourceDefinition(LogSourceDefinition logSourceDefinition, string filename) {
            XmlSerializer serializer = new XmlSerializer(typeof(LogSourceDefinition));
            StreamWriter writer = new StreamWriter(filename);
            serializer.Serialize(writer, logSourceDefinition);
            writer.Close();
        }

        private void addLogsCollectionFileMenuItem_Click(object sender, EventArgs e) {
            var openFileDialog = new OpenFileDialog {
                Filter = "All files (*.*)|*.*|Log definition files (*.xml)|*.xml",
                FilterIndex = 2
            };
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK) {
                try {
                    var serializer = new XmlSerializer(typeof(LogSourceDefinition));
                    using (var fileStream = new FileStream(openFileDialog.FileName, FileMode.Open)) {
                        LogSourceDefinition logDefinition = null;
                        logDefinition = (LogSourceDefinition)serializer.Deserialize(fileStream);
                        _logSourceListController.AddLogSourceDefinition(logDefinition);
                        displayStartAnalysisPrompt();
                    }
                }
                catch (Exception exc) {
                    MessageBox.Show($"File {openFileDialog.FileName} is not a valid log source definition file. {exc.Message}.",
                                     "Invalid log source definition file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
        }

        private void checkFolderOnlyMenuItem_Click(object sender, EventArgs e) {
            if (logFilesList.SelectedNode != null) {
                logFilesList.SelectedNode.Checked = true;
                foreach (TreeNode child in logFilesList.SelectedNode.Nodes) {
                    child.Checked = false;
                }
            }
        }
    }
}

