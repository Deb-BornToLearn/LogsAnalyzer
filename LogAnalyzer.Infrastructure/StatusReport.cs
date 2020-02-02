using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Infrastructure {
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
