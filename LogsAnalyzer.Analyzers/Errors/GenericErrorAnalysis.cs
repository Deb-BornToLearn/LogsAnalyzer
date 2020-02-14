using LogAnalyzer.Infrastructure.Analysis;

namespace LogAnalyzer.Analyzers.Errors {
    public class GenericErrorAnalysis : BaseAnalysisResult {
        public readonly string ErrorMessage;
        public GenericErrorAnalysis(string errorMsg) {
            ErrorMessage = errorMsg;
        }
    }
}
