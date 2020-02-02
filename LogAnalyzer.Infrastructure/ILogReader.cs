using LogAnalyzer.Infrastructure.Analysis;
using System.Collections.Generic;
using System.IO;

namespace LogAnalyzer.Infrastructure {
    public interface ILogReader {
        List<BaseLogAnalyzer> Analyzers { get; }
        void ReadSource(string sourceName, Stream source);
        void EndReadAll();
    }
}
