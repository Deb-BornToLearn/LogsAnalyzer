using System;
using System.Collections.Generic;
using System.Xml;
using LogAnalyzer.Infrastructure.Configuration;

namespace LogsAnalyzer.Infrastructure.Configuration {
    public class AnalyzerConfigurationXmlSource : IConfigurationSource {
        private readonly XmlNode _rootConfigNode;

        public AnalyzerConfigurationXmlSource(XmlNode rootConfigNode) {
            _rootConfigNode = rootConfigNode;
        }

        public List<AnalyzerChainConfiguration> GetAnalyzerChainConfigurations() {
            var analyzerChainConfigurations = new List<AnalyzerChainConfiguration>();
            var analyzerChainNodes = _rootConfigNode.SelectNodes("//analyzerShortCircuitChain");
            foreach (XmlNode analyzerChainNode in analyzerChainNodes) {
                var analyzerChainConfig = new AnalyzerChainConfiguration();
                analyzerChainConfig.DisplayName = analyzerChainNode.Attributes["displayName"].Value;
                var analyzerNodes = analyzerChainNode.SelectNodes("descendant::analyzer");
                foreach (XmlNode analyzerNode in analyzerNodes) {
                    var analyzerConfig = createAnalyzerConfig(analyzerNode);
                    analyzerChainConfig.AnalyzerConfigurations.Add(analyzerConfig);
                }
                analyzerChainConfigurations.Add(analyzerChainConfig);
            }
            return analyzerChainConfigurations;
        }

        public List<AnalyzerConfiguration> GetAnalyzerConfigurations() {
            var analyzerConfigs = new List<AnalyzerConfiguration>();
            var analyzerNodes = _rootConfigNode.SelectNodes("//analyzers/analyzer");
            foreach (XmlNode analyzerNode in analyzerNodes) {
                var analyzerConfig = createAnalyzerConfig(analyzerNode);
                analyzerConfigs.Add(analyzerConfig);
            }
            return analyzerConfigs;
        }

        private AnalyzerConfiguration createAnalyzerConfig(XmlNode analyzerNode) {
            var analyzerConfig = new AnalyzerConfiguration();
            setTypeActivationName(analyzerConfig, analyzerNode);
            setDisplayName(analyzerConfig, analyzerNode);
            setCtorArgs(analyzerConfig, analyzerNode);
            return analyzerConfig;
        }

        private void setCtorArgs(AnalyzerConfiguration analyzerConfig, XmlNode analyzerNode) {
            var ctorArgs = analyzerNode.SelectNodes("ctorArgs/ctorArg");
            foreach (XmlNode ctorArgNode in ctorArgs) {
                var ctorArgValueAttribute = ctorArgNode.Attributes["value"];
                if (ctorArgValueAttribute == null) throw new CtorValueAttributeNotFoundException();

                var ctorArgNameAttribute = ctorArgNode.Attributes["name"];
                var ctorArgName = ctorArgNameAttribute != null ? ctorArgNameAttribute.Value : Guid.NewGuid().ToString();

                if (analyzerConfig.ConstructorArgs.ContainsKey(ctorArgName)) {
                    throw new DuplicateCtorArgNameException(ctorArgName);
                }
                analyzerConfig.ConstructorArgs.Add(ctorArgName, ctorArgValueAttribute.Value);
            }
        }

        private void setDisplayName(AnalyzerConfiguration analyzerConfig, XmlNode analyzerNode) {
            XmlNode typeNameNode = getChildNode(analyzerNode, "displayName");
            analyzerConfig.DisplayName = typeNameNode == null ? string.Empty : typeNameNode.InnerText;
        }

        private void setTypeActivationName(AnalyzerConfiguration analyzerConfig, XmlNode analyzerNode) {
            XmlNode typeNameNode = getChildNode(analyzerNode, "typeName", new TypeNameNodeNotFoundException());
            analyzerConfig.TypeActivationName = typeNameNode.InnerText;
        }

        private XmlNode getChildNode(XmlNode sourceNode, string nodeName, Exception exToThrowIfNotFound = null) {
            XmlNode requestedNode = sourceNode.SelectSingleNode(nodeName);
            if (requestedNode == null && exToThrowIfNotFound != null) {
                throw exToThrowIfNotFound;
            }
            return requestedNode;
        }

     
    }

    public class TypeNameNodeNotFoundException : ApplicationException {

    }

    public class CtorValueAttributeNotFoundException : ArgumentException {
        public CtorValueAttributeNotFoundException()
            : base("ctorArg value attribute not found") {

        }
    }

    public class DuplicateCtorArgNameException : ArgumentException {
        public DuplicateCtorArgNameException(string ctorArgName)
            : base($"Duplicate ctorArg name {ctorArgName} found in configuration") {

        }
    }
}
