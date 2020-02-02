using System.Collections.Generic;

namespace LogsAnalyzer.Infrastructure.Configuration {
    public class AnalyzerConfiguration {
        public Dictionary<string, string> ConstructorArgs { get; internal set; }
        public AnalyzerConfiguration() {
            ConstructorArgs = new Dictionary<string, string>();
        }
        public string TypeActivationName { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }

        public override string ToString() {
            return DisplayName ?? "<Empty display name>";
        }
    }

}
