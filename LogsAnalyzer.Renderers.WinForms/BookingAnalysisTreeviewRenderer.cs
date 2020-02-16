using LogAnalyzer.Analyzers.Bookings.Models;
using System;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms {
    public class BookingAnalysisTreeViewRenderer {
        public readonly BookingAnalysis Booking;
        public BookingAnalysisTreeViewRenderer(BookingAnalysis bookingAnalysis) {
            Booking = bookingAnalysis;
        }
        
        public TreeNode Render() {
            var rootNode = new TreeNode();
            rootNode.Text = $"{Booking.CustomerLastName}, {Booking.CustomerFirstName}";
            rootNode.Nodes.Add(createNode($"Distributor: {Booking.DistributorShortName}"));
            rootNode.Nodes.Add(createNode($"Account Id: {Booking.AccountId}"));
            rootNode.Nodes.Add(createNode($"Provider: {Booking.PrimaryProvider}"));
            rootNode.Nodes.Add(createNode($"Commences: {Booking.StartDateUTC}, Concludes: {Booking.EndDateUTC}"));
            return rootNode;
        }

        private TreeNode createNode(string text) {
            return new TreeNode { 
                Text = text
            };
        }
    }
}
