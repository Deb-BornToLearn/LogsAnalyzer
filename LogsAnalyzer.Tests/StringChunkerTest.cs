using LogsAnalyzer.Renderers.WinForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogAnalyzer.Tests.StringChunkerTests {
    [TestClass]
    public class StringChunkerTest {
        [TestMethod]
        public void itShouldNotBreakWords1() {
            var charsPerChunk = 2;
            var input = "abc def";

            var result = StringChunker.ComputeChunks(input, charsPerChunk);

            Assert.AreEqual(2, result.Count);
            foreach (var chunk in result) {
                var chunked = input.Substring(chunk.StartPosition, chunk.ChunkLength);
            }
        }

        [TestMethod]
        public void itShouldNotBreakWords2() {
            var charsPerChunk = 2;
            var input = "abc def ghi";

            var result = StringChunker.ComputeChunks(input, charsPerChunk);

            Assert.AreEqual(3, result.Count);
            foreach (var chunk in result) {
                var chunked = input.Substring(chunk.StartPosition, chunk.ChunkLength);
            }
        }

        [TestMethod]
        public void itShouldNotBreakWords3() {
            var charsPerChunk = 7;
            var input = "abc def";

            var result = StringChunker.ComputeChunks(input, charsPerChunk);

            Assert.AreEqual(1, result.Count);
            foreach (var chunk in result) {
                var chunked = input.Substring(chunk.StartPosition, chunk.ChunkLength);
            }
        }
    }
}
