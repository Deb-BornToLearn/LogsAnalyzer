using System.Collections.Generic;

namespace LogAnalyzer.Infrastructure.Configuration {
    public interface IConfigurationSource {
        List<AnalyzerConfiguration> GetAnalyzerConfigurations();
    }
}
