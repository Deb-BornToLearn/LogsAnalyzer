using LogsAnalyzer.Infrastructure.Analysis;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms {
    public abstract class BaseTreeViewRenderer<T> where T : BaseLogAnalyzer {
        public abstract TreeNode Render();

        public T Analyzer { get; protected set; }

        public void SetAnalyzer(BaseLogAnalyzer analyzer) {
            Analyzer = analyzer as T;
        }

        protected TreeNode CreateNode(string text) {
            return new TreeNode { 
                Text = text
            };
        }
    }
}
