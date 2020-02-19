using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LogAnalyzer.UI.WinForms {
    public class HiddenNode {
        public TreeNode Node { get; protected set; }
        public TreeNode ParentNode { get; protected set; }
        public TreeView TreeView { get; protected set; }
        public int Index { get; protected set; }
        public HiddenNode(TreeNode node, int index, TreeNode parentNode) {
            Node = node;
            Index = index;
            ParentNode = parentNode;
            TreeView = node.TreeView;
        }
    }
    public class TreeViewFilterer {
        public readonly TreeView TargetTreeView;
        protected readonly List<HiddenNode> HiddenNodes;
        public string CurrentFilter { get; protected set; }
        public TreeViewFilterer(TreeView treeView) {
            TargetTreeView = treeView;
            HiddenNodes = new List<HiddenNode>();
        }
        public void Filter(string filter) {
            CurrentFilter = filter;
            unhideNodes();
            doFilter();
            ensureSelectedNodeIsVisible();
        }

        private void ensureSelectedNodeIsVisible() {
            if (TargetTreeView.SelectedNode != null) {
                TargetTreeView.SelectedNode.EnsureVisible();
            }
        }

        private void unhideNodes() {
            foreach (var hiddenNode in HiddenNodes) {
                unhideNode(hiddenNode);
            }
            HiddenNodes.Clear();
        }

        private void doFilter() {
            foreach (TreeNode node in TargetTreeView.Nodes) {
                tagFilterState(node);
            }
            gatherNodesToHide();
            hideNodes();
        }


        private void hideNodes() {
            foreach (var hiddenNode in HiddenNodes) {
                hideNode(hiddenNode);
            }
        }

        private void hideNode(HiddenNode nodeToHide) {
            if (nodeToHide.ParentNode != null) {
                nodeToHide.ParentNode.Nodes.Remove(nodeToHide.Node);
            }
            else {
                nodeToHide.TreeView.Nodes.Remove(nodeToHide.Node);
            }
        }

        private void unhideNode(HiddenNode hiddenNode) {
            if (hiddenNode.ParentNode != null) {
                hiddenNode.ParentNode.Nodes.Insert(hiddenNode.Index, hiddenNode.Node);
            }
            else {
                hiddenNode.TreeView.Nodes.Insert(hiddenNode.Index, hiddenNode.Node);
            }
        }

        private void gatherNodesToHide() {
            foreach (TreeNode node in TargetTreeView.Nodes) {
                tryPutNodeToHiddenList(node);
            }
        }

        private void tryPutNodeToHiddenList(TreeNode node) {
            if ((bool)node.Tag == false) {
                HiddenNodes.Add(new HiddenNode(node, node.Index, node.Parent));
            }
            else {
                foreach (TreeNode childNode in node.Nodes) {
                    tryPutNodeToHiddenList(childNode);
                }
            }
        }

        private void tagFilterState(TreeNode node) {
            node.Tag = node.Text.IndexOf(CurrentFilter, StringComparison.InvariantCultureIgnoreCase) > -1;
            if ((bool)node.Tag) {
                ensureAllAncestorsAreNotFilteredOut(node);
            }
            foreach (TreeNode childNode in node.Nodes) {
                tagFilterState(childNode);
            }
        }

        private void ensureAllAncestorsAreNotFilteredOut(TreeNode node) {
            var currentParent = node.Parent;
            while (currentParent != null) {
                currentParent.Tag = true;
                currentParent = currentParent.Parent;
            }
        }
    }
}
