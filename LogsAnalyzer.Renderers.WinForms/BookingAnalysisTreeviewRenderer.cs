using LogAnalyzer.Analyzers.Bookings;
using System.Linq;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms {
    public class BookingAnalysisTreeViewRenderer : BaseTreeViewRenderer<BookingAnalyzer> {
        public override TreeNode Render() {
            var rootBookingAnalyzer = new TreeNode {
                Text = $"Booking Analyzer Results ({Analyzer.Bookings.Count})"
            };

            foreach (var booking in Analyzer.Bookings) {
                var bookingNode = new TreeNode();
                bookingNode.Text = $"{booking.CustomerLastName}, {booking.CustomerFirstName}";
                bookingNode.Nodes.Add(CreateNode($"Account Id: {booking.AccountId}"));
                bookingNode.Nodes.Add(CreateNode($"Distributor: {booking.DistributorShortName}"));
                bookingNode.Nodes.Add(CreateNode($"Provider: {booking.PrimaryProvider}"));
                bookingNode.Nodes.Add(CreateNode($"Commences: {booking.StartDateUTC}, Concludes: {booking.EndDateUTC}"));
                bookingNode.Nodes.Add(CreateNode($"Payment Amount: {booking.AmountPaid}"));
                bookingNode.Nodes.Add(CreateNode($"Payment Option: {booking.PaymentOption}"));
                bookingNode.Nodes.Add(CreateNode($"Channel Commission: {booking.ChannelCommission}"));
                bookingNode.Nodes.Add(CreateNode($"{booking.ProductName} ({booking.ProductId})"));
                bookingNode.Nodes.Add(CreateNode($"Products: {booking.ProductTotal}, Extras: {booking.ExtrasTotal}"));
                bookingNode.Nodes.Add(CreateNode($"Source: {booking.Source}"));
                bookingNode.Nodes.Add(CreateNode($"Lines {booking.StartLineNumber} to {booking.EndLineNumber}"));

                if (booking.MiscellaneousTraceData.Any()) {
                    var mtdRootNode = CreateNode($"Miscellaneous trace data");
                    bookingNode.Nodes.Add(mtdRootNode);
                    foreach (var mtd in booking.MiscellaneousTraceData) {
                        mtdRootNode.Nodes.Add(CreateNode($"Ln {mtd.StartLineNumber} {mtd.ParsedMiscTraceData}"));
                    }
                }
                
                rootBookingAnalyzer.Nodes.Add(bookingNode);
            }
            return rootBookingAnalyzer;
        }
    }
}
