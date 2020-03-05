using LogAnalyzer.Analyzers.Inventory;
using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms.TreeView {
    public class DailyInventoryCacheUpdateAnalysisTreeViewRenderer : BaseTreeViewRenderer {
        protected DailyInventoryCacheUpdateAnalyzer Analyzer;
        public override Type AnalyzerType => typeof(DailyInventoryCacheUpdateAnalyzer);

        public override TreeNode Render() {
            var rootNode = new TreeNode {
                Text = $"Daily Inventory Cache Update Analyzer Results ({Analyzer.DailyInventoryCacheUpdates.Count})"
            };
            foreach (var cacheUpdate in Analyzer.DailyInventoryCacheUpdates) {
                var acctNode = CreateNodeWithCommonContextMenuStrip($"{cacheUpdate.Reference} - {cacheUpdate.AccountId}");
                
                var sourceNode = CreateNode($"Source: {cacheUpdate.Source}");
                ContextMenuStrips.Add(sourceNode, CreateContextMenuItemForLogFile(sourceNode, cacheUpdate.Source));
                acctNode.Nodes.Add(sourceNode);

                acctNode.Nodes.Add($"Lines {cacheUpdate.StartLineNumber} to {cacheUpdate.EndLineNumber}");
                acctNode.Nodes.Add(CreateNodeWithCommonContextMenuStrip($"Old Start Date: {cacheUpdate.OldStartDate}, Old End Date: {cacheUpdate.OldEndDate}"));
                acctNode.Nodes.Add(CreateNodeWithCommonContextMenuStrip($"New Start Date: {cacheUpdate.NewStartDate}, New End Date: {cacheUpdate.NewEndDate}"));
                acctNode.Nodes.Add(CreateNodeWithCommonContextMenuStrip($"Old inventory: {cacheUpdate.OldInventoryIds}"));
                acctNode.Nodes.Add(CreateNodeWithCommonContextMenuStrip($"New inventory: {cacheUpdate.NewInventoryIds}"));

                foreach (var log in cacheUpdate.LogEntries) {
                    acctNode.Nodes.Add(CreateNodeWithCommonContextMenuStrip($"Ln {log.StartLineNumber} {log.LogText}"));
                }
                rootNode.Nodes.Add(acctNode);
            }
            return rootNode;
        }

        public override void SetAnalyzer(BaseLogAnalyzer analyzer) {
            Analyzer = analyzer as DailyInventoryCacheUpdateAnalyzer;
        }

        private ContextMenuStrip CreateContextMenuItemForLogFile(TreeNode node, string filename) {
            var contextMenuStrip = CreateCommonContextMenuStrip(node);
            contextMenuStrip.Items.Add(CreateOpenFileInNotepadPlusMenuItem("Open log file in Notepad++", filename));
            return contextMenuStrip;
        }
    }
}
