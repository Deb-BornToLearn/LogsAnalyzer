using LogsAnalyzer.Infrastructure;
using LogsAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Infrastructure.Configuration;
using LogsAnalyzer.Infrastructure.Factory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace LogAnalyzer.UI.WinForms {
    public partial class AnalysisResultsForm : Form {

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
            AnalyzeLogs();
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
            var logReader = new LogReader(Analyzers);
            foreach (string file in LogFiles) {
                try {
                    using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
                        logReader.ReadSource(file, stream);
                    }
                }
                catch (Exception exc) {
                    MessageBox.Show(exc.Message);
                }
            }

            foreach (var analyzer in Analyzers) {
                resultsTextbox.AppendText(analyzer.AnalysesToString());
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
    }
}
