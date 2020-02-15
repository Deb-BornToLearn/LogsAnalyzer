using System;
using System.Windows.Forms;

namespace LogAnalyzer.UI.WinForms.Controllers {
    public class TreeViewAfterCheckController {

        internal void OnAfterCheck(object sender, EventArgs eventArgs) {
            TreeViewEventArgs e = eventArgs as TreeViewEventArgs;
            if (e == null) return;

            if (eventFiredViaCode(e)) return;

            foreach (TreeNode childNode in e.Node.Nodes) {
                childNode.Checked = e.Node.Checked;
            }
            if (e.Node.Parent != null) {
                var shouldCheckParentNode = false;
                foreach (TreeNode sibling in e.Node.Parent.Nodes) {
                    if (sibling.Checked) {
                        shouldCheckParentNode = true;
                        break;
                    }
                }
                e.Node.Parent.Checked = shouldCheckParentNode;
            }
        }
        private bool eventFiredViaCode(TreeViewEventArgs e) {
            return e.Action == TreeViewAction.Unknown;
        }

    }
}
