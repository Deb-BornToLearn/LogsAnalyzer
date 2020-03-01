using LogAnalyzer.Infrastructure.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogAnalyzer.Analyzers.Bookings.Models {
    public class BookingAnalysis : BaseAnalysisResult {

        public ReservationConfirmationAnalysis Confirmation;

        public List<MiscellaneousTraceDataAnalysis> MiscellaneousTraceData;
        public string RawXml { get; set; }
        public BookingAnalysis() {
            Extras = new List<Extra>();
            MiscellaneousTraceData = new List<MiscellaneousTraceDataAnalysis>();
        }
        public string AccountId;

        public string TransactionId;
        public string Timestamp;
        public string AmountPaid;

        public string DistributorShortName;
        public string ProductId;
        public string ProductName;
        public string ChannelCommission;
        public string PaymentOption;

        public string StartDate;
        public string EndDate;

        public string StartDateUTC {
            get {
                return utcDatetimeToString(StartDate);
            }
        }

        public string EndDateUTC {
            get {
                return utcDatetimeToString(EndDate);
            }
        }

        public string PrimaryProvider;
        public int ProviderCount;

        public string ProductTotal;
        public string ExtrasTotal;

        public string CustomerFirstName;
        public string CustomerLastName;

        public List<Extra> Extras;

        private string utcDatetimeToString(string utcDateTime) {
            if (string.IsNullOrWhiteSpace(utcDateTime)) return string.Empty;

            return DateTimeOffset.Parse(utcDateTime).ToString("dd MMM, yyyy HH:mm:ss");
        }
        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine($"Transaction ID: {TransactionId}");
            sb.AppendLine($"Timestamp: {utcDatetimeToString(Timestamp)}");
            sb.AppendLine($"Account Id: {AccountId}");
            sb.AppendLine($"Customer: {CustomerLastName}, {CustomerFirstName}");
            sb.AppendLine($"Distributor: {DistributorShortName}");
            sb.AppendLine($"Provider: {PrimaryProvider}");
            sb.AppendLine($"Commences: {utcDatetimeToString(StartDate)}, Concludes: {utcDatetimeToString(EndDate)}");
            sb.AppendLine($"Payment Amount: {AmountPaid}");
            sb.AppendLine($"Payment Option: {PaymentOption}");
            sb.AppendLine($"Commission: {ChannelCommission}");
            sb.AppendLine($"{ProductName} ({ProductId})");
            sb.AppendLine($"Products: {ProductTotal}, Extras: {ExtrasTotal}");
            sb.AppendLine($"Source: {Source}");
            sb.AppendLine($"Lines {StartLineNumber} to {EndLineNumber}");
            foreach (var e in Extras) {
                sb.AppendLine($"{e.Name} ({e.Code})");
                sb.AppendLine($"\tAdult Price: {e.AdultPrice}, Per Adult Price: {e.PerAdultPrice}, Per Adult Price Per Night {e.PerAdultPricePerNight}");
                sb.AppendLine($"\tChild Price: {e.ChildPrice}, Per Child Price: {e.PerChildPrice}, Per Child Price Per Night {e.PerChildPricePerNight}");
                sb.AppendLine($"\tIsPerNight: {e.IsPerNight}, IsAllowOccupancySelect: {e.IsAllowOccupancySelect}, IsNightsSelectable: {e.IsNightsSelectable}");
                foreach (var night in e.SelectedNights) {
                    sb.AppendLine($"\t\tSelected Night: {night.SelectedNight}");
                }
            }

            if (MiscellaneousTraceData.Any()) {
                sb.AppendLine("");
                sb.AppendLine("*** Miscellaneous Trace Data ***");
            }

            foreach (var mtd in MiscellaneousTraceData) {
                sb.AppendLine($"Ln {mtd.StartLineNumber}:  {mtd.ParsedMiscTraceData}");
            }
           
            return sb.ToString();
        }
    }

}
