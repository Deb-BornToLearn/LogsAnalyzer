using System.Collections.Generic;
using System.Linq;

namespace LogAnalyzer.Analyzers.Errors {
    public class ErrorSummarizer<T> : GenericErrorAnalyzer<T> {
        public List<string> SubstringsToMatch = new List<string>();

        public List<long> LineNumbers = new List<long>();
        public string ErrorMessage { get; protected set; }

        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            var analyzed = base.Analyze(lineText, lineNumber, sourceName);
            if (analyzed) {
                foreach (var substring in SubstringsToMatch) {
                    analyzed = lineText.Contains(substring);
                    if (!analyzed) break;
                }                
                if (analyzed) {
                    ErrorMessage = Errors.Last().ErrorMessage;
                    LineNumbers.Add(lineNumber);
                }
            }
            return analyzed;
        }
    }
}
