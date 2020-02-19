using System;
using System.Collections.Generic;
using System.Linq;

namespace LogAnalyzer.Analyzers.Errors.Smtp {
    public class SmtpErrorAnalyzer : ErrorSummarizer {
        public override string NoErrorFoundMessage => "No SMTP errors found";
        public SmtpErrorAnalyzer() {
            SubstringsToMatch.Add("Failure sending mail");
        }

        public override string AnalysesToString() {
            if (ErrorSources.Any()) {
                return $"SMTP error(s) found: {ErrorSources.Count}, starting at line {ErrorSources.First().LineNumber} in {ErrorSources.First().Source}: {ErrorMessage}";
            }

            return NoErrorFoundMessage;
        }
    }
}
