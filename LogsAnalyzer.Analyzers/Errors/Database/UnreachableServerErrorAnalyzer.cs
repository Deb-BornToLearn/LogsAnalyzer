using System.Linq;

namespace LogAnalyzer.Analyzers.Errors.Database {
    public class UnreachableServerErrorAnalyzer : ErrorSummarizer {
        public override string NoErrorFoundMessage => "No unreachable server errors found";
        public UnreachableServerErrorAnalyzer() {
            SubstringsToMatch.Add("SqlException");
            SubstringsToMatch.Add("The server was not found or was not accessible");
        }

        public override string AnalysesToString() {
            if (ErrorSources.Any()) {
                return $"Unreachable database server error(s) found: {ErrorSources.Count}, starting at line {ErrorSources.First().LineNumber} in {ErrorSources.First().Source}: {ErrorMessage}";
            }

            return NoErrorFoundMessage;
        }

    }
}
