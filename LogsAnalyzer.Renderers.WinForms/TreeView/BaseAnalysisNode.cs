using System.Collections.Generic;

namespace LogsAnalyzer.Renderers.WinForms.TreeView {
    public abstract class BaseAnalysisNode {
        public abstract object GetPropertyGridObject();
        public abstract string DisplayText { get; }

        public readonly List<BaseAnalysisNode> Nodes;


        public BaseAnalysisNode() {
            Nodes = new List<BaseAnalysisNode>();

            //PropertyGrid pg = null;
            //pg.SelectedObject
        }
    }
}
