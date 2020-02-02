using System.Collections.Generic;

namespace LogsAnalyzer.Infrastructure.Configuration {
    public interface IConfigurationSource {
        List<AnalyzerConfiguration> GetAnalyzerConfigurations();
    }
}
