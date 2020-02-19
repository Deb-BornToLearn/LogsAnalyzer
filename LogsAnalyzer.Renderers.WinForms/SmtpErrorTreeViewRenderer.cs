using LogAnalyzer.Analyzers.Errors.Smtp;
using LogsAnalyzer.Infrastructure.Analysis;
using System.Linq;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms {
    public class SmtpErrorTreeViewRenderer : BaseTreeViewRenderer<SmtpErrorAnalyzer> {
        public override TreeNode Render() {
            if (!Analyzer.ErrorSources.Any()) {
                return CreateNode(Analyzer.NoErrorFoundMessage);
            }

            var rootNode = CreateNode($"SMTP error(s) found: {Analyzer.ErrorSources.Count}");

            rootNode.Nodes.Add(CreateNode($"First occurrence at line {Analyzer.ErrorSources.First().LineNumber} in {Analyzer.ErrorSources.First().Source}"));

            rootNode.Nodes.Add(CreateNode($"Last occurrence at line {Analyzer.ErrorSources.Last().LineNumber} in {Analyzer.ErrorSources.Last().Source}"));

            var errorMessageNode = CreateNode("Error message");
            rootNode.Nodes.Add(errorMessageNode);

            int minCharsPerLine = 100;
            var chunkDefinitions = StringChunker.ComputeChunks(Analyzer.ErrorMessage, minCharsPerLine);
            foreach (var chunkDef in chunkDefinitions) {
                var messageChunk = Analyzer.ErrorMessage.Substring(chunkDef.StartPosition, chunkDef.ChunkLength);
                errorMessageNode.Nodes.Add(CreateNode(messageChunk));
            }
            return rootNode;
        }
    }
}
