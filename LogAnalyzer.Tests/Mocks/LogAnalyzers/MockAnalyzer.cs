using LogAnalyzer.Infrastructure;
using LogAnalyzer.Infrastructure.Analysis;
using System;
using System.Collections.Generic;

namespace LogAnalyzer.Tests.Mocks.LogAnalyzers {
    public class MockAnalyzer : BaseLogAnalyzer, IDisposable {
        private bool _beginReadAllCalled = false;
        private bool _endReadAllCalled = false;

        private bool _beginReadSourceCalled = false;
        private bool _endReadSourceCalled = false;

        private List<String> _sourcesRead = new List<string>();

        private string _lastSourceRead;
        public int LinesCount { get; private set; }
        public long LastLineNumberFromReader { get; private set; }
        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            if (!_beginReadSourceCalled) {
                throw new InvalidOperationException("Analyze called without invoking BeginRead(source) of MockAnalyzer first");
            }
            LinesCount++;
            LastLineNumberFromReader = lineNumber;
            return true;
        }

        public override void BeginRead(string sourceName) {
            if (!_beginReadAllCalled) {
                throw new InvalidOperationException("BeginRead of MockAnalyzer called without invoking BeginReadAll first");
            }
            if (!string.IsNullOrEmpty(_lastSourceRead) && (sourceName != _lastSourceRead)) {
                if (!_sourcesRead.Contains(_lastSourceRead)){
                    throw new InvalidOperationException($"EndRead(source) not called for {_lastSourceRead} before initiating BeginRead(source) for {sourceName}");
                }
            }
            _beginReadSourceCalled = true;
            _lastSourceRead = sourceName;
        }

        public override void BeginReadAll() {
            _beginReadAllCalled = true;
        }

        public override void EndRead(string sourceName) {
            _endReadSourceCalled = true;
            _sourcesRead.Add(sourceName);
        }

        public override void EndReadAll() {
            if (!_endReadSourceCalled) {
                throw new InvalidOperationException("EndReadAll of MockAnalyzer called without invoking EndRead first");
            }
            _endReadAllCalled = true;
        }

        public void Dispose() {
            if (!_endReadAllCalled) {
                throw new InvalidOperationException("EndReadAll of MockAnalyzer not called");
            }
        }

    }
}
