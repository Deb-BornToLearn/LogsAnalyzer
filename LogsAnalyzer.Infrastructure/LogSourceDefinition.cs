using System.Collections.Generic;

namespace LogAnalyzer.Infrastructure {
    public class LogSourceDefinition {
        public readonly List<string> SourceFiles;
        public readonly List<string> SourceFolders;

        public LogSourceDefinition() {
            SourceFiles = new List<string>();
            SourceFolders = new List<string>();

        }
    }
}
