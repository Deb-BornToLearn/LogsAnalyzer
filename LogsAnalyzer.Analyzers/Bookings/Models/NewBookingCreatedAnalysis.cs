using LogAnalyzer.Infrastructure.Analysis;

namespace LogAnalyzer.Analyzers.Bookings.Models {
    public class NewBookingCreatedAnalysis : BaseAnalysisResult {
        public readonly string ClientTransactionId;
        public readonly string AccountId;
        public readonly string CustomerFirstName;
        public readonly string CustomerLastName;
        public readonly MiscellaneousTraceDataAnalysis MiscTraceDataAnalysis;

        public NewBookingCreatedAnalysis(NewBookingInfo newBooking, MiscellaneousTraceDataAnalysis miscTraceDataAnalysis) {
            ClientTransactionId = newBooking.ClientTransactionId;
            AccountId = newBooking.AccountId;
            CustomerFirstName = newBooking.CustomerFirstName;
            CustomerLastName = newBooking.CustomerLastName;
            MiscTraceDataAnalysis = miscTraceDataAnalysis;
        }
    }
}
