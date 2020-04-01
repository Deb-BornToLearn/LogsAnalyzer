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
        public readonly List<InsertedRPlusData> InsertedProducts = new List<InsertedRPlusData>();
        public readonly List<InsertedRPlusData> InsertedInventory = new List<InsertedRPlusData>();
        public readonly List<InsertedRPlusData> InsertedProductInventory = new List<InsertedRPlusData>();
        public readonly List<InsertedRPlusData> InsertedRatePlans = new List<InsertedRPlusData>();
        public readonly List<InsertedRPlusData> InsertedAccommodationBookingExtras = new List<InsertedRPlusData>();
        public readonly List<InsertedRPlusData> InsertedAccommodationRatePlanBookingExtras = new List<InsertedRPlusData>();
        public readonly List<InsertedRPlusData> InsertedBookings = new List<InsertedRPlusData>();
        public readonly List<InsertedInactiveProduct> InsertedInactiveProducts = new List<InsertedInactiveProduct>();

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
        public long LineNumber { get; set; }
    }
    public class InsertedRPlusData {
        public string RId { get; set; }
        public string RPlusId { get; set; }
        public string Name { get; set; }
        public bool IsOk { get; set; }
        public string StatusMessage { get; set; }
        public long LineNumber { get; set; }

    }

    public class InsertedInactiveProduct {
        public string RId { get; set; }
        public string RPlusId { get; set; }
        public string RatePlanName { get; set; }
        public string RatePlanId { get; set; }
        public long LineNumber { get; set; }

    }

}
