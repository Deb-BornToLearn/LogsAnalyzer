using LogAnalyzer.Analyzers.Bookings.Models;
using LogAnalyzer.Infrastructure.Analysis;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Analyzers.Bookings.Parsers {
    public class NewBookingCreatedParser : IParser<BaseAnalysisResult> {
        public BaseAnalysisResult Output => _output;

        private NewBookingCreatedAnalysis _output;
        private MiscBookingTraceDataParser _miscBookingTraceDataParser = new MiscBookingTraceDataParser();

        public bool Parse(string text) {
            NewBookingInfo newAccount;
            MiscellaneousTraceDataAnalysis miscTraceDataAnalysis;
            if (tryParseNewAccountCreated(text, out newAccount, out miscTraceDataAnalysis)) {
                _output = new NewBookingCreatedAnalysis(newAccount, miscTraceDataAnalysis);
                return true;
            }
            return false;
        }

        private bool tryParseNewAccountCreated(string text, out NewBookingInfo newAccount, out MiscellaneousTraceDataAnalysis miscTraceDataAnalysis) {
            newAccount = null;
            miscTraceDataAnalysis = null;
            if (_miscBookingTraceDataParser.Parse(text)) {
                miscTraceDataAnalysis = _miscBookingTraceDataParser.Output as MiscellaneousTraceDataAnalysis;
                if (miscTraceDataAnalysis != null) {
                    if (tryCreateNewAccountFromTraceData(miscTraceDataAnalysis, out newAccount)) {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool tryCreateNewAccountFromTraceData(MiscellaneousTraceDataAnalysis miscTraceData, out NewBookingInfo newAccount) {
            newAccount = null;
            var ACCOUNT_CREATED_PATTERN = @"Created account.*customer.*\[(.+?),\s*(.+?)\].*TransactionId.*\[(.*)\]";
            var m = Regex.Match(miscTraceData.ParsedMiscTraceData, ACCOUNT_CREATED_PATTERN);
            if (m.Success && m.Groups.Count > 3) {
                newAccount = new NewBookingInfo {
                    AccountId = miscTraceData.AccountId,
                    CustomerFirstName = m.Groups[1].Value,
                    CustomerLastName = m.Groups[2].Value,
                    ClientTransactionId = m.Groups[3].Value
                };
            }
            return m.Success;
        }
    }
}
