using LogAnalyzer.Infrastructure.Analysis;
using LogAnalyzer.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogAnalyzer.Infrastructure.Factory {
    public class AnalyzersBuilder{
        public List<AnalyzerConfiguration> AnalyzerConfigurations { get; protected set; }

        public AnalyzersBuilder(List<AnalyzerConfiguration> analyzerConfiguration) {
            AnalyzerConfigurations = analyzerConfiguration;
        }

        public List<BaseLogAnalyzer> BuildAnalyzers() {
            var analyzers = new List<BaseLogAnalyzer>();
            foreach (var config in AnalyzerConfigurations) {
                var args = config.ConstructorArgs.Select(a => a.Value).ToArray();
                var typeName = config.TypeActivationName;
                var analyzer = TypeFactory.CreateInstance<BaseLogAnalyzer>(new FullTypeNameEntry(typeName), args);
                analyzers.Add(analyzer);
            }
            return analyzers;
        }
    }
}
