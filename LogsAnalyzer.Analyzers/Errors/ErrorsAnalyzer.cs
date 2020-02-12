using LogAnalyzer.Analyzers.Errors.UnhandledErrors;
using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogAnalyzer.Analyzers.Errors {
    public class ErrorsAnalyzer : BaseLogAnalyzer {
        public const string ERROR_LOG_PATTERN = @"ERROR.*?:\s*(.*)";
        public List<ErrorAnalysis> Errors = new List<ErrorAnalysis>();

       
        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, ERROR_LOG_PATTERN);
            if (m.Success) {
                var error = new ErrorAnalysis(m.Groups[1].Value);
                error.StartLineNumber = lineNumber;
                error.EndLineNumber = lineNumber;
                error.Source = sourceName;
                Errors.Add(error);
            }
            return m.Success;
        }
        public override string AnalysesToString() {
            if (!Errors.Any()) return "No errors found";

            var sb = new StringBuilder();
            foreach (var error in Errors) {
                sb.AppendLine($"Ln {error.StartLineNumber} {error.ErrorMessage}");
            }
            return sb.ToString();
        }
    }
}
