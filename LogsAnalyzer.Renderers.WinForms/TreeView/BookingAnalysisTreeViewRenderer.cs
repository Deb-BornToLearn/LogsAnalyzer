using LogAnalyzer.Analyzers.Bookings;
using LogAnalyzer.Analyzers.Bookings.Models;
using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Linq;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms.TreeView {
    public class BookingAnalysisTreeViewRenderer : BaseTreeViewRenderer {

        protected BookingAnalyzer Analyzer;

        public override Type AnalyzerType => typeof(BookingAnalyzer);

        public BookingAnalysisTreeViewRenderer() {
            Analyzer = new BookingAnalyzer();
        }

        public override TreeNode Render() {
            var rootBookingAnalyzer = new TreeNode {
                Text = $"Booking Analyzer Results ({Analyzer.Bookings.Count})"
            };

            foreach (var booking in Analyzer.Bookings) {
                var bookingNode = new TreeNode();
                bookingNode.Text = $"{booking.CustomerLastName}, {booking.CustomerFirstName}";
                ContextMenuStrips.Add(bookingNode, createContextMenuForBookingNode(bookingNode, booking));

                bookingNode.Nodes.Add(CreateNode($"Account Id: {booking.AccountId}"));
                bookingNode.Nodes.Add(CreateNode($"Distributor: {booking.DistributorShortName}"));
                bookingNode.Nodes.Add(CreateNode($"Provider: {booking.PrimaryProvider}"));
                bookingNode.Nodes.Add(CreateNode($"Commences: {booking.StartDateUTC}, Concludes: {booking.EndDateUTC}"));
                bookingNode.Nodes.Add(CreateNode($"Payment Amount: {booking.AmountPaid}"));
                bookingNode.Nodes.Add(CreateNode($"Payment Option: {booking.PaymentOption}"));
                bookingNode.Nodes.Add(CreateNode($"Channel Commission: {booking.ChannelCommission}"));
                bookingNode.Nodes.Add(CreateNode($"{booking.ProductName} ({booking.ProductId})"));
                bookingNode.Nodes.Add(CreateNode($"Products: {booking.ProductTotal}, Extras: {booking.ExtrasTotal}"));
                var logFileNode = CreateNode($"Source: {booking.Source}");
                ContextMenuStrips.Add(logFileNode, CreateContextMenuItemForLogFile(logFileNode, booking.Source));
                bookingNode.Nodes.Add(logFileNode);
                bookingNode.Nodes.Add(CreateNode($"Lines {booking.StartLineNumber} to {booking.EndLineNumber}"));

                if (booking.MiscellaneousTraceData.Any()) {
                    var mtdRootNode = CreateNode($"Miscellaneous trace data");
                    bookingNode.Nodes.Add(mtdRootNode);
                    foreach (var mtd in booking.MiscellaneousTraceData) {
                        var mtdNode = CreateNode($"Ln {mtd.StartLineNumber} {mtd.ParsedMiscTraceData}");
                        var contextMenu = CreateContextMenuItemForLogFile(mtdNode, mtd.Source);
                        ContextMenuStrips.Add(mtdNode, contextMenu);
                        mtdRootNode.Nodes.Add(mtdNode);
                    }
                }

                rootBookingAnalyzer.Nodes.Add(bookingNode);
            }
            return rootBookingAnalyzer;
        }

        private ContextMenuStrip CreateContextMenuItemForLogFile(TreeNode node, string filename) {
            var contextMenuStrip = CreateCommonContextMenuStrip(node);
            contextMenuStrip.Items.Add(CreateOpenFileInNotepadPlusMenuItem("Open log file in Notepad++", filename));
            return contextMenuStrip;
        }

        private ContextMenuStrip createContextMenuForBookingNode(TreeNode node, BookingAnalysis booking) {
            var contextMenuStrip = CreateCommonContextMenuStrip(node);
            contextMenuStrip.Items.Add(CreateCopyToClipboardMenuItem("Copy raw XML to Clipboard", booking.RawXml));
            return contextMenuStrip;
        }

        public override void SetAnalyzer(BaseLogAnalyzer analyzer) {
            Analyzer = analyzer as BookingAnalyzer;
        }
    }
}
