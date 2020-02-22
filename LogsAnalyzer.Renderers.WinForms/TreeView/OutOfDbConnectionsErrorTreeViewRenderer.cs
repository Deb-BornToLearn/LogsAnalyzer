using LogAnalyzer.Analyzers.Errors.Database;
using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Linq;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms.TreeView {
    public class OutOfDbConnectionsErrorTreeViewRenderer : BaseTreeViewRenderer {
        protected OutOfConnectionsErrorAnalyzer Analyzer;
        public override Type AnalyzerType => typeof(OutOfConnectionsErrorAnalyzer);

        public override TreeNode Render() {
            if (!Analyzer.ErrorSources.Any()) {
                return CreateNode(Analyzer.NoErrorFoundMessage);
            }

            var rootNode = CreateNode($"Out-of-database-connections error(s) found: {Analyzer.ErrorSources.Count}");
            ContextMenuStrips.Add(rootNode, createContextMenuStripForErrorNode(rootNode));

            var firstOccurrenceNode = CreateNode($"First occurrence at line {Analyzer.ErrorSources.First().LineNumber} in {Analyzer.ErrorSources.First().Source}");
            ContextMenuStrips.Add(firstOccurrenceNode,
                                  createContextMenuForLogFile(firstOccurrenceNode, Analyzer.ErrorSources.First().Source));
            rootNode.Nodes.Add(firstOccurrenceNode);

            var lastOccurrenceNode = CreateNode($"Last occurrence at line {Analyzer.ErrorSources.Last().LineNumber} in {Analyzer.ErrorSources.Last().Source}");
            ContextMenuStrips.Add(lastOccurrenceNode,
                                 createContextMenuForLogFile(lastOccurrenceNode, Analyzer.ErrorSources.Last().Source));
            rootNode.Nodes.Add(lastOccurrenceNode);

            TreeNode errorMessageNode = CreateNode("Error message");
            rootNode.Nodes.Add(errorMessageNode);

            ContextMenuStrips.Add(errorMessageNode, createContextMenuStripForErrorNode(errorMessageNode));

            CreateChunkedNodesFromString(Analyzer.ErrorMessage)
                .ToList().ForEach(n => errorMessageNode.Nodes.Add(n));

            return rootNode;
        }

        public override void SetAnalyzer(BaseLogAnalyzer analyzer) {
            Analyzer = analyzer as OutOfConnectionsErrorAnalyzer;
        }

        private ContextMenuStrip createContextMenuForLogFile(TreeNode node, string source) {
            var contextMenuStrip = CreateCommonContextMenuStrip(node);
            contextMenuStrip.Items.Add(CreateOpenFileInNotepadPlusMenuItem("Open log file in Notepad++", source));
            return contextMenuStrip;
        }


        private ContextMenuStrip createContextMenuStripForErrorNode(TreeNode node) {
            var contextMenuStrip = CreateCommonContextMenuStrip(node);
            contextMenuStrip.Items.Add(CreateCopyToClipboardMenuItem("Copy error message to Clipboard", Analyzer.ErrorMessage));
            return contextMenuStrip;
        }
    }
}
