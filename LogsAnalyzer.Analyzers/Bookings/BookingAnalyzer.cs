using LogAnalyzer.Analyzers.Bookings.Models;
using LogAnalyzer.Analyzers.Bookings.Parsers;
using LogAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Analyzers.Bookings.Parsers;
using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogAnalyzer.Analyzers.Bookings {
    public class BookingAnalyzer<T> : BaseLogAnalyzer<BookingAnalysis> {
        protected List<BookingAnalysis> Bookings = new List<BookingAnalysis>();
        public override List<BookingAnalysis> Results => Bookings;

        protected List<IParser<BaseAnalysisResult>> Parsers = new List<IParser<BaseAnalysisResult>>();

        protected Dictionary<Type, Action<BaseAnalysisResult, string, long, long, string>> OutputConsumers =
              new Dictionary<Type, Action<BaseAnalysisResult, string, long, long, string>>();

        public BookingAnalyzer() {
            Parsers.Add(new BookingParser());
            Parsers.Add(new NewBookingCreatedParser());
            Parsers.Add(new MiscBookingTraceDataParser());

            OutputConsumers.Add(typeof(BookingAnalysis), consumeBookingAnalysis);
            OutputConsumers.Add(typeof(NewBookingCreatedAnalysis), newBookingCreatedAnalysis);
            OutputConsumers.Add(typeof(MiscellaneousTraceDataAnalysis), miscTraceDataAnalysis);
        }

        private long _currentLineNumberStart = -1;


        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            bool lineProcessed = false;
            foreach (var parser in Parsers) {
                lineProcessed = parser.Parse(lineText);
                if (lineProcessed && _currentLineNumberStart == -1) {
                    _currentLineNumberStart = lineNumber;
                }
                if (lineProcessed) {
                    if (parser.Output != null) {
                        consumeParserOutput(parser.Output, lineText, _currentLineNumberStart, lineNumber, sourceName);
                        _currentLineNumberStart = -1;
                    }
                    break;
                }
            }
            return lineProcessed;
        }

        private void consumeParserOutput(BaseAnalysisResult output, string rawText,
                                        long currentLineNumberStart, long currentLineNumber, string sourceName) {
            if (output == null) return;

            var outputConsumer = OutputConsumers[output.GetType()];
            if (outputConsumer != null) {
                outputConsumer(output, rawText, currentLineNumberStart, currentLineNumber, sourceName);
            }
        }

        private void consumeBookingAnalysis(BaseAnalysisResult output, string rawText,
                                     long currentLineNumberStart, long currentLineNumber, string sourceName) {
            var booking = output as BookingAnalysis;
            if (booking != null) {
                if (!Bookings.Exists(b => b.TransactionId == booking.TransactionId)) {
                    booking.Source = sourceName;
                    booking.StartLineNumber = currentLineNumberStart;
                    booking.EndLineNumber = currentLineNumber;

                    Bookings.Add(booking);
                }
            }
        }

        private void newBookingCreatedAnalysis(BaseAnalysisResult output, string rawText,
                                     long currentLineNumberStart, long currentLineNumber, string sourceName) {
            var newAccountCreated = output as NewBookingCreatedAnalysis;
            if (newAccountCreated != null) {
                // Need logic below for times when TransactionId is not in the request.
                var theBooking = Bookings.FirstOrDefault(b => string.IsNullOrEmpty(b.TransactionId));
                if (theBooking == null) {
                    theBooking = Bookings.FirstOrDefault(b => b.TransactionId == newAccountCreated.ClientTransactionId);
                }
                else {
                    theBooking.TransactionId = newAccountCreated.ClientTransactionId;
                }
                theBooking.AccountId = newAccountCreated.AccountId;

                newAccountCreated.MiscTraceDataAnalysis.StartLineNumber = currentLineNumber;
                newAccountCreated.MiscTraceDataAnalysis.EndLineNumber = currentLineNumber;

                theBooking.MiscellaneousTraceData.Add(newAccountCreated.MiscTraceDataAnalysis);
            }
        }

        private void miscTraceDataAnalysis(BaseAnalysisResult output, string rawText,
                                     long currentLineNumberStart, long currentLineNumber, string sourceName) {
            var miscTraceData = output as MiscellaneousTraceDataAnalysis;
            if (miscTraceData != null) {
                miscTraceData.Source = sourceName;
                miscTraceData.StartLineNumber = currentLineNumber;
                miscTraceData.EndLineNumber = currentLineNumber;
                var theBooking = Bookings.FirstOrDefault(b => b.AccountId == miscTraceData.AccountId);
                theBooking?.MiscellaneousTraceData.Add(miscTraceData);
            }
        }

        public override string AnalysesToString() {
            if (!Bookings.Any()) return "No booking found";

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
