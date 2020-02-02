using System;

namespace LogsAnalyzer.Infrastructure {
    public class StatusReport {
        public Type Source { get; set; }
        public string Message { get; set; }
        public ReportType ReportType { get; set; } 
    }

    public enum ReportType { 
        Info,
        Warning,
        Error
    }
}
