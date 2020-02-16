using System;
using System.Linq;

namespace LogAnalyzer.Analyzers.Errors.UnhandledErrors {
    public class UnhandledErrorsAnalyzer<T> : GenericErrorAnalyzer<T> {
        public override string NoErrorFoundMessage => "No unhandled errors found";

        public override string AnalysesToString() {
            if (!Errors.Any()) return base.AnalysesToString();

            return $"UNHANDLED ERRORS: {Errors.Count}, starting at line {Errors.Min(e => e.StartLineNumber)}{Environment.NewLine}" + base.AnalysesToString();
        }
    }
}
