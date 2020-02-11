using LogAnalyzer.Infrastructure.Analysis;

namespace LogAnalyzer.Analyzers.Bookings.Models {
    public class MiscellaneousTraceDataAnalysis : BaseAnalysisResult {
        public readonly string ParsedMiscTraceData;
        public readonly string AccountId;
        public MiscellaneousTraceDataAnalysis(string accountId, string parsedMiscTraceData) {
            AccountId = accountId;
            ParsedMiscTraceData = parsedMiscTraceData;
        }
        
    }
}
