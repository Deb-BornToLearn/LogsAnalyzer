using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Analyzers.Migration.R2RPlus {
    public class R2RPlusMigrationAnalyzer : BaseLogAnalyzer {
        public readonly List<R2RPlusMigrationAnalysis> R2RPlusMigrationResults = new List<R2RPlusMigrationAnalysis>();
        private readonly List<Func<string, long, string, bool>> _logParsers = new List<Func<string, long, string, bool>>();

        private const string MIG_LOG_MARKER = @"\[MIG:(.*?)\|(.*?)\|(.*?)\|(.*?)\|(.*?)\]";

        private string _lastInsertedBookingError;

        public R2RPlusMigrationAnalyzer() {
            _logParsers.Add(tryParseBookingControllerPostLastError);
            _logParsers.Add(tryParseStartMigration);
            _logParsers.Add(tryParseEndMigration);
            _logParsers.Add(tryParseBusinessUpdate);
            _logParsers.Add(tryParseInsertBookingFailure);
            _logParsers.Add(tryParseInsertBookingSuccess);
        }
        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            foreach (var parser in _logParsers) {
                if (parser(lineText, lineNumber, sourceName)) {
                    return true;
                }
            }
            return false;
        }

        #region Parsers
        private bool tryParseStartMigration(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, $"{MIG_LOG_MARKER}.*Migration START");
            if (m.Success) {
                var result = new R2RPlusMigrationAnalysis(m);
                result.StartLineNumber = lineNumber;
                result.Source = sourceName;
                R2RPlusMigrationResults.Add(result);
            }
            return m.Success;
        }
        private bool tryParseEndMigration(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, $"{MIG_LOG_MARKER}.*Migration END. Result: (.*)");
            if (m.Success) {
                var result = new R2RPlusMigrationAnalysis(m);
                var analysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == result.LogId);
                if (analysis != null) {
                    analysis.HadErrors = m.Groups[6].Value.ToUpper() != "TRUE";
                }
            }
            return m.Success;
        }

        private bool tryParseBookingControllerPostLastError(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, "Exception.*\"Message\":\"(.*?)\".*BookingsController.Post\\(Guid businessId, Account account\\)");
            if (m.Success) {
                _lastInsertedBookingError = m.Groups[1].Value;
            }
            // Always return false because this parser is only for buffering the last potential error message 
            // of failed inserted bookings.
            return false;
        }

        private bool tryParseBusinessUpdate(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, $"{MIG_LOG_MARKER}.*Successfully updated business");
            if (m.Success) {
                var result = new R2RPlusMigrationAnalysis(m);
                var analysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == result.LogId);
                if (analysis != null) {
                    analysis.WasBusinessUpdated = true;
                }
            }
            return m.Success;
        }
        private bool tryParseInsertBookingFailure(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, $@"{MIG_LOG_MARKER}.*Failed to insert booking \[(.*)\]: (.*)");
            if (m.Success) {
                var result = new R2RPlusMigrationAnalysis(m);
                var analysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == result.LogId);
                if (analysis != null) {
                    var failedBooking = new InsertedData {
                        RId = m.Groups[6].Value,
                        IsOk = false
                    };
                    if (!string.IsNullOrEmpty(_lastInsertedBookingError)) {
                        failedBooking.StatusMessage = $"Probable cause: {_lastInsertedBookingError}";
                        _lastInsertedBookingError = null;
                    }
                    else {
                        failedBooking.StatusMessage = m.Groups[7].Value;
                    }
                    analysis.InsertedBookings.Add(failedBooking);
                }
            }
            return m.Success;
        }
        private bool tryParseInsertBookingSuccess(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, $@"{MIG_LOG_MARKER}.*Successfully inserted booking with Id(.*)");
            if (m.Success) {
                var result = new R2RPlusMigrationAnalysis(m);
                var analysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == result.LogId);
                if (analysis != null) {
                    analysis.InsertedBookings.Add(new InsertedData {
                        RId = m.Groups[6].Value,
                        IsOk = true
                    });
                }
            }
            return m.Success;
        }

        #endregion
    }
}
