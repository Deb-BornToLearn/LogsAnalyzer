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
                analyzersList.Items.Add(analyzerConfig, true);
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
            if (logFilesList.Items.Count == 0) {
                MessageBox.Show("Please select one or more log files to analyze by right-clicking on Log files list",
                                "Select log file(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (analyzersList.CheckedItems.Count == 0) {
                MessageBox.Show("Please check one or more analyzers to run");
                return;
            }

            List<AnalyzerConfiguration> analyzerConfigurations = getSelectedAnalyzers();
            List<string> logFiles = getLogFileNames();
            var resultsForm = new AnalysisResultsForm(analyzerConfigurations, logFiles);
            resultsForm.Show();
        }

        private List<AnalyzerConfiguration> getSelectedAnalyzers() {
            List<AnalyzerConfiguration> selectedAnalyzers = new List<AnalyzerConfiguration>();
            foreach (var item in analyzersList.CheckedItems) {
                selectedAnalyzers.Add(item as AnalyzerConfiguration);
            }
            return selectedAnalyzers;
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
            foreach (AnalyzerConfiguration config in analyzersList.CheckedItems) {
                analyzerConfigs.Add(config);
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
