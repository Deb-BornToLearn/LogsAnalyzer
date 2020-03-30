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
        private InsertedRatePlan _lastInsertedRatePlan;

        private class InsertedRatePlan {
            public readonly string Name;
            public readonly string Code;
            public InsertedRatePlan(string name, string code) {
                Name = name;
                Code = code;
            }
        }

        public R2RPlusMigrationAnalyzer() {
            _logParsers.Add(tryParseStartMigration);
            _logParsers.Add(tryParseEndMigration);
            _logParsers.Add(tryParseBusinessUpdate);
            _logParsers.Add(tryParseInsertBooking);
            _logParsers.Add(tryParseInsertProduct);
            _logParsers.Add(tryParseInsertRatePlan);
            _logParsers.Add(tryParseInsertInactiveProduct);
        }
        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            tryParseBookingControllerPostLastError(lineText, lineNumber, sourceName);
            tryParseInsertedRatePlan(lineText, lineNumber, sourceName);

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

        private void tryParseInsertedRatePlan(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, "{\"ProductId\".*\"Name\":\"(.*?)\".*\"Code\":\"(.*?)\"");
            if (m.Success) {
                _lastInsertedRatePlan = new InsertedRatePlan(m.Groups[1].Value, m.Groups[2].Value);
            }
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
                analysis.EndLineNumber = lineNumber;
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
        private bool tryParseInsertBooking(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $@"Insert result of R booking \[(.*?)\] via \[(.*?)\]: \[(.*?)\] (.*)");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    var isOk = m.Groups[3].Value.ToUpper() == "TRUE";
                    var insertedBooking = new InsertedRPlusData {
                        RId = m.Groups[1].Value,
                        RPlusId = m.Groups[2].Value,
                        IsOk = isOk,
                        LineNumber = analysis.StartLineNumber
                    };
                    if (!isOk && !string.IsNullOrEmpty(_lastInsertedBookingError)) {
                        insertedBooking.StatusMessage = $"Probable cause: {_lastInsertedBookingError}";
                        _lastInsertedBookingError = null;
                    }
                    else {
                        insertedBooking.StatusMessage = m.Groups[4].Value;
                    }
                    theAnalysis.InsertedBookings.Add(insertedBooking);
                }
            }
            return m.Success;
        }

        private bool tryParseInsertProduct(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $@"Insert result of R Product \[(.*?)\|(.*?)\] via \[(.*?)\]: \[(.*?)\] (.*)");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    var isOk = m.Groups[4].Value.ToUpper() == "TRUE";
                    var inserted = new InsertedRPlusData {
                        RId = m.Groups[1].Value,
                        Name = m.Groups[2].Value,
                        RPlusId = m.Groups[3].Value,
                        IsOk = isOk,
                        LineNumber = analysis.StartLineNumber
                    };
                    inserted.StatusMessage = m.Groups[5].Value;
                    theAnalysis.InsertedProducts.Add(inserted);
                }
            }
            return m.Success;
        }

         private bool tryParseInsertInactiveProduct(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $@"Created R\+ inactive product \[(.*?)\] for.*?\[(.*?)\((.*)\)\].*\[(.*?)\]");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    var inserted = new InsertedInactiveProduct { 
                        RId = m.Groups[4].Value,
                        RPlusId = m.Groups[1].Value,
                        RatePlanId = m.Groups[3].Value,
                        RatePlanName = m.Groups[2].Value.Trim(),
                        LineNumber = analysis.StartLineNumber
                    };
                    theAnalysis.InsertedInactiveProducts.Add(inserted);
                }
            }
            return m.Success;
        }

        private bool tryParseInsertRatePlan(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $@"Insert result of Rate Plan \[(.*?)\] with Id \[(.*?)\]: \[(.*?)\] (.*)");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    var isOk = m.Groups[3].Value.ToUpper() == "TRUE";
                    var insertedData = new InsertedRPlusData {
                        RId = m.Groups[1].Value,
                        RPlusId = m.Groups[2].Value,
                        IsOk = isOk,
                        LineNumber = analysis.StartLineNumber
                    };
                    insertedData.StatusMessage = m.Groups[4].Value;
                    if (_lastInsertedRatePlan != null) {
                        insertedData.Name = $"{_lastInsertedRatePlan.Name} ({_lastInsertedRatePlan.Code})";
                    }
                    theAnalysis.InsertedRatePlans.Add(insertedData);
                }
            }
            return m.Success;
        }

        #endregion
    }
}
