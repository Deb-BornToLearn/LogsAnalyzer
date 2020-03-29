using LogAnalyzer.Infrastructure.Analysis;
using System;
using System.Text.RegularExpressions;

namespace LogsAnalyzer.Infrastructure.Analysis.PatternMatch {
    public class RegexPatternMatchAnalyzer : BaseLogAnalyzer {
        private const int REGEX_MATCH_TIMEOUT = 3;
        public readonly string RegexPattern;
        private Regex _regex;
        public RegexPatternMatchAnalyzer(string regexPattern) {
            RegexPattern = regexPattern;
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(REGEX_MATCH_TIMEOUT));
        }
        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            bool matched = false;
            try {
                matched = _regex.IsMatch(lineText);
                if (matched) {
                    Results.Add(new BaseAnalysisResult {
                        Text = lineText,
                        Source = sourceName,
                        StartLineNumber = lineNumber,
                        EndLineNumber = lineNumber
                    });
                }
            }
            catch (RegexMatchTimeoutException) {
                Results.Add(new BaseAnalysisResult {
                    Text = $"RegexMatchTimeout of {REGEX_MATCH_TIMEOUT} seconds exceeded using pattern {RegexPattern} on input [{lineText}]",
                    Source = sourceName,
                    StartLineNumber = lineNumber,
                    EndLineNumber = lineNumber
                });
            }
            catch (Exception exc) {
                Results.Add(new BaseAnalysisResult {
                    Text = $"Exception [{exc.Message}] occurred while applying pattern {RegexPattern} on input [{lineText}].",
                    Source = sourceName,
                    StartLineNumber = lineNumber,
                    EndLineNumber = lineNumber
                });
            }
            return matched;
        }
    }
}
