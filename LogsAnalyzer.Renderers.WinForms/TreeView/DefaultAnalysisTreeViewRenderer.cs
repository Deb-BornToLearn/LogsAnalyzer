using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms.TreeView {
    public class DefaultAnalysisTreeViewRenderer : BaseTreeViewRenderer {
        public override Type AnalyzerType => typeof(BaseLogAnalyzer);

        public BaseLogAnalyzer Analyzer { get; protected set; }

        public override TreeNode Render() {
            var node = CreateNode(Analyzer.AnalysesToString());
            ContextMenuStrips.Add(node, CreateCommonContextMenuStrip(node));
            return node;
        }

        public override void SetAnalyzer(BaseLogAnalyzer analyzer) {
            Analyzer = analyzer;
        }
    }
}
