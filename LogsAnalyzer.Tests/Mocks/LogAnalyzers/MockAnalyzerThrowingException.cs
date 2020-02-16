using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Collections.Generic;

namespace LogsAnalyzer.Tests.Mocks.LogAnalyzers {
    public class MockAnalyzerThrowingException<T> : BaseLogAnalyzer<T> {
        public override List<T> Results => throw new NotImplementedException();

        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            throw new Exception("Deliberate exception in Analyze that should be handled gracefully");
        }

        public override void BeginRead(string sourceName) {
            throw new Exception("Deliberate exception in BeginRead that should be handled gracefully");
        }

        public override void BeginReadAll() {
            throw new Exception("Deliberate exception in BeginReadAll that should be handled gracefully");
        }

        public override void EndRead(string sourceName) {
            throw new Exception("Deliberate exception in EndRead that should be handled gracefully");
        }

        public override void EndReadAll() {
            throw new Exception("Deliberate exception in EndReadAll that should be handled gracefully");
        }

    }
}
