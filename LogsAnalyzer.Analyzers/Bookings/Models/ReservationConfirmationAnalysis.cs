using LogAnalyzer.Infrastructure.Analysis;

namespace LogAnalyzer.Analyzers.Bookings.Models {
    public class ReservationConfirmationAnalysis : BaseAnalysisResult {
        public string Reference { get; protected set; }
        public string ObxReference { get; protected set; }
        public string AccountId { get; protected set; }
        public ReservationConfirmationAnalysis(string accountId, string reference, string obxRef) {
            AccountId = accountId;
            Reference = reference;
            ObxReference = obxRef;
        }

    }
}
