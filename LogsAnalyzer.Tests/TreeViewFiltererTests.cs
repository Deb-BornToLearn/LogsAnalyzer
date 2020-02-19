using LogAnalyzer.UI.WinForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Windows.Forms;

namespace LogAnalyzer.Tests.TreeViewFiltererTests {
    [TestClass]
    public class TreeViewFiltererTest {
        [TestMethod]
        public void itShouldTagNodesAsFiltered() {
            var sourceTreeView = new TreeView();
            sourceTreeView.Nodes.Add(createNode("Root node 1"));
            sourceTreeView.Nodes.Add(createNode("Sample node"));
            sourceTreeView.Nodes.Add(createNode("Another Root node"));

            var filterer = new TreeViewFilterer(sourceTreeView);
            filterer.Filter("Root");

            var taggedCount = countTagged(filterer.TargetTreeView);
            Assert.AreEqual(2, taggedCount);

            filterer.Filter("Sample");
            taggedCount = countTagged(filterer.TargetTreeView);
            Assert.AreEqual(1, taggedCount);

            filterer.Filter("qwerty");
            taggedCount = countTagged(filterer.TargetTreeView);
            Assert.AreEqual(0, taggedCount);

            filterer.Filter("o");
            taggedCount = countTagged(filterer.TargetTreeView);
            Assert.AreEqual(3, taggedCount);
        }

        [TestMethod]
        public void itShouldTagParentNodesWhenAtLeastOneOfChildNodesIsTagged() {
            var sourceTreeView = new TreeView();
            var countries = createNode("Countries");
            countries.Nodes.Add(createNode("Australia"));
            countries.Nodes.Add(createNode("Cambodia"));
            countries.Nodes.Add(createNode("Indonesia"));
            countries.Nodes.Add(createNode("Philippines"));
            countries.Nodes.Add(createNode("United States"));
            sourceTreeView.Nodes.Add(countries);

            var cities = createNode("Cities");
            cities.Nodes.Add(createNode("Jakarta"));
            cities.Nodes.Add(createNode("Manila"));
            cities.Nodes.Add(createNode("Subiaco"));
            sourceTreeView.Nodes.Add(cities);

            var continents = createNode("Continents");
            continents.Nodes.Add(createNode("Asia"));
            continents.Nodes.Add(createNode("Africa"));
            continents.Nodes.Add(createNode("North America"));
            continents.Nodes.Add(createNode("Europe"));
            sourceTreeView.Nodes.Add(continents);

            var filterer = new TreeViewFilterer(sourceTreeView);
            filterer.Filter("ia");

            var taggedCount = countTagged(filterer.TargetTreeView);
            Assert.AreEqual(8, taggedCount);

            filterer.Filter("Cities");
            taggedCount = countTagged(filterer.TargetTreeView);
            Assert.AreEqual(1, taggedCount);

            filterer.Filter("Manila");
            taggedCount = countTagged(filterer.TargetTreeView);
            Assert.AreEqual(2, taggedCount);

            filterer.Filter("qwerty");
            taggedCount = countTagged(filterer.TargetTreeView);
            Assert.AreEqual(0, taggedCount);
        }

        #region Helpers
        private int countTagged(TreeView treeView) {
            int count = treeView.Nodes.OfType<TreeNode>().Count(n => (bool)n.Tag);
            foreach (TreeNode node in treeView.Nodes) {
                count += countTagged(node);
            }
            return count;
        }
        private int countTagged(TreeNode parentNode) {
            var count = parentNode.Nodes.OfType<TreeNode>().Count(n => (bool)n.Tag);
            foreach (TreeNode childNode in parentNode.Nodes) {
                count += countTagged(childNode);
            }
            return count;
        }
        private TreeNode createNode(string text) {
            return new TreeNode {
                Text = text
            };
        }
        #endregion
    }
}
