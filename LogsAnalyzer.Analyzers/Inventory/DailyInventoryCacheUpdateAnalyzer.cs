using LogsAnalyzer.Infrastructure.Analysis;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Analyzers.Inventory {
    public class DailyInventoryCacheUpdateAnalyzer : BaseLogAnalyzer {
        public List<DailyInventoryCacheUpdateAnalysis> DailyInventoryCacheUpdates { get; protected set; }

          private const string INVENTORY_UPDATE_START_PATTERN = 
            @"\[INV_LOG:(.*)\|(.*)?\] About to update.*OldStart: \[(.*)\].*OldEnd: \[(.*)\].*" +
            @"NewStart: \[(.*)\].*NewEnd: \[(.*)\].*Old Inventory: \[(.*)\].*New Inventory: \[(.*)\]";
        public DailyInventoryCacheUpdateAnalyzer() {
            DailyInventoryCacheUpdates = new List<DailyInventoryCacheUpdateAnalysis>();
        }

        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            if (tryParseInventoryUpdateStart(lineText, lineNumber, sourceName)) {
                return true;
            }
            else if (tryParseInventoryUpdateEnd(lineText, lineNumber, sourceName)) {
                return true;
            }
            else if (tryParseInventoryUpdateProgress(lineText, lineNumber, sourceName)) {
                return true;
            }
            return false;
        }

        private bool tryParseInventoryUpdateEnd(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, @"\[INV_LOG:(.*)\|(.*)?\] (Updated daily inventory cache for current values .*)");
            if (m.Success) {
                var dailyInventoryCacheUpdate = DailyInventoryCacheUpdates.LastOrDefault(d => d.AccountId == m.Groups[1].Value);
                if (dailyInventoryCacheUpdate != null) {
                    dailyInventoryCacheUpdate.EndLineNumber = lineNumber;
                    dailyInventoryCacheUpdate.LogEntries.Add(new LogEntry {
                        StartLineNumber = lineNumber,
                        EndLineNumber = lineNumber,
                        Source = sourceName,
                        LogText = m.Groups[3].Value
                    });
                }
            }
            return m.Success;
        }

        private bool tryParseInventoryUpdateProgress(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, @"\[INV_LOG:(.*)\|(.*)?\] (.*)");
            if (m.Success) {
                var dailyInventoryCacheUpdate = DailyInventoryCacheUpdates.LastOrDefault(d => d.AccountId == m.Groups[1].Value);
                if (dailyInventoryCacheUpdate != null) {
                    dailyInventoryCacheUpdate.LogEntries.Add(new LogEntry {
                        StartLineNumber = lineNumber,
                        EndLineNumber = lineNumber,
                        Source = sourceName,
                        LogText = m.Groups[3].Value
                    });
                }
            }
            return m.Success;
        }

      
        private bool tryParseInventoryUpdateStart(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, INVENTORY_UPDATE_START_PATTERN);
            if (m.Success) {
                var newDailyInventoryCacheUpdate = new DailyInventoryCacheUpdateAnalysis {
                    AccountId = m.Groups[1].Value,
                    Reference = m.Groups[2].Value,
                    OldStartDate = m.Groups[3].Value,
                    OldEndDate = m.Groups[4].Value,
                    NewStartDate = m.Groups[5].Value,
                    NewEndDate = m.Groups[6].Value,
                    OldInventoryIds = m.Groups[7].Value,
                    NewInventoryIds = m.Groups[8].Value,
                    StartLineNumber = lineNumber,
                    Source = sourceName
                };
                
                DailyInventoryCacheUpdates.Add(newDailyInventoryCacheUpdate);
            }
            return m.Success;
        }
    }
}
