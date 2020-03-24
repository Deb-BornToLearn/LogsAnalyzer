using LogsAnalyzer.Infrastructure.Analysis;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Analyzers.Inventory {
    public class ClosedForCheckoutAnalyzer : BaseLogAnalyzer {
        public List<ClosedForCheckoutAnalysis> ClosedForCheckoutResults { get; set; }
        public ClosedForCheckoutAnalyzer() {
            ClosedForCheckoutResults = new List<ClosedForCheckoutAnalysis>();
        }
        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            if (tryParseStartLog(lineText, lineNumber, sourceName)) {
                return true;
            }
            else if (tryParseDailyRatePlanLineLog(lineText, lineNumber, sourceName)) {
                return true;
            }
            else if (tryParseClosedForCheckoutResult(lineText, lineNumber, sourceName)) {
                return true;
            }
            return false;
        }

        private bool tryParseStartLog(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, @"\[CLCH:(.*)\|(.*)\].*prodInventory:(.*)\|(.*)");
            if (m.Success) {
                var result = new ClosedForCheckoutAnalysis {
                    InventoryId = m.Groups[1].Value,
                    Date = m.Groups[2].Value,
                    ProductId = m.Groups[3].Value,
                    ProductName = m.Groups[4].Value,
                    StartLineNumber = lineNumber,
                    Source = sourceName
                };
                ClosedForCheckoutResults.Add(result);
            }
            return m.Success;
        }
        private bool tryParseDailyRatePlanLineLog(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, @"\[CLCH:(.*)\|(.*?)\].*:(.*)\|ClosedForCheckout:(.*)");
            if (m.Success) {
                var inventoryId = m.Groups[1].Value;
                var date = m.Groups[2].Value;
                var result = ClosedForCheckoutResults.LastOrDefault(r => r.InventoryId == inventoryId && r.Date == date);
                result.DailyRatePlanLines.Add(new DailyRatePlanLine {
                    Id = m.Groups[3].Value,
                    ClosedForCheckout = m.Groups[4].Value
                });
            }
            return m.Success;
        }

        private bool tryParseClosedForCheckoutResult(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, @"\[CLCH:(.*)\|(.*?)\].*ClosedForCheckout result:(.*)");
            if (m.Success) {
                var inventoryId = m.Groups[1].Value;
                var date = m.Groups[2].Value;
                var result = ClosedForCheckoutResults.LastOrDefault(r => r.InventoryId == inventoryId && r.Date == date);
                result.ClosedForCheckoutResult = m.Groups[3].Value;
                result.EndLineNumber = lineNumber;
            }
            return m.Success;
        }
    }
}
