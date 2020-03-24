using LogAnalyzer.Infrastructure.Analysis;
using System.Collections.Generic;

namespace LogAnalyzer.Analyzers.Inventory {
    public class ClosedForCheckoutAnalysis : BaseAnalysisResult {
        public string InventoryId { get; set; }
        public string Date { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ClosedForCheckoutResult { get; set; }
        public List<DailyRatePlanLine> DailyRatePlanLines { get; set; }

        public ClosedForCheckoutAnalysis() {
            DailyRatePlanLines = new List<DailyRatePlanLine>();
        }
    }

    public class DailyRatePlanLine {
        public string Id { get; set; }
        public string ClosedForCheckout { get; set; }
    }
}
