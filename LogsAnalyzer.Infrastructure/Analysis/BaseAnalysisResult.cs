using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Infrastructure.Analysis {
    public class BaseAnalysisResult {
        public long StartLineNumber;
        public long EndLineNumber;
        public string Source;
    }
}
