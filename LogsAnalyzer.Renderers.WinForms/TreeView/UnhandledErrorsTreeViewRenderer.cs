using LogAnalyzer.Analyzers.Errors.UnhandledErrors;
using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Linq;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms.TreeView {
    public class UnhandledErrorsTreeViewRenderer : BaseTreeViewRenderer {
        protected UnhandledErrorsAnalyzer Analyzer;
        public override Type AnalyzerType => typeof(UnhandledErrorsAnalyzer);

        
        public override TreeNode Render() {
            var rootNode = CreateNode($"Unhandled error(s) found: {Analyzer.Errors.Count}");

            var errorsBySource = Analyzer.Errors.GroupBy(e => e.Source,
                                                         (key, g) => new { 
                                                            Source = key,
                                                            Errors = g.ToList()
                                                         });
            foreach (var errorGroup in errorsBySource) { 
                var errorNode = CreateNode($"{errorGroup.Source}");
                ContextMenuStrips.Add(errorNode, createContextMenuForLogFile(errorNode, errorGroup.Source));
                foreach (var error in errorGroup.Errors) {
                    errorNode.Nodes.Add($"Ln {error.StartLineNumber}: {error.ErrorMessage}");
                }
                rootNode.Nodes.Add(errorNode);
            }
            return rootNode;
        }

        public override void SetAnalyzer(BaseLogAnalyzer analyzer) {
            Analyzer = analyzer as UnhandledErrorsAnalyzer;
        }

        private ContextMenuStrip createContextMenuForLogFile(TreeNode node, string source) {
            var contextMenuStrip = CreateCommonContextMenuStrip(node);
            contextMenuStrip.Items.Add(CreateOpenFileInNotepadPlusMenuItem("Open log file in Notepad++", source));
            return contextMenuStrip;
        }
    }
}
