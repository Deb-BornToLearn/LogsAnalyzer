using System.Collections.Generic;
using System.Text;

namespace LogAnalyzer.Infrastructure.Analysis {
    public abstract class BaseLogAnalyzer : ILogAnalyzer {
        public List<AnalysisResult> Results { get; protected set; }
        public BaseLogAnalyzer() {
            Results = new List<AnalysisResult>();
        }
        public abstract bool Analyze(string lineText, long lineNumber, string sourceName);

        public virtual void BeginRead(string sourceName){ }

        public virtual void BeginReadAll() { }

        public virtual void EndRead(string sourceName) { }

        public virtual void EndReadAll() { }

        public virtual string AnalysesToString() {
            var sb = new StringBuilder();
            foreach (var a in Results) {
                sb.AppendLine(a.ToString());
            }
            return sb.ToString();
        }
    }
}
