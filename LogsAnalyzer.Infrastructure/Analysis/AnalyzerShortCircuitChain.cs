using LogsAnalyzer.Infrastructure.Analysis;
using System.Collections.Generic;

namespace LogAnalyzer.Infrastructure.Analysis {
    public class AnalyzerShortCircuitChain {
        public readonly string Name;
        public readonly List<ILogAnalyzer> Analyzers;
     
        public AnalyzerShortCircuitChain(string name) {
            Name = name;
            Analyzers = new List<ILogAnalyzer>();
        }
    }
}
