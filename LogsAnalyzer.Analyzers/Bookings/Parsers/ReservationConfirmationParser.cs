using LogAnalyzer.Analyzers.Bookings.Models;
using LogAnalyzer.Infrastructure.Analysis;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Analyzers.Bookings.Parsers {
    public class ReservationConfirmationParser : IParser<BaseAnalysisResult> {

        public BaseAnalysisResult Output => ReservationConfirmation;

        protected const string CONFIRM_RESERVATION_START_PATTERN = "ConfirmReservation.*Started";
        protected const string CONFIRM_RESERVATION_DATA_PATTERN = "\"Reference\":\"(.*?)\",\"ObxReference\":(.*?),.*\"Id\":\"(.*?)\",";
        private bool _foundConfirmReservationStart = false;
        private ReservationConfirmationAnalysis ReservationConfirmation;

        public bool Parse(string text) {
            if (_foundConfirmReservationStart) {
                var foundConfirmationData = tryParseReservationConfirmationData(text);
                if (foundConfirmationData) {
                    // Reset for any matches that follow
                    _foundConfirmReservationStart = false;
                }
                return foundConfirmationData;
            }
            else if (isReservationConfirmationStart(text)) {
                _foundConfirmReservationStart = true;
                return true;
            }
            return false;
        }

        private bool tryParseReservationConfirmationData(string text) {
            var m = Regex.Match(text, CONFIRM_RESERVATION_DATA_PATTERN);
            if (m.Success) {
                ReservationConfirmation = new ReservationConfirmationAnalysis(m.Groups[3].Value, 
                                                                              m.Groups[1].Value, 
                                                                              m.Groups[2].Value == "null" ? string.Empty : m.Groups[2].Value);
            }
            return m.Success;
        }

        private bool isReservationConfirmationStart(string text) {
            return Regex.IsMatch(text, CONFIRM_RESERVATION_START_PATTERN);
        }
    }


}
