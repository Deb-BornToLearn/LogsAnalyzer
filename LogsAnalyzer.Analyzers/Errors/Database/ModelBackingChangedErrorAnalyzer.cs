using System.Linq;

namespace LogAnalyzer.Analyzers.Errors.Database {
    public class ModelBackingChangedErrorAnalyzer : ErrorSummarizer {

        public override string NoErrorFoundMessage => "No Model-Backing-Changed errors found";
        public ModelBackingChangedErrorAnalyzer() {
            SubstringsToMatch.Add("context has changed since the database was created");
        }

        public override string AnalysesToString() {
            if (ErrorSources.Any()) {
                return $"Model-Backing-Changed error(s) found: {ErrorSources.Count}, starting at line {ErrorSources.First().LineNumber} in {ErrorSources.First().Source}: {ErrorMessage}";
            }

            return NoErrorFoundMessage;
        }
    }
}
