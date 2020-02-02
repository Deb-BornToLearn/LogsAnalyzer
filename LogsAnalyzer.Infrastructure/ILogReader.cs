using LogsAnalyzer.Infrastructure.Analysis;
using System.Collections.Generic;
using System.IO;

namespace LogsAnalyzer.Infrastructure {
    public interface ILogReader {
        List<BaseLogAnalyzer> Analyzers { get; }
        void ReadSource(string sourceName, Stream source);
        void EndReadAll();
    }
}
