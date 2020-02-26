using LogAnalyzer.Infrastructure;

namespace LogAnalyzer.UI.WinForms.Controllers {
    public abstract class BaseLogSourceListController<T> {
        protected T ListView { get; }

        public BaseLogSourceListController(T listView) {
            ListView = listView;
        }

        public abstract void AddFile(string filename);
        internal abstract void AddFolder(string folder, bool addFiles);
        internal abstract void AddLogSourceDefinition(LogSourceDefinition logDefinition);
        internal abstract bool HasAny();
        internal abstract bool HasSelectedAny();
        internal abstract void RemoveSelectedItems();
        internal abstract void RemoveAllItems();
        internal abstract LogSourceDefinition BuildLogSourceDefinition();
        internal abstract LogSourceDefinition BuildLogSourceDefinitionFromSelection();
        internal abstract bool IsSelectedItemFolder();
    }

    public enum ItemType {
        File,
        Folder
    }
}
