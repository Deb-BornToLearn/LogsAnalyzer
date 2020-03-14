using LogAnalyzer.Analyzers.PatternMatch;
using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Linq;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms.TreeView {
    public class RegexPatternAnalysisTreeViewRenderer : BaseTreeViewRenderer {

        protected RegexPatternMatchAnalyzer Analyzer;

        public override Type AnalyzerType => typeof(RegexPatternMatchAnalyzer);

        public override TreeNode Render() {
            if (!Analyzer.Results.Any()) {
                return CreateNode($"No matches found for {Analyzer.DisplayName} using pattern \"{Analyzer.RegexPattern}\"");
            }
            var rootNode = CreateNode($"{Analyzer.DisplayName} matches found using pattern \"{Analyzer.RegexPattern}\": {Analyzer.Results.Count}");
            var matchesBySource = Analyzer.Results.GroupBy(e => e.Source,
                                                         (key, g) => new {
                                                             Source = key,
                                                             Matches = g.ToList()
                                                         });
            foreach (var matchGroup in matchesBySource) {
                var errorNode = CreateNode($"{matchGroup.Source} ({matchGroup.Matches.Count})");
                ContextMenuStrips.Add(errorNode, createContextMenuForLogFile(errorNode, matchGroup.Source));
                foreach (var match in matchGroup.Matches) {
                    var textNode = CreateNode($"Ln {match.StartLineNumber}: {match.Text}");
                    ContextMenuStrips.Add(textNode, CreateCommonContextMenuStrip(textNode));
                    errorNode.Nodes.Add(textNode);
                }
                rootNode.Nodes.Add(errorNode);
            }
            return rootNode;
        }

        public override void SetAnalyzer(BaseLogAnalyzer analyzer) {
            Analyzer = analyzer as RegexPatternMatchAnalyzer;
        }

        private ContextMenuStrip createContextMenuForLogFile(TreeNode node, string source) {
            var contextMenuStrip = CreateCommonContextMenuStrip(node);
            contextMenuStrip.Items.Add(CreateOpenFileInNotepadPlusMenuItem("Open log file in Notepad++", source));
            return contextMenuStrip;
        }
    }
}
