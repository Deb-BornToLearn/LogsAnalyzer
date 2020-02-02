using System.Collections.Generic;

namespace LogsAnalyzer.Infrastructure.Analysis {
    public class AnalysisResult {
        public long StartLineNumber;
        public long EndLineNumber;
        public string Source;
        public string Text;
        public List<string> MiscellaneousTraceData{ get; protected set; }

        public AnalysisResult() {
            MiscellaneousTraceData = new List<string>();
        }

        public override string ToString() {
            return Text;
        }

        
    }
}
