using LogAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogsAnalyzer.Infrastructure.Factory {
    public class AnalyzersBuilder{
        public List<AnalyzerConfiguration> AnalyzerConfigurations { get; protected set; }

        public AnalyzersBuilder(List<AnalyzerConfiguration> analyzerConfiguration) {
            AnalyzerConfigurations = analyzerConfiguration;
        }

        public List<BaseLogAnalyzer<BaseAnalysisResult>> BuildAnalyzers() {
            var analyzers = new List<BaseLogAnalyzer<BaseAnalysisResult>>();
            foreach (var config in AnalyzerConfigurations) {
                var args = config.ConstructorArgs.Select(a => a.Value).ToArray();
                var typeName = config.TypeActivationName;
                var fte = new FullTypeNameEntry(typeName);
                //var analyzer = TypeFactory.CreateInstance<BaseLogAnalyzer<BaseAnalysisResult>>(new FullTypeNameEntry(typeName), args);
                var analyzer = Activator.CreateInstance(fte.AssemblyName, fte.TypeName);
                var x  = analyzer.Unwrap() as BaseLogAnalyzer<BaseAnalysisResult>;
                analyzers.Add(x);
            }
            return analyzers;
        }
    }
}
