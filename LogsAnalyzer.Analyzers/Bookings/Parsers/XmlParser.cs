using System.Text;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Analyzers.Bookings.Parsers {
    public class XmlParser {
        public string RootElementTagName { get; protected set; }
        public string OutputXmlString { get; protected set; }

        private StringBuilder _buffer = null;

        private readonly string _selfClosingRootElementPattern;
        private readonly string _openCloseTagSameOnSameLinePattern;

        public XmlParser(string rootElementTagName) {
            RootElementTagName = rootElementTagName;
            _selfClosingRootElementPattern = $"<{rootElementTagName}[^>]*\\s*/>";
            _openCloseTagSameOnSameLinePattern = $"<{rootElementTagName}>.*?</{rootElementTagName}>";
        }

        public bool Parse(string input, out bool done) {
            done = false;
            string parseOutput;
            if (tryParseXmlOnSameLine(input, out parseOutput)) {
                OutputXmlString = parseOutput;
                done = true;
                return true;
            }
            else if (isStartTag(input)) {
                _buffer = new StringBuilder();
                _buffer.AppendLine(input);
                return true;
            }
            else if (isEndTag(input)) {
                _buffer.AppendLine(input);
                OutputXmlString = _buffer.ToString();
                done = true;
                return true;
            }
            else {
                if (_buffer != null) {
                    _buffer.AppendLine(input);
                    return true;
                }
            }
            return false;
        }
        public void Reset() {
            _buffer = null;
            OutputXmlString = string.Empty;
        }

        private bool isEndTag(string input) {
            return Regex.Match(input, $"</{RootElementTagName}>").Success;
        }

        private bool isStartTag(string input) {
            return Regex.Match(input, $"<{RootElementTagName}.*>").Success;
        }

        private bool tryParseXmlOnSameLine(string lineText, out string output) {
            output = string.Empty;
            var m = Regex.Match(lineText, _selfClosingRootElementPattern);
            if (m.Success) {
                output = m.Groups[0].Value;
                return true;
            }

            m = Regex.Match(lineText, _openCloseTagSameOnSameLinePattern);
            if (m.Success) {
                output = m.Groups[0].Value;
            }
            return m.Success;
        }


    }
}
