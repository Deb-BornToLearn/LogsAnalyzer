using System.Collections.Generic;

namespace LogsAnalyzer.Infrastructure.Analysis {
    public interface ILogAnalyzer<T> {

        List<T> Results { get; }

        string AnalysesToString();
        void BeginReadAll();    
        void BeginRead(string sourceName);

        /// <summary>
        /// Return true to indicate input lineText was processed; otherwise, return false it was ignored.
        /// This is important to detect important log messages (ex. recurring fatal errors) that do not have analyzers
        /// so that analyzers can be created for them.
        /// </summary>
        /// <param name="lineText"></param>
        /// <param name="lineNumber"></param>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        bool Analyze(string lineText, long lineNumber, string sourceName);
        void EndRead(string sourceName);
        void EndReadAll();
    }
}
