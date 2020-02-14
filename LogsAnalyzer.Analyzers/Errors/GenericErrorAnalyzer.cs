using LogsAnalyzer.Infrastructure.Analysis;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Analyzers.Errors {
    public class GenericErrorAnalyzer : BaseLogAnalyzer {
        public const string ERROR_LOG_PATTERN = @"ERROR.*?:\s*(.*)";
        
        public List<GenericErrorAnalysis> Errors = new List<GenericErrorAnalysis>();

        public virtual string NoErrorFoundMessage => "No errors found";
         
        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, ERROR_LOG_PATTERN);
            if (m.Success) {
                var error = new GenericErrorAnalysis(m.Groups[1].Value);
                error.StartLineNumber = lineNumber;
                error.EndLineNumber = lineNumber;
                error.Source = sourceName;
                Errors.Add(error);
            }
            return m.Success;
        }
        public override string AnalysesToString() {
            if (!Errors.Any()) return NoErrorFoundMessage;

            var sb = new StringBuilder();
            foreach (var error in Errors) {
                sb.AppendLine($"Ln {error.StartLineNumber} {error.ErrorMessage}");
            }
            return sb.ToString();
        }
    }
}
