using System.Linq;

namespace LogAnalyzer.Analyzers.Errors.Database {
    public class OutOfConnectionsErrorAnalyzer : ErrorSummarizer {
        public override string NoErrorFoundMessage => "No all-connections-in-use errors found";

        public OutOfConnectionsErrorAnalyzer() {
            SubstringsToMatch.Add("all pooled connections were in use");
        }

        public override string AnalysesToString() {
            if (ErrorSources.Any()) {
                return $"All-connections-in-use errors found: {ErrorSources.Count}, starting at line {ErrorSources.First().LineNumber}: {ErrorMessage}";
            }

            return NoErrorFoundMessage;
        }

    }
}
