using LogAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Infrastructure.Analysis;
using System.Collections.Generic;
using System.IO;

namespace LogsAnalyzer.Infrastructure {
    public interface ILogReader {
        List<BaseLogAnalyzer<BaseAnalysisResult>> Analyzers { get; }
        void ReadSource(string sourceName, Stream source);
        void EndReadAll();
    }
}
