using LogAnalyzer.Infrastructure.Configuration;
using System.Collections.Generic;

namespace LogsAnalyzer.Infrastructure.Configuration {
    public interface IConfigurationSource {
        List<AnalyzerConfiguration> GetAnalyzerConfigurations();
        List<AnalyzerChainConfiguration> GetAnalyzerChainConfigurations();
    }
}
