using LogAnalyzer.Infrastructure.Analysis;

namespace LogAnalyzer.Analyzers.Errors.UnhandledErrors {
    public class ErrorAnalysis : BaseAnalysisResult {
        public readonly string ErrorMessage;
        public ErrorAnalysis(string errorMsg) {
            ErrorMessage = errorMsg;
        }
    }
}
