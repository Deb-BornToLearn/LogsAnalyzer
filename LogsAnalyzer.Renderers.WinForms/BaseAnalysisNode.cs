using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogsAnalyzer.Renderers.WinForms {
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
