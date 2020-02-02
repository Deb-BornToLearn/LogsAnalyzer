using LogAnalyzer.Analyzers.Bookings.Models;
using LogAnalyzer.Infrastructure.Analysis;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogAnalyzer.Analyzers.Bookings {
    public class BookingAnalyzer : BaseLogAnalyzer {
        public List<BookingAnalysis> Bookings = new List<BookingAnalysis>();
        protected BookingParser Parser;

        public BookingAnalyzer() {
            Parser = new BookingParser();
        }

        private long _currentLineNumberStart = -1;
        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            var lineProcessed = Parser.Accept(lineText);
            if (lineProcessed && _currentLineNumberStart == -1) {
                _currentLineNumberStart = lineNumber;
            }
            if (Parser.BookingAnalysis != null) {
                Parser.BookingAnalysis.Source = sourceName;
                Parser.BookingAnalysis.StartLineNumber = _currentLineNumberStart;
                Parser.BookingAnalysis.EndLineNumber = lineNumber;
                Bookings.Add(Parser.BookingAnalysis);
                _currentLineNumberStart = -1;
            }
            return lineProcessed;
        }

        public override string AnalysesToString() {
            var sb = new StringBuilder();
            var chronoSortedBookings = Bookings.OrderBy(d => d.Timestamp);
            foreach (var booking in chronoSortedBookings) {
                sb.AppendLine("BOOKING");
                sb.Append(booking.ToString());
                sb.AppendLine("===============================================================================");
            }
            return sb.ToString();
        }
    }



}
