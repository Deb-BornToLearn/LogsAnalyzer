using LogAnalyzer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LogAnalyzer.UI.WinForms.Controllers {
    public abstract class BaseLogSourceListController<T> {
        protected T ListView { get; }

        public BaseLogSourceListController(T listView) {
            ListView = listView;
        }

        public abstract void AddFile(string filename);
        internal abstract bool HasFile();
        internal abstract bool HasSelectedFile();
        internal abstract void RemoveSelectedItems();
        internal abstract LogSourceDefinition GetSelectedLogSources();
        internal abstract void RemoveAllItems();
        internal abstract void AddFolder(string folder);
    }

    public enum ItemType {
        File,
        Folder
    }
}
