using LogAnalyzer.Analyzers.Inventory;
using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms.TreeView {
    public class ClosedForCheckoutAnalysisTreeViewRenderer : BaseTreeViewRenderer {
        protected ClosedForCheckoutAnalyzer Analyzer;
        public override Type AnalyzerType => typeof(ClosedForCheckoutAnalyzer);

        public override TreeNode Render() {
            var rootNode = new TreeNode {
                Text = $"Closed for Checkout Analyzer Results ({Analyzer.ClosedForCheckoutResults.Count})"
            };
            foreach (var result in Analyzer.ClosedForCheckoutResults) {
                var inventoryNode = CreateNodeWithCommonContextMenuStrip($"Inventory {result.InventoryId} | {result.Date}");
                var sourceNode = CreateNode($"Ln {result.StartLineNumber} to {result.EndLineNumber} in {result.Source}");
                ContextMenuStrips.Add(sourceNode, CreateContextMenuItemForLogFile(sourceNode, result.Source));

                var productNode = CreateNodeWithCommonContextMenuStrip($"Product: {result.ProductName} ({result.ProductId})");
                var closedForCheckoutResultNode = CreateNodeWithCommonContextMenuStrip($"ClosedForCheckout result: {result.ClosedForCheckoutResult}");
                inventoryNode.Nodes.Add(productNode);
                inventoryNode.Nodes.Add(closedForCheckoutResultNode);
                inventoryNode.Nodes.Add(sourceNode);

                var dailyRatePlanLineNodes = CreateNode("Daily Rate Plan Lines");
                foreach (var drpl in result.DailyRatePlanLines) {
                    var drplNode = CreateNodeWithCommonContextMenuStrip($"{drpl.Id} - ClosedForCheckout: {drpl.ClosedForCheckout}");
                    dailyRatePlanLineNodes.Nodes.Add(drplNode);
                }
                inventoryNode.Nodes.Add(dailyRatePlanLineNodes);
                rootNode.Nodes.Add(inventoryNode);
            }
            return rootNode;
        }

        public override void SetAnalyzer(BaseLogAnalyzer analyzer) {
            Analyzer = analyzer as ClosedForCheckoutAnalyzer;
        }

        private ContextMenuStrip CreateContextMenuItemForLogFile(TreeNode node, string filename) {
            var contextMenuStrip = CreateCommonContextMenuStrip(node);
            contextMenuStrip.Items.Add(CreateOpenFileInNotepadPlusMenuItem("Open log file in Notepad++", filename));
            return contextMenuStrip;
        }
    }
}
