namespace LogAnalyzer.Infrastructure.Analysis {
    public class BaseAnalysisResult {
        public long StartLineNumber;
        public long EndLineNumber;
        public string Source;
        public string Text;

        public override string ToString() {
            return $"Ln {StartLineNumber} {Text}";
        }
    }
}
