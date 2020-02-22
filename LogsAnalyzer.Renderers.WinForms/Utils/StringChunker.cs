using System;
using System.Collections.Generic;

namespace LogsAnalyzer.Renderers.WinForms {
    public class StringChunker {
        public static List<ChunkDefinition> ComputeChunksWithMinCharLimit(string input, int charsPerChunk) {
            var chunkPositions = new List<ChunkDefinition>();
            var inputLength = input.Length;
            int currentStartPos = 0;
            while (true) {
                int chunkLength = charsPerChunk;
                int currentPos = currentStartPos + chunkLength;
                if (currentPos >= inputLength) {
                    while (currentPos > inputLength) {
                        chunkLength--;
                        currentPos--;
                    }
                    chunkPositions.Add(new ChunkDefinition {
                        StartPosition = currentStartPos,
                        ChunkLength = chunkLength
                    });
                    break;
                } else {
                    while (currentPos < inputLength && input[currentPos] != ' ') {
                        currentPos++;
                        chunkLength++;
                    }
                    chunkPositions.Add(new ChunkDefinition {
                        StartPosition = currentStartPos,
                        ChunkLength = chunkLength
                    });
                    currentStartPos += chunkLength + 1;
                    if (currentStartPos >= inputLength) break;
                }
            }
            return chunkPositions;
        }
    }

    public struct ChunkDefinition {
        public int StartPosition;
        public int ChunkLength;
        public ChunkDefinition(int startPosition, int chunkLength) {
            StartPosition = startPosition;
            ChunkLength = chunkLength;
        }
    }
}
