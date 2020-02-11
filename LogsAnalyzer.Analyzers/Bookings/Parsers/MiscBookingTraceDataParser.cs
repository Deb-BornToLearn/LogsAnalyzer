using LogAnalyzer.Analyzers.Bookings.Models;
using LogAnalyzer.Infrastructure.Analysis;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Analyzers.Bookings.Parsers {
    public class MiscBookingTraceDataParser : IParser<BaseAnalysisResult> {

        private const string MISC_LOG_PATTERN = @"\[MTD:(.+?)\](.*)";
        private MiscellaneousTraceDataAnalysis  _output;

        public BaseAnalysisResult Output => _output;

        public bool Parse(string text) {
            string parsedMiscTraceData;
            string acctId;
            if (tryParseMiscellaneousTraceData(text, out acctId, out parsedMiscTraceData)) {
                _output = new MiscellaneousTraceDataAnalysis(acctId, parsedMiscTraceData);
                return true;
            }
            return false;
        }

        private bool tryParseMiscellaneousTraceData(string lineText, out string acctId, out string parsedMiscTraceData) {
            parsedMiscTraceData = lineText;
            acctId = string.Empty;
            var m = Regex.Match(lineText, MISC_LOG_PATTERN);
            if (m.Success && m.Groups.Count > 2) {
                // Check below needed to remove duplicate log entries;
                // much faster than doing via RegEx: .*^(?!.*Rezobx\+EASWebService).*\[MTD:.+?\](.*)
                //if (parsedMiscTraceData.Contains(@"Rezobx+EASWebService")) {
                //    return false;
                //}
                acctId = m.Groups[1].Value;
                parsedMiscTraceData = m.Groups[2].Value;
            }
            return m.Success;
        }
    }

 
}
