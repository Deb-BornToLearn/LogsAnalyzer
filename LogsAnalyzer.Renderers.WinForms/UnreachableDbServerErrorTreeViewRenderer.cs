using LogAnalyzer.Analyzers.Errors.Database;
using LogsAnalyzer.Infrastructure.Analysis;
using System.Linq;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms {
    public class UnreachableDbServerErrorTreeViewRenderer : BaseTreeViewRenderer<UnreachableServerErrorAnalyzer> {
        public override TreeNode Render() {
            if (!Analyzer.ErrorSources.Any()) {
                return CreateNode(Analyzer.NoErrorFoundMessage);
            }

            var rootNode = CreateNode($"Unreachable Database error(s) found: {Analyzer.ErrorSources.Count}");

            rootNode.Nodes.Add(CreateNode($"First occurrence at line {Analyzer.ErrorSources.First().LineNumber} in {Analyzer.ErrorSources.First().Source}"));

            rootNode.Nodes.Add(CreateNode($"Last occurrence at line {Analyzer.ErrorSources.Last().LineNumber} in {Analyzer.ErrorSources.Last().Source}"));

            var errorMessageNode = CreateNode("Error message");
            rootNode.Nodes.Add(errorMessageNode);

            int charsPerLine = 100;
            var chunkDefinitions = StringChunker.ComputeChunks(Analyzer.ErrorMessage, charsPerLine);
            foreach (var chunkDef in chunkDefinitions) {
                var messageChunk = Analyzer.ErrorMessage.Substring(chunkDef.StartPosition, chunkDef.ChunkLength);
                errorMessageNode.Nodes.Add(CreateNode(messageChunk));
            }
            return rootNode;
        }
    }
}
