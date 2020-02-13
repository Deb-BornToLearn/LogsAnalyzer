using LogAnalyzer.Infrastructure.Configuration;
using LogsAnalyzer.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.UI.WinForms {
    public class AnalysisArgs {
        public List<AnalyzerConfiguration> AnalyzerConfigurations { get; protected set; }
        public List<AnalyzerChainConfiguration> AnalyzerChainConfigurations { get; protected set; }
        public AnalysisArgs() {
            AnalyzerConfigurations = new List<AnalyzerConfiguration>();
            AnalyzerChainConfigurations = new List<AnalyzerChainConfiguration>();
        }
    }
}
