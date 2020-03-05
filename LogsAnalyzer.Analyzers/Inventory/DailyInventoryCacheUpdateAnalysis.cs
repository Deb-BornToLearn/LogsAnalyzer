using LogAnalyzer.Infrastructure.Analysis;
using System.Collections.Generic;

namespace LogAnalyzer.Analyzers.Inventory {
    public class DailyInventoryCacheUpdateAnalysis : BaseAnalysisResult {
        public List<LogEntry> LogEntries;
        public string AccountId { get; set; }
        public string Reference { get; set; }

        public string NewStartDate { get; set; }
        public string NewEndDate { get; set; }
        public string NewInventoryIds { get; set; }
        
        public string OldStartDate { get; set; }
        public string OldEndDate { get; set; }
        public string OldInventoryIds { get; set; }

        public DailyInventoryCacheUpdateAnalysis() {
            LogEntries = new List<LogEntry>();
        }
    }

    public class LogEntry : BaseAnalysisResult { 
        public string LogText { get; set; }
    }
}
