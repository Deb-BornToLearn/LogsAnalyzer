using System.Collections.Generic;
using System.Windows.Forms;
using LogAnalyzer.Infrastructure.Configuration;
using LogsAnalyzer.Infrastructure.Configuration;

namespace LogAnalyzer.UI.WinForms.Controllers {
    public class LogAnalyzerListTreeViewController<T> : BaseLogAnalyzerListController<TreeView> {
        public LogAnalyzerListTreeViewController(TreeView listView) : base(listView) {
        }

        internal override void AddAnalyzers(List<AnalyzerConfiguration> analyzerConfigs) {
            foreach (var analyzerConfig in analyzerConfigs) {
                var node = new TreeNode(analyzerConfig.DisplayName);
                node.Tag = analyzerConfig;
                node.Checked = true;
                ListView.Nodes.Add(node);
            }
        }

        internal override void AddAnalyzerChains(List<AnalyzerChainConfiguration> analyzerChainConfigs) {
            foreach (var analyzerChainConfig in analyzerChainConfigs) {
                TreeNode node = new TreeNode(analyzerChainConfig.DisplayName);
                node.Tag = analyzerChainConfig;
                node.Checked = true;
                node.Expand();
                foreach (var analyzerConfig in analyzerChainConfig.AnalyzerConfigurations) {
                    var subNode = new TreeNode(analyzerConfig.DisplayName);
                    subNode.Tag = analyzerConfig;
                    subNode.Checked = analyzerConfig.Enabled;
                    node.Nodes.Add(subNode);
                }
                ListView.Nodes.Add(node);
            }
        }

        internal override bool IsAnyAnalyzerSelected() {
            foreach (TreeNode node in ListView.Nodes) {
                if (node.Checked) return true;
            }
            return false;
        }

        internal override AnalysisArgs BuildAnalysisArgs() {
            var analysisArgs = new AnalysisArgs();
            List<AnalyzerConfiguration> selectedAnalyzers = new List<AnalyzerConfiguration>();
            foreach (TreeNode item in ListView.Nodes) {
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

        internal override List<AnalyzerConfiguration> GetSelectedAnalyzerConfigurations() {
            var analyzerConfigs = new List<AnalyzerConfiguration>();
            foreach (TreeNode node in ListView.Nodes) {
                if (node.Checked) {
                    analyzerConfigs.Add((AnalyzerConfiguration)node.Tag);
                }
            }
            return analyzerConfigs;
        }
    }
}
