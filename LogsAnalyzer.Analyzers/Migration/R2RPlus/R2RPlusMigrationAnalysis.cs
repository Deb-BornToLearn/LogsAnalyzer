using LogAnalyzer.Infrastructure.Analysis;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Analyzers.Migration.R2RPlus {
    public class R2RPlusMigrationAnalysis : BaseAnalysisResult {
        public string LogId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string RBusinessId { get; set; }
        public string RPlusBusinessId { get; set; }
        public bool WasBusinessUpdated { get; set; }
        public bool HadErrors { get; set; }

        public readonly List<DeletedRPlusData> DeletedAccountBookingExtras = new List<DeletedRPlusData>();
        public readonly List<DeletedRPlusData> DeletedStatementPaymentLines = new List<DeletedRPlusData>();
        public readonly List<DeletedRPlusData> DeletedAccommodationItems = new List<DeletedRPlusData>();
        public readonly List<DeletedRPlusData> DeletedStatementChargeLines = new List<DeletedRPlusData>();
        public readonly List<DeletedRPlusData> DeletedAccounts = new List<DeletedRPlusData>();
        public readonly List<DeletedRPlusData> DeletedRatePlanLines = new List<DeletedRPlusData>();
        public readonly List<DeletedRPlusData> DeletedAccommodationInventories = new List<DeletedRPlusData>();
        public readonly List<DeletedRPlusData> DeletedAccommodationInventoryDailyCache = new List<DeletedRPlusData>();
        public readonly List<InsertedData> InsertedProducts = new List<InsertedData>();
        public readonly List<InsertedData> InsertedInventory = new List<InsertedData>();
        public readonly List<InsertedData> InsertedProductInventory = new List<InsertedData>();
        public readonly List<InsertedData> InsertedRatePlans = new List<InsertedData>();
        public readonly List<InsertedData> InsertedAccommodationBookingExtras = new List<InsertedData>();
        public readonly List<InsertedData> InsertedAccommodationRatePlanBookingExtras = new List<InsertedData>();
        public readonly List<InsertedData> InsertedBookings = new List<InsertedData>();

        public R2RPlusMigrationAnalysis(Match regExMatch) {
            LogId = regExMatch.Groups[5].Value;
            RBusinessId = regExMatch.Groups[2].Value;
            RPlusBusinessId = regExMatch.Groups[1].Value;
            StartDate = regExMatch.Groups[3].Value;
            EndDate = regExMatch.Groups[4].Value;
        }
    }
    public class DeletedRPlusData {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class InsertedData {
        public string RId { get; set; }
        public string RPlusId { get; set; }
        public string Name { get; set; }
        public bool IsOk { get; set; }
        public string StatusMessage { get; set; }
    }
}
