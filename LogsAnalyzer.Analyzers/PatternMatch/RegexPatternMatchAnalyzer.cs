using LogAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Infrastructure.Analysis;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Analyzers.PatternMatch {
    public class RegexPatternMatchAnalyzer : BaseLogAnalyzer {
        public readonly string RegexPattern;
        private Regex _regex;
        public RegexPatternMatchAnalyzer(string regexPattern) {
            RegexPattern = regexPattern;
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            var matched = _regex.IsMatch(lineText);
            if (matched) {
                Results.Add(new BaseAnalysisResult {
                    Text = lineText,
                    Source = sourceName,
                    StartLineNumber = lineNumber,
                    EndLineNumber = lineNumber
                });
            }
            return matched;
        }
    }
}
