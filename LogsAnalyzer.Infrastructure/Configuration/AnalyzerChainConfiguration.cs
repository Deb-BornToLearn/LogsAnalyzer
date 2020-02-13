using LogsAnalyzer.Infrastructure.Configuration;
using System;
using System.Collections.Generic;

namespace LogAnalyzer.Infrastructure.Configuration {
    public class AnalyzerChainConfiguration {
        public string DisplayName { get; set; }
        public List<AnalyzerConfiguration> AnalyzerConfigurations { get; protected set; }
        public AnalyzerChainConfiguration() {
            AnalyzerConfigurations = new List<AnalyzerConfiguration>();
        }
    }
}
