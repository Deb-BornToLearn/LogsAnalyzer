using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Analyzers.Migration.R2RPlus {
    public class R2RPlusMigrationAnalyzer : BaseLogAnalyzer {
        public readonly List<R2RPlusMigrationAnalysis> R2RPlusMigrationResults = new List<R2RPlusMigrationAnalysis>();
        private readonly List<Func<string, R2RPlusMigrationAnalysis, bool>> _logParsers = new List<Func<string, R2RPlusMigrationAnalysis, bool>>();

        private const string MIG_LOG_MARKER = @"\[MIG:(.*?)\|(.*?)\|(.*?)\|(.*?)\|(.*?)\]";

        private string _lastInsertedBookingError;

        public R2RPlusMigrationAnalyzer() {
            _logParsers.Add(tryParseStartMigration);
            _logParsers.Add(tryParseEndMigration);
            _logParsers.Add(tryParseBusinessUpdate);
            _logParsers.Add(tryParseInsertBookingFailure);
            _logParsers.Add(tryParseInsertBookingSuccess);
        }
        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            tryParseBookingControllerPostLastError(lineText, lineNumber, sourceName);
            
            R2RPlusMigrationAnalysis analysis;
            string logMessage;
            if (!tryParseMigrationLog(lineText, lineNumber, sourceName, out logMessage, out analysis)) {
                return false;
            }

            foreach (var parser in _logParsers) {
                if (parser(logMessage, analysis)) {
                    return true;
                }
            }
            return false;
        }


         #region Parsers
        private bool tryParseMigrationLog(string lineText, long lineNumber, string sourceName, 
                                          out string logMessage, out R2RPlusMigrationAnalysis analysis) {
            analysis = null;
            logMessage = string.Empty;
            var m = Regex.Match(lineText, $"{MIG_LOG_MARKER}(.*)");
            if (m.Success) {
                analysis = new R2RPlusMigrationAnalysis(m);
                analysis.StartLineNumber = lineNumber;
                analysis.Source = sourceName;
                logMessage = m.Groups[6].Value;
            }
            return m.Success;
        }
       
        private bool tryParseStartMigration(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $"Migration START");
            if (m.Success) {
                R2RPlusMigrationResults.Add(analysis);
            }
            return m.Success;
        }
        private bool tryParseEndMigration(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $"Migration END. Result: (.*)");
            if (m.Success) {
                var result = new R2RPlusMigrationAnalysis(m);
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    theAnalysis.HadErrors = m.Groups[1].Value.ToUpper() != "TRUE";
                    theAnalysis.EndLineNumber = analysis.EndLineNumber;
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

        private bool tryParseBusinessUpdate(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $"Successfully updated business");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    theAnalysis.WasBusinessUpdated = true;
                }
            }
            return m.Success;
        }
        private bool tryParseInsertBookingFailure(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $@"Failed to insert booking \[(.*)\]: (.*)");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    var failedBooking = new InsertedRPlusData {
                        RId = m.Groups[1].Value,
                        IsOk = false,
                        LineNumber = analysis.StartLineNumber
                    };
                    if (!string.IsNullOrEmpty(_lastInsertedBookingError)) {
                        failedBooking.StatusMessage = $"Probable cause: {_lastInsertedBookingError}";
                        _lastInsertedBookingError = null;
                    }
                    else {
                        failedBooking.StatusMessage = m.Groups[2].Value;
                    }
                    theAnalysis.InsertedBookings.Add(failedBooking);
                }
            }
            return m.Success;
        }
        private bool tryParseInsertBookingSuccess(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $@"Successfully inserted booking with Id(.*)");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    theAnalysis.InsertedBookings.Add(new InsertedRPlusData {
                        RId = m.Groups[1].Value,
                        IsOk = true,
                        LineNumber = analysis.StartLineNumber
                    });
                }
            }
            return m.Success;
        }

        #endregion
    }
}
