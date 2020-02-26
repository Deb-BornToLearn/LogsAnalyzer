using LogAnalyzer.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LogAnalyzer.UI.WinForms.Controllers {
    public class LogSourceTreeViewController<T> : BaseLogSourceListController<TreeView> {
        public LogSourceTreeViewController(TreeView listView) : base(listView) {

        }

        public override void AddFile(string filename) {
            var isFileInList = false;
            foreach (TreeNode node in ListView.Nodes) {
                if (isFileNode(node) && node.Text == filename) {
                    return;
                }
                if (isFolderNode(node)) {
                    foreach (TreeNode fileNode in node.Nodes) {
                        if (fileNode.Text == filename) return;
                    }
                    if (isDirectDescendantOf(filename, node.Text)) {
                        node.Nodes.Add(createNode(filename, ItemType.File));
                        return;
                    }
                }
            }

            if (!isFileInList) {
                var newNode = new TreeNode {
                    Text = filename,
                    Checked = true,
                    Tag = ItemType.File
                };
                ListView.Nodes.Add(newNode);
            }
        }

        private bool isDirectDescendantOf(string filename, string folderName) {
            return Path.GetDirectoryName(filename) == folderName;
        }

        internal override void AddFolder(string folder, bool addFiles) {
            if (isFolderInList(folder)) return;

            var folderNode = createNode(folder, ItemType.Folder);
            ListView.Nodes.Add(folderNode);

            if (addFiles) {
                var filesInFolder = Directory.GetFiles(folder);
                foreach (var file in filesInFolder) {
                    var childFileNode = createNode(file, ItemType.File);
                    folderNode.Nodes.Add(childFileNode);
                }
                removeDescendantsInList(folderNode);
            }
            else {
                moveDescendantsInList(folderNode);
            }

            folderNode.Expand();
        }

        private void moveDescendantsInList(TreeNode folderNode) {
            List<TreeNode> nodesToMove = new List<TreeNode>();
            foreach (TreeNode node in ListView.Nodes) {
                if (isFileNode(node) && isDirectDescendantOf(node.Text, folderNode.Text)) {
                    nodesToMove.Add(node);
                }
            }

            nodesToMove.ForEach(n => {
                ListView.Nodes.Remove(n);
                folderNode.Nodes.Add(n);
            });
        }

        private void removeDescendantsInList(TreeNode folderNode) {
            List<TreeNode> nodesToRemove = new List<TreeNode>();
            foreach (var fileNode in ListView.Nodes
                                                .OfType<TreeNode>()
                                                .Where(f => isFileNode(f) &&
                                                            isDirectDescendantOf(f.Text, folderNode.Text))) {
                nodesToRemove.Add(fileNode);
            }
            nodesToRemove.ForEach(n => ListView.Nodes.Remove(n));
        }

        private bool isFolderInList(string folder) {
            return ListView.Nodes.OfType<TreeNode>().Any(n => n.Text == folder);
        }

        internal override bool HasAny() {
            return ListView.Nodes.Count > 0;
        }


        internal override bool HasSelectedAny() {
            foreach (TreeNode node in ListView.Nodes) {
                if (node.Checked) return true;
                foreach (TreeNode fileNode in node.Nodes) {
                    if (fileNode.Checked) return true;
                }
            }
            return false;
        }

        internal override bool IsSelectedItemFolder() {
            if (ListView.SelectedNode == null) return false;

            return isFolderNode(ListView.SelectedNode);
        }


        private bool isFileNode(TreeNode node) {
            return (ItemType)node.Tag == ItemType.File;
        }

        private bool isFolderNode(TreeNode node) {
            return (ItemType)node.Tag == ItemType.Folder;
        }

        internal override void RemoveAllItems() {
            ListView.Nodes.Clear();
        }

        internal override void RemoveSelectedItems() {
            var nodesToRemove = new List<TreeNode>();
            var itemList = new List<string>();
            foreach (TreeNode node in ListView.Nodes.OfType<TreeNode>().Where(n => n.Checked)) {
                if (node.Nodes.Count > 0) {
                    var deleteFolderNode = true;
                    foreach (TreeNode fileNode in node.Nodes) {
                        if (fileNode.Checked) {
                            nodesToRemove.Add(fileNode);
                        }
                        else {
                            // Don't delete folder node if at least one file sub-node is unchecked.
                            if (deleteFolderNode) deleteFolderNode = false;
                        }
                    }
                    if (deleteFolderNode) nodesToRemove.Add(node);
                }
                else {
                    nodesToRemove.Add(node);
                }
            }

            nodesToRemove.ForEach(n => ListView.Nodes.Remove(n));
        }
        private TreeNode createNode(string filename, ItemType itemType) {
            return new TreeNode {
                Text = filename,
                Checked = true,
                Tag = itemType
            };
        }

        internal override LogSourceDefinition BuildLogSourceDefinition() {
            return buildLogSourceDefinition(false);
        }

        internal override LogSourceDefinition BuildLogSourceDefinitionFromSelection() {
            return buildLogSourceDefinition(true);
        }
        private LogSourceDefinition buildLogSourceDefinition(bool selectedOnly) {
            var logSourceDefinition = new LogSourceDefinition();
            foreach (TreeNode node in ListView.Nodes) {
                if (selectedOnly && !node.Checked) continue;

                if (isFileNode(node)) {
                    logSourceDefinition.SourceFiles.Add(node.Text);
                }
                else if (isFolderNode(node)) {
                    logSourceDefinition.SourceFolders.Add(node.Text);
                    foreach (TreeNode fileNode in node.Nodes) {
                        if (selectedOnly && !fileNode.Checked) continue;

                        logSourceDefinition.SourceFiles.Add(fileNode.Text);
                    }
                }
            }
            return logSourceDefinition;
        }

        internal override void AddLogSourceDefinition(LogSourceDefinition logSourceDefinition) {
            logSourceDefinition.SourceFiles.ForEach(f => AddFile(f));
            logSourceDefinition.SourceFolders.ForEach(f => AddFolder(f, true));
        }
    }
}
