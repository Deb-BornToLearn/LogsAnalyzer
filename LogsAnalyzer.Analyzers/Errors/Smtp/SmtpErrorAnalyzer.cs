using System;
using System.Collections.Generic;
using System.Linq;

namespace LogAnalyzer.Analyzers.Errors.Smtp {
    public class SmtpErrorAnalyzer<T> : ErrorSummarizer<T> {
        public override string NoErrorFoundMessage => "No SMTP errors found";
        public SmtpErrorAnalyzer() {
            SubstringsToMatch.Add("Failure sending mail");
        }

        public override string AnalysesToString() {
            if (LineNumbers.Any()) {
                return $"SMTP error(s) found: {LineNumbers.Count}, starting at line {LineNumbers.First()}: {ErrorMessage}";
            }

            return NoErrorFoundMessage;
        }
    }
}
