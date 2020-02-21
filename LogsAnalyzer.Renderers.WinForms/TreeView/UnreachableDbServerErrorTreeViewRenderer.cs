using LogAnalyzer.Analyzers.Errors.Database;
using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Linq;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms.TreeView {
    public class UnreachableDbServerErrorTreeViewRenderer : BaseTreeViewRenderer {

        protected UnreachableServerErrorAnalyzer Analyzer;
        public UnreachableDbServerErrorTreeViewRenderer() {
            Analyzer = new UnreachableServerErrorAnalyzer();
        }

        public override Type AnalyzerType => typeof(UnreachableServerErrorAnalyzer);

        public override TreeNode Render() {
            if (!Analyzer.ErrorSources.Any()) {
                return CreateNode(Analyzer.NoErrorFoundMessage);
            }

            var rootNode = CreateNode($"Unreachable Database error(s) found: {Analyzer.ErrorSources.Count}");
            ContextMenuStrips.Add(rootNode, createContextMenuStripForRoot(rootNode));

            var firstOccurrenceNode = CreateNode($"First occurrence at line {Analyzer.ErrorSources.First().LineNumber} in {Analyzer.ErrorSources.First().Source}");
            rootNode.Nodes.Add(firstOccurrenceNode);
            ContextMenuStrips.Add(firstOccurrenceNode, createContextMenuForLogFile(firstOccurrenceNode, Analyzer.ErrorSources.First().Source));

            var lastOccurrenceNode = CreateNode($"Last occurrence at line {Analyzer.ErrorSources.Last().LineNumber} in {Analyzer.ErrorSources.Last().Source}");
            rootNode.Nodes.Add(lastOccurrenceNode);
            ContextMenuStrips.Add(lastOccurrenceNode, createContextMenuForLogFile(lastOccurrenceNode, Analyzer.ErrorSources.Last().Source));


            var errorMessageNode = CreateNode("Error message");
            ContextMenuStrips.Add(errorMessageNode, createContextMenuStripForRoot(errorMessageNode));
            rootNode.Nodes.Add(errorMessageNode);

            CreateChunkedNodesFromString(Analyzer.ErrorMessage)
                .ToList().ForEach(n => errorMessageNode.Nodes.Add(n));

            return rootNode;
        }

        public override void SetAnalyzer(BaseLogAnalyzer analyzer) {
            Analyzer = analyzer as UnreachableServerErrorAnalyzer;
        }

        private ContextMenuStrip createContextMenuForLogFile(TreeNode node, string file) {
            var contextMenuStrip = CreateCommonContextMenuStrip(node);
            contextMenuStrip.Items.Add(CreateOpenFileInNotepadPlusMenuItem("Open log file in Notepad++", file));
            return contextMenuStrip;
        }

        private ContextMenuStrip createContextMenuStripForRoot(TreeNode node) {
            var contextMenuStrip = CreateCommonContextMenuStrip(node);
            contextMenuStrip.Items.Add(CreateCopyToClipboardMenuItem("Copy error message to Clipboard", Analyzer.ErrorMessage));
            return contextMenuStrip;
        }
    }
}
