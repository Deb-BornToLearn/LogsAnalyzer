using LogAnalyzer.Infrastructure.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogAnalyzer.Infrastructure {
    public class LogReader : ILogReader {
        public List<StatusReport> Reports { get; protected set; }
        public List<BaseLogAnalyzer> Analyzers { get; protected set; }
        public LogReader(List<BaseLogAnalyzer> analyzers) {
            if (analyzers == null || !analyzers.Any()) throw new NullOrEmptyAnalyzersException(); 

            Reports = new List<StatusReport>();
            Analyzers = analyzers;
            Analyzers.ForEach(a => beginReadAll(a));
        }

        public void EndReadAll() {
            Analyzers.ForEach(a => endReadAll(a));
        }

        public void ReadSource(string sourceName, Stream source) {
            Analyzers.ForEach(a => beginRead(a, sourceName));

            using (var lineReader = new StreamReader(source)) {
                int lineNumber = 1;
                while (lineReader.Peek() >= 0) {
                    var line = lineReader.ReadLine();
                    Analyzers.ForEach(a => analyze(a, line, lineNumber, sourceName));
                    lineNumber++;
                }
            }

            Analyzers.ForEach(a => endRead(a, sourceName));
        }


        private void beginReadAll(ILogAnalyzer a) {
            try {
                a.BeginReadAll();
            }
            catch (Exception exc) {
                Reports.Add(new StatusReport {
                    Source = a.GetType(),
                    Message = $"ILogAnalyzer {a.GetType()} threw an exception during BeginReadAll(). {exc.Message}",
                    ReportType = ReportType.Warning
                });
            }
        }
        private void endRead(ILogAnalyzer a, string sourceName) {
            try {
                a.EndRead(sourceName);
            }
            catch (Exception exc) {
                Reports.Add(new StatusReport {
                    Source = a.GetType(),
                    Message = $"ILogAnalyzer {a.GetType()} threw an exception during EndRead({sourceName}). {exc.Message}",
                    ReportType = ReportType.Warning
                });
            }
        }

        private void analyze(ILogAnalyzer a, string line, int lineNumber, string sourceName) {
            try {
                a.Analyze(line, lineNumber, sourceName);
            }
            catch {
                // Just suppress exception and do nothing; too costly to do anything here.
            }
        }

        private void beginRead(ILogAnalyzer a, string sourceName) {
            try {
                a.BeginRead(sourceName);
            }
            catch (Exception exc) {
                Reports.Add(new StatusReport {
                    Source = a.GetType(),
                    Message = $"ILogAnalyzer {a.GetType()} threw an exception during BeginRead({sourceName}). {exc.Message}",
                    ReportType = ReportType.Warning
                });
            }
        }

        private void endReadAll(ILogAnalyzer a) {
            try {
                a.EndReadAll();
            }
            catch (Exception exc) {
                Reports.Add(new StatusReport {
                    Source = a.GetType(),
                    Message = $"ILogAnalyzer {a.GetType()} threw an exception during EndReadAll(). {exc.Message}",
                    ReportType = ReportType.Warning
                });
            }
        }
    }

    public class NullOrEmptyAnalyzersException : ApplicationException { }
}
