using LogAnalyzer.Analyzers.Errors.Smtp;
using System;
using System.Linq;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms {
    public class SmtpErrorTreeViewRenderer : BaseTreeViewRenderer<SmtpErrorAnalyzer> {
        private ContextMenuStrip _contextMenuStrip = null;

        public ContextMenuStrip ContextMenuStrip {
            get {
                return _contextMenuStrip;
            }
        }

        public override TreeNode Render() {
            if (_contextMenuStrip == null) {
                _contextMenuStrip = new ContextMenuStrip();
                ToolStripMenuItem stripItem = new ToolStripMenuItem();
                stripItem.Text = "Copy to clipboard";
                stripItem.Click += (source, args) => {
                    MessageBox.Show("Copied!");
                };
                _contextMenuStrip.Items.Add(stripItem);
            }

            if (!Analyzer.ErrorSources.Any()) {
                return CreateNode(Analyzer.NoErrorFoundMessage);
            }

            var rootNode = CreateNode($"SMTP error(s) found: {Analyzer.ErrorSources.Count}");

            rootNode.Nodes.Add(CreateNode($"First occurrence at line {Analyzer.ErrorSources.First().LineNumber} in {Analyzer.ErrorSources.First().Source}"));

            rootNode.Nodes.Add(CreateNode($"Last occurrence at line {Analyzer.ErrorSources.Last().LineNumber} in {Analyzer.ErrorSources.Last().Source}"));

            TreeNode errorMessageNode = CreateNode("Error message");
            rootNode.Nodes.Add(errorMessageNode);

            int minCharsPerLine = 100;
            var chunkDefinitions = StringChunker.ComputeChunks(Analyzer.ErrorMessage, minCharsPerLine);
            foreach (var chunkDef in chunkDefinitions) {
                var messageChunk = Analyzer.ErrorMessage.Substring(chunkDef.StartPosition, chunkDef.ChunkLength);
                errorMessageNode.Nodes.Add(CreateNode(messageChunk));
            }
            return rootNode;
        }

        private void copyToClipboard(object sender, EventArgs e) {
            MessageBox.Show("Copied!");
        }
    }
}
