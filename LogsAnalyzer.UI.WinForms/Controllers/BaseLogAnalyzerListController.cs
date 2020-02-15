using LogAnalyzer.Infrastructure.Configuration;
using LogsAnalyzer.Infrastructure.Configuration;
using System;
using System.Collections.Generic;

namespace LogAnalyzer.UI.WinForms.Controllers {
    public abstract class BaseLogAnalyzerListController<T> {
        protected T ListView { get; }
        public BaseLogAnalyzerListController(T listView) {
            ListView = listView;
        }

        internal abstract void AddAnalyzers(List<AnalyzerConfiguration> analyzerConfigs);
        internal abstract void AddAnalyzerChains(List<AnalyzerChainConfiguration> analyzerConfigs);
        internal abstract bool IsAnyAnalyzerSelected();
        internal abstract AnalysisArgs BuildAnalysisArgs();
        internal abstract List<AnalyzerConfiguration> GetSelectedAnalyzerConfigurations();
    }
}
