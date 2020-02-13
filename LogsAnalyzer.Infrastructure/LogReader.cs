using LogAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogsAnalyzer.Infrastructure {
    public class LogReader : ILogReader {

        public delegate void ReadProgressEventHandler(LogReader reader, ReadProgressEventArgs args);
        public event ReadProgressEventHandler OnReadProgress;

        public List<StatusReport> Reports { get; protected set; }
        public List<BaseLogAnalyzer> Analyzers { get; protected set; }
        public List<AnalyzerShortCircuitChain> AnalyzerShortCircuitChains { get; protected set; }

        public LogReader() {
            Reports = new List<StatusReport>();
            Analyzers = new List<BaseLogAnalyzer>();
            AnalyzerShortCircuitChains = new List<AnalyzerShortCircuitChain>();
        }

        public LogReader(List<BaseLogAnalyzer> analyzers) : this() {
            if (analyzers == null) throw new NullAnalyzersException();

            Analyzers = analyzers;
            beginReadAll();
        }

        public LogReader(List<AnalyzerShortCircuitChain> analyzerShortCircuitChains) : this() {
            if (analyzerShortCircuitChains == null) throw new NullAnalyzerShortCircuitChainException();

            AnalyzerShortCircuitChains = analyzerShortCircuitChains;
            beginReadAll();
        }

        public LogReader(List<BaseLogAnalyzer> analyzers, List<AnalyzerShortCircuitChain> analyzerShortCircuitChains) : this() {
            if (analyzers == null) throw new NullAnalyzersException();
            if (analyzerShortCircuitChains == null) throw new NullAnalyzerShortCircuitChainException();

            Analyzers = analyzers;
            AnalyzerShortCircuitChains = analyzerShortCircuitChains;
            beginReadAll();
        }


        private void beginReadAll() {
            Analyzers.ForEach(a => beginReadAll(a));
            AnalyzerShortCircuitChains.ForEach(c => c.Analyzers.ForEach(a => beginReadAll(a)));
        }


        public void EndReadAll() {
            Analyzers.ForEach(a => endReadAll(a));
            AnalyzerShortCircuitChains.ForEach(c => c.Analyzers.ForEach(a => endReadAll(a)));
        }

        public void ReadSource(string sourceName, Stream source) {
            Analyzers.ForEach(a => beginRead(a, sourceName));
            AnalyzerShortCircuitChains.ForEach(c => c.Analyzers.ForEach(a => beginRead(a, sourceName)));

            using (var lineReader = new StreamReader(source)) {
                int lineNumber = 1;
                while (lineReader.Peek() >= 0) {
                    OnReadProgress?.Invoke(this, new ReadProgressEventArgs(sourceName, lineNumber));
                    var line = lineReader.ReadLine();
                    Analyzers.ForEach(a => analyze(a, line, lineNumber, sourceName));
                    
                    foreach (var analyzerChain in AnalyzerShortCircuitChains) { 
                        foreach(var analyzer in analyzerChain.Analyzers) {
                            if (analyze(analyzer, line, lineNumber, sourceName)){
                                continue;
                            }
                        }
                    }

                    lineNumber++;
                }
            }

            Analyzers.ForEach(a => endRead(a, sourceName));
            AnalyzerShortCircuitChains.ForEach(c => c.Analyzers.ForEach(a => endRead(a, sourceName)));
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

        private bool analyze(ILogAnalyzer a, string line, int lineNumber, string sourceName) {
            try {
                return a.Analyze(line, lineNumber, sourceName);
            }
            catch {
                // Just suppress exception and do nothing; too costly to do anything here.
                return false;
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

    public class NullAnalyzersException : ApplicationException { }
    public class NullAnalyzerShortCircuitChainException : ApplicationException { }

    public class ReadProgressEventArgs : EventArgs {
        public readonly string SourceName;
        public readonly long LineNumber;
        public ReadProgressEventArgs(string sourceNamme, int lineNumber) {
            SourceName = sourceNamme;
            LineNumber = lineNumber;
        }

    }
}
