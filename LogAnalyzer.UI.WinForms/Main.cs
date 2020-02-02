using LogAnalyzer.Infrastructure;
using LogAnalyzer.Infrastructure.Analysis;
using LogAnalyzer.Infrastructure.Configuration;
using LogAnalyzer.Infrastructure.Factory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LogAnalyzer.UI.WinForms {
    public partial class Main : Form {
        public Main() {
            InitializeComponent();
            loadAnalyzersToList();
            logFilesList.Items.Add(@"C:\JABE\JABELabs\LogAnalyzer\v3.rezobxplus.webservices\reservation.log");
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
            resultsTextbox.Text = string.Empty;

            if (logFilesList.Items.Count == 0) {
                MessageBox.Show("Please select one or more log files to analyze");
                return;
            }

            if (analyzersList.CheckedItems.Count == 0) {
                MessageBox.Show("Please check one or more analyzers to run");
                return;
            }

            List<BaseLogAnalyzer> analyzers = loadAnalyzers();

            var logReader = new LogReader(analyzers);

            foreach (string file in logFilesList.Items) {
                using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
                    logReader.ReadSource(file, stream);
                }
            }

            foreach (var analyzer in analyzers) {
                resultsTextbox.AppendText(analyzer.AnalysesToString());
            }
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

        private void wordWrapResults_CheckedChanged(object sender, EventArgs e) {
            resultsTextbox.WordWrap = wordWrapResults.Checked;
        }
    }
}
