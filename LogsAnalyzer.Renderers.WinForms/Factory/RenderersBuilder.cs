using LogsAnalyzer.Infrastructure.Analysis;
using LogsAnalyzer.Infrastructure.Factory;
using LogsAnalyzer.Renderers.WinForms.TreeView;
using System;
using System.Collections.Generic;
using System.Xml;

namespace LogsAnalyzer.Renderers.WinForms.Factory {
    public class RenderersBuilder {
        protected string ConfigFile { get; set; }
        public RenderersBuilder(string configFile) {
            ConfigFile = configFile;
        }

        public List<BaseTreeViewRenderer> BuildRenderers() {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(ConfigFile);
            return buildRenderers(xmlDoc);
        }

        private List<BaseTreeViewRenderer> buildRenderers(XmlDocument xmlDoc) {
            List<BaseTreeViewRenderer> renderers = new List<BaseTreeViewRenderer>();
            var rendererNodes = xmlDoc.SelectNodes("//renderers/renderer/typeName");
            foreach (XmlNode node in rendererNodes) {
                var r = TypeFactory.CreateInstance<BaseTreeViewRenderer>(new FullTypeNameEntry(node.InnerText), new object[] { });
                renderers.Add(r);
            }
            return renderers ;
        }
    }
}
