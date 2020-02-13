using LogAnalyzer.Infrastructure.Configuration;
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
        public Main() {
            InitializeComponent();
            loadAnalyzersToList();
        }

        private void loadAnalyzersToList() {
            var configFile = Path.Combine(Application.StartupPath, "LogAnalyzer.config");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFile);
            var analyzersConfigurationSource = new AnalyzerConfigurationXmlSource(xmlDoc);
            var analyzerConfigs = analyzersConfigurationSource.GetAnalyzerConfigurations();
            foreach (var analyzerConfig in analyzerConfigs) {
                var node = new TreeNode(analyzerConfig.DisplayName);
                node.Tag = analyzerConfig;
                node.Checked = true;
                analyzersList.Nodes.Add(node);
            }

            var analyzerChainConfigs = analyzersConfigurationSource.GetAnalyzerChainConfigurations();
            foreach (var analyzerChainConfig in analyzerChainConfigs) {
                TreeNode node = new TreeNode(analyzerChainConfig.DisplayName);
                node.Tag = analyzerChainConfig;
                node.Checked = true;
                node.Expand();
                foreach (var analyzerConfig in analyzerChainConfig.AnalyzerConfigurations) {
                    var subNode = new TreeNode(analyzerConfig.DisplayName);
                    subNode.Tag = analyzerConfig;
                    subNode.Checked = true;
                    node.Nodes.Add(subNode);
                }
                analyzersList.Nodes.Add(node);
            }
        }

        private void addLogFileMenuItem_Click(object sender, EventArgs e) {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;

            openFileDialog.Filter = "All files (*.*)|*.*|Log files (*.log)|*.log|Text files (*.txt)|*.txt";
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK) {
                foreach (var filename in openFileDialog.FileNames) {
                    if (!logFilesList.Items.Contains(filename)) {
                        logFilesList.Items.Add(filename);
                    }
                }
                if (MessageBox.Show("Start logs analysis now?", "Ready to start analysis", 
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    runAnalysis();
                }
            }

        }

        private void logFilesListMenu_Opened(object sender, EventArgs e) {
            removeLogFileMenuItem.Enabled = logFilesList.SelectedIndices.Count > 0;
        }

        private void removeLogFileMenuItem_Click(object sender, EventArgs e) {
            var itemList = new List<string>();
            foreach (string item in logFilesList.SelectedItems) {
                itemList.Add(item);
            }
            foreach (var item in itemList) {
                logFilesList.Items.Remove(item);
            }
        }

        private void analyzeButton_Click(object sender, EventArgs e) {
            runAnalysis();
        }

        private void runAnalysis() {
            if (logFilesList.Items.Count == 0) {
                MessageBox.Show("Please select one or more log files to analyze by right-clicking on Log files list",
                                "Select log file(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!isAnyAnalyzerSelected(analyzersList)) {
                MessageBox.Show("Please check one or more analyzers to run",
                                "Select analyzer(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AnalysisArgs analyzerConfigurations = getSelectedAnalyzers();
            List<string> logFiles = getLogFileNames();
            var resultsForm = new AnalysisResultsForm(analyzerConfigurations, logFiles);
            resultsForm.Show();
        }

        private bool isAnyAnalyzerSelected(TreeView tvw) {
            foreach (TreeNode node in tvw.Nodes) {
                if (node.Checked) return true;
            }
            return false;
        }

        private AnalysisArgs getSelectedAnalyzers() {
            var analysisArgs = new AnalysisArgs();
            List<AnalyzerConfiguration> selectedAnalyzers = new List<AnalyzerConfiguration>();
            foreach (TreeNode item in analyzersList.Nodes) {
                if (item.Checked) {
                    if (item.Tag.GetType() == typeof(AnalyzerConfiguration)) {
                        analysisArgs.AnalyzerConfigurations.Add(item.Tag as AnalyzerConfiguration);
                    }
                    else if (item.Tag.GetType() == typeof(AnalyzerChainConfiguration)) {
                        var analyzerChainConfig = (AnalyzerChainConfiguration)item.Tag;
                        foreach (TreeNode subNode in item.Nodes) {
                            var analyzerConfig = (AnalyzerConfiguration)subNode.Tag;
                            analyzerConfig.Enabled = subNode.Checked;
                        }
                        analysisArgs.AnalyzerChainConfigurations.Add(analyzerChainConfig);
                    }
                }
            }
            return analysisArgs;
        }

        private List<string> getLogFileNames() {
            List<string> logFiles = new List<string>();
            foreach (string file in logFilesList.Items) {
                logFiles.Add(file);
            }
            return logFiles;
        }

        private List<BaseLogAnalyzer> loadAnalyzers() {
            var analyzerConfigs = new List<AnalyzerConfiguration>();
            foreach (TreeNode node in analyzersList.Nodes) {
                if (node.Checked) { 
                   analyzerConfigs.Add((AnalyzerConfiguration)node.Tag);
                }
            }
            if (!analyzerConfigs.Any()) {
                MessageBox.Show("Please select one or more log analyzers to run");
                return null;
            }

            var analyzerBuilder = new AnalyzersBuilder(analyzerConfigs);

            return analyzerBuilder.BuildAnalyzers();
        }

        private void removeAllLogsMenuItem_Click(object sender, EventArgs e) {
            var itemList = new List<string>();
            foreach (string item in logFilesList.Items) {
                itemList.Add(item);
            }
            foreach (var item in itemList) {
                logFilesList.Items.Remove(item);
            }
        }

    }
}
