using System.Linq;

namespace LogAnalyzer.Analyzers.Errors.Smtp {
    public class SmtpErrorAnalyzer : ErrorSummarizer {
        public override string NoErrorFoundMessage => "No SMTP errors found";
        public SmtpErrorAnalyzer(params string[] substringMatches) {
            if (substringMatches == null || substringMatches.Length == 0) {
                SubstringsToMatch.Add("Failure sending mail");
            }
            else {
                SubstringsToMatch.AddRange(substringMatches);
            }
        }

        public override string AnalysesToString() {
            if (ErrorSources.Any()) {
                return $"SMTP error(s) found: {ErrorSources.Count}, starting at line {ErrorSources.First().LineNumber} in {ErrorSources.First().Source}: {ErrorMessage}";
            }

            return NoErrorFoundMessage;
        }
    }
}
