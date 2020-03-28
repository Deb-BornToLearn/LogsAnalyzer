using LogAnalyzer.Analyzers.Migration.R2RPlus;
using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms.TreeView {
    public class R2RPlusMigrationTreeViewRenderer : BaseTreeViewRenderer {
        protected R2RPlusMigrationAnalyzer Analyzer;

        public override Type AnalyzerType => typeof(R2RPlusMigrationAnalyzer);

        public override TreeNode Render() {
            var rootResult = new TreeNode {
                Text = $"R2RPlus Migration Analyzer Results ({Analyzer.R2RPlusMigrationResults.Count})"
            };

            foreach (var r in Analyzer.R2RPlusMigrationResults) {
                var resultRootNode = CreateNodeWithCommonContextMenuStrip($"{r.LogId} | {r.RPlusBusinessId} | {r.RBusinessId}");
                resultRootNode.Nodes.Add(CreateNodeWithCommonContextMenuStrip($"Start: {r.StartDate}, End: {r.EndDate}"));
                resultRootNode.Nodes.Add(CreateNode($"Updated business? {r.WasBusinessUpdated}"));
                resultRootNode.Nodes.Add(CreateNode($"Migration all OK? {!r.HadErrors}"));
                renderInsertedBookings(resultRootNode, r.InsertedBookings);
             
                rootResult.Nodes.Add(resultRootNode);
            }

            return rootResult;
        }

        private void renderInsertedBookings(TreeNode parentNode, List<InsertedRPlusData> insertedBookings) {
            var failed = insertedBookings.Where(b => !b.IsOk).Count();
            var insertedBookingsNode = CreateNodeWithCommonContextMenuStrip($"Inserted bookings (Failed: {failed} | Total: {insertedBookings.Count})");
            foreach (var b in insertedBookings) {
                var ok = b.IsOk ? "OK" : "FAILED";
                var msg = string.IsNullOrEmpty(b.StatusMessage) ? "" : $"({b.StatusMessage})";
                var node = CreateNodeWithCommonContextMenuStrip($"Name: {b.Name ?? "none"} [{b.RId}] | {ok} {msg}");
                insertedBookingsNode.Nodes.Add(node);
            }
            parentNode.Nodes.Add(insertedBookingsNode);
        }

        public override void SetAnalyzer(BaseLogAnalyzer analyzer) {
            Analyzer = analyzer as R2RPlusMigrationAnalyzer;
        }
    }
}
