using System.Collections.Generic;
using System.Linq;

namespace LogAnalyzer.Analyzers.Errors {
    public struct ErrorSource {
        public string Source;
        public long LineNumber;
        public ErrorSource(string source, long lineNumber) {
            Source = source;
            LineNumber = lineNumber;
        }
    }

    public class ErrorSummarizer : GenericErrorAnalyzer {
        public List<string> SubstringsToMatch = new List<string>();

        public List<ErrorSource> ErrorSources = new List<ErrorSource>();

        //public List<long> LineNumbers = new List<long>();
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
                    //LineNumbers.Add(lineNumber);
                    ErrorSources.Add(new ErrorSource(sourceName, lineNumber));
                }
            }
            return analyzed;
        }
    }
}
