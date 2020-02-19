using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LogAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Infrastructure;
using LogsAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Infrastructure.Factory;
using LogsAnalyzer.Tests.Mocks.LogAnalyzers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogsAnalyzer.Tests {
    [TestClass]
    public class LogReaderTests {
        [TestMethod]
        public void itShouldCallBeginReadAllOfAnalyzersBeforeReadingLogs() {
            var mockAnalyzer = new MockAnalyzer();
            var analyzers = new List<BaseLogAnalyzer>();
            analyzers.Add(mockAnalyzer);
            var logReader = new LogReader(analyzers);

            using (var stream = toStream("This is a test")) {
                logReader.ReadSource("source1", stream);

                // Implied assertion: MockAnalyzer should not raise exception at this point
            }
        }

        [TestMethod]
        public void itShouldCallEndReadAllOfAnalyzersAfterReadingAllLogs() {
            var mockAnalyzer = new MockAnalyzer();
            var analyzers = new List<BaseLogAnalyzer>();
            analyzers.Add(mockAnalyzer);
            var logReader = new LogReader(analyzers);

            using (var stream = toStream("This is a test")) {
                logReader.ReadSource("source1", stream);
            }
            logReader.EndReadAll();
            mockAnalyzer.Dispose();

            // Implied assertion: MockAnalyzer should not raise exception at this point
        }

        [TestMethod]
        public void itShouldCallBeginReadOfAnalyzersBeforeAnalyzing() {
            var mockAnalyzer = new MockAnalyzer();
            var analyzers = new List<BaseLogAnalyzer>();
            analyzers.Add(mockAnalyzer);
            var logReader = new LogReader(analyzers);

            using (var stream = toStream("This is a test")) {
                logReader.ReadSource("source1", stream);
            }

            // Implied assertion: MockAnalyzer should not raise exception at this point
        }

        [TestMethod]
        public void itShouldCallEndReadOfAnalyzersAfterReadingSource() {
            var mockAnalyzer = new MockAnalyzer();
            var analyzers = new List<BaseLogAnalyzer>();
            analyzers.Add(mockAnalyzer);
            var logReader = new LogReader(analyzers);

            using (var stream = toStream("This is a test")) {
                logReader.ReadSource("source1", stream);
            }
            logReader.EndReadAll();
            // Implied assertion: MockAnalyzer should not raise exception at this point
        }

        [TestMethod]
        public void itShouldReadAllLinesFromSourceAndPassCorrectLineNumberToAnalyzers() {
            var mockAnalyzer = new MockAnalyzer();
            var analyzers = new List<BaseLogAnalyzer>();
            analyzers.Add(mockAnalyzer);
            var logReader = new LogReader(analyzers);

            var sb = new StringBuilder();
            sb.AppendLine("This is line 1");
            sb.AppendLine("This is line 2");
            sb.AppendLine("This is line 3");

            using (var stream = toStream(sb.ToString())) {
                logReader.ReadSource("source1", stream);
            }
            logReader.EndReadAll();

            Assert.AreEqual(3, mockAnalyzer.LinesCount);
            Assert.AreEqual(3, mockAnalyzer.LastLineNumberFromReader);
        }

        [TestMethod]
        public void itShouldSupportMultipleAnalyzers() {
            var mockAnalyzer1 = new MockAnalyzer();
            var mockAnalyzer2 = new MockAnalyzer();

            var analyzers = new List<BaseLogAnalyzer>();
            analyzers.Add(mockAnalyzer1);
            analyzers.Add(mockAnalyzer2);

            var logReader = new LogReader(analyzers);

            var sb = new StringBuilder();
            sb.AppendLine("This is line 1");
            sb.AppendLine("This is line 2");
            sb.AppendLine("This is line 3");

            using (var stream = toStream(sb.ToString())) {
                logReader.ReadSource("source1", stream);
            }
            logReader.EndReadAll();

            Assert.AreEqual(3, mockAnalyzer1.LinesCount);
            Assert.AreEqual(3, mockAnalyzer1.LastLineNumberFromReader);

            Assert.AreEqual(3, mockAnalyzer2.LinesCount);
            Assert.AreEqual(3, mockAnalyzer2.LastLineNumberFromReader);
        }

        [TestMethod]
        public void itShouldCallEndReadSourceOfAnalyzersBeforeCallingBeginReadOfNewSource() {
            var mockAnalyzer = new MockAnalyzer();
            var analyzers = new List<BaseLogAnalyzer>();
            analyzers.Add(mockAnalyzer);
            var logReader = new LogReader(analyzers);

            var sb = new StringBuilder();
            sb.AppendLine("This is line 1");
            sb.AppendLine("This is line 2");
            sb.AppendLine("This is line 3");

            using (var stream = toStream(sb.ToString())) {
                logReader.ReadSource("source1", stream);
            }

            using (var stream = toStream(sb.ToString())) {
                logReader.ReadSource("source2", stream);
            }

        }

        [TestMethod]
        public void itShouldHandleExceptionInBeginReadAll() {
            var mockAnalyzer = new MockAnalyzerThrowingException();
            var analyzers = new List<BaseLogAnalyzer>();
            analyzers.Add(mockAnalyzer);
            var logReader = new LogReader(analyzers);

            using (var stream = toStream("This is a test")) {
                logReader.ReadSource("source1", stream);
            }

            // Implied assertion: No unhandled exception at this point.
        }

        [TestMethod]
        public void itShouldHandleExceptionInBeginRead() {
            var mockAnalyzer = new MockAnalyzerThrowingException();
            var analyzers = new List<BaseLogAnalyzer>();
            analyzers.Add(mockAnalyzer);
            var logReader = new LogReader(analyzers);

            using (var stream = toStream("This is a test")) {
                logReader.ReadSource("source1", stream);
            }

            // Implied assertion: No unhandled exception at this point.
        }

        [TestMethod]
        public void itShouldHandleExceptionInAnalyze() {
            var mockAnalyzer = new MockAnalyzerThrowingException();
            var analyzers = new List<BaseLogAnalyzer>();
            analyzers.Add(mockAnalyzer);
            var logReader = new LogReader(analyzers);

            using (var stream = toStream("This is a test")) {
                logReader.ReadSource("source1", stream);
            }

            // Implied assertion: No unhandled exception at this point.
        }

        [TestMethod]
        public void itShouldHandleExceptionInEndReadAll() {
            var mockAnalyzer = new MockAnalyzerThrowingException();
            var analyzers = new List<BaseLogAnalyzer>();
            analyzers.Add(mockAnalyzer);
            var logReader = new LogReader(analyzers);

            using (var stream = toStream("This is a test")) {
                logReader.ReadSource("source1", stream);
                logReader.EndReadAll();
            }

            // Implied assertion: No unhandled exception at this point.
        }

        [TestMethod]
        public void reflectionTest() {
            var typeName = "LogAnalyzer.Analyzers, LogAnalyzer.Analyzers.Bookings.BookingAnalyzer`1[LogAnalyzer.Analyzers.Bookings.Models.BookingAnalysis]";
            var fte = new FullTypeNameEntry(typeName);
            try {
                var y = Activator.CreateInstance(fte.AssemblyName, fte.TypeName);
            }
            catch (Exception exc) {
                System.Diagnostics.Debug.Print(exc.Message);
            }

        }

        #region private
        private Stream toStream(string input) {
            return new MemoryStream(Encoding.UTF8.GetBytes(input ?? ""));
        }
        #endregion
    }
}
