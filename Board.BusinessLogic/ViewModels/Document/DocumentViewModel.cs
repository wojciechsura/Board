using Board.BusinessLogic.Infrastructure.Document;
using Board.BusinessLogic.Infrastructure.Document.Database;
using Board.BusinessLogic.Models.Data;
using Board.BusinessLogic.Models.Document;
using Board.BusinessLogic.Services.Document;
using Board.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class DocumentViewModel : BaseViewModel
    {
        // Private types ------------------------------------------------------

        private class TableProgress
        {
            public TableProgress(TableViewModel table)
            {
                Table = table;
            }

            public TableViewModel Table { get; }
        }

        private class TableLoadingWorker : BackgroundWorker
        {
            private BaseDatabase database;

            public TableLoadingWorker(BaseDatabase database)
            {
                this.database = database;
                WorkerSupportsCancellation = true;
                WorkerReportsProgress = true;
            }

            protected override void OnDoWork(DoWorkEventArgs e)
            {
                // Load all tables
                List<TableModel> tables = database.GetTables();

                foreach (var table in tables)
                {
                    if (CancellationPending)
                        return;
                    TableViewModel tableViewModel = BuildTableViewModel(table, database);

                    // TODO: load columns and entities

                    ReportProgress(0, new TableProgress(tableViewModel));
                }
            }
        }

        // Private fields -----------------------------------------------------

        private readonly WallDocument document;

        private readonly IDocumentFactory documentFactory;

        private TableLoadingWorker loadingWorker;
        private bool isLoading;

        private readonly ObservableCollection<TableViewModel> tables = new();
        private TableViewModel activeTable = null;

        // Private methods ----------------------------------------------------

        private DocumentViewModel()
        {
            isLoading = false;
        }

        private static TableViewModel BuildTableViewModel(TableModel table, BaseDatabase database)
        {
            List<ColumnModel> columns = database.GetColumnsForTable(table.Id);
            List<ColumnViewModel> columnViewModels = new();

            foreach (var column in columns)
            {
                ColumnViewModel columnViewModel = BuildColumnViewModel(column, database);
                columnViewModels.Add(columnViewModel);
            }

            var tableViewModel = new TableViewModel(table, columnViewModels);
            return tableViewModel;
        }

        private static ColumnViewModel BuildColumnViewModel(ColumnModel column, BaseDatabase database)
        {
            List<EntryModel> entries = database.GetEntriesForColumn(column.Id);

            List<EntryViewModel> entryViewModels = entries.Select(e => BuildEntryViewModel(e, database)).ToList();

            var columnViewModel = new ColumnViewModel(column, entryViewModels);
            return columnViewModel;
        }

        private static EntryViewModel BuildEntryViewModel(EntryModel entry, BaseDatabase database)
        {
            return new EntryViewModel(entry);
        }

        private void LoadTables()
        {
            tables.Clear();

            if (loadingWorker != null)
            {
                loadingWorker.CancelAsync();
                loadingWorker = null;
            }

            IsLoading = true;

            loadingWorker = new TableLoadingWorker(document.Database);
            loadingWorker.ProgressChanged += HandleLoadingWorkerProgress;
            loadingWorker.RunWorkerCompleted += HandleLoadingWorkerCompleted;
            loadingWorker.RunWorkerAsync();
        }

        private void HandleLoadingWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsLoading = false;
            loadingWorker = null;
        }

        private void HandleLoadingWorkerProgress(object sender, ProgressChangedEventArgs e)
        {
            var tableProgress = (TableProgress)e.UserState;
            tables.Add(tableProgress.Table);
        }

        // Public methods -----------------------------------------------------

        /// <summary>
        /// Use this ctor for creating new document from prepared document info
        /// </summary>
        public DocumentViewModel(IDocumentFactory documentFactory, DocumentInfo info)
            : this()
        {
            this.documentFactory = documentFactory;

            documentFactory.SaveDefinition(info);
            document = documentFactory.Create(info);

            LoadTables();
        }

        /// <summary>
        /// Use this ctor for opening existing document from given definition path
        /// </summary>
        public DocumentViewModel(IDocumentFactory documentFactory, string definitionPath)
            : this()
        {
            this.documentFactory = documentFactory;

            var documentInfo = documentFactory.OpenDefinition(definitionPath);
            document = documentFactory.Open(documentInfo);

            LoadTables();
        }

        public bool CanClose()
        {
            return true;
        }

        public void Close()
        {

        }

        public bool IsLoading
        {
            get => isLoading;
            set => Set(ref isLoading, value);
        }

        public void AddTableFromModel(TableModel newTable)
        {
            document.Database.AddTable(newTable);

            var tableViewModel = BuildTableViewModel(newTable, document.Database);
            tables.Add(tableViewModel);

            if (ActiveTable == null)
                ActiveTable = tableViewModel;
        }

        public void UpdateTableFromModel(TableViewModel tableViewModel, TableModel updatedTable)
        {
            document.Database.UpdateTable(updatedTable);

            var newTableViewModel = BuildTableViewModel(updatedTable, document.Database);

            int index = tables.IndexOf(tableViewModel);
            tables[index] = newTableViewModel;

            ActiveTable = newTableViewModel;
        }

        public void DeleteTable(TableViewModel tableViewModel, bool permanent)
        {
            document.Database.DeleteTable(tableViewModel.Table, permanent);
            int index = tables.IndexOf(tableViewModel);

            tables.RemoveAt(index);
            index = Math.Max(0, Math.Min(index, tables.Count - 1));

            if (tables.Count > 0)
                ActiveTable = tables[index];
        }

        // Public properties --------------------------------------------------

        public ObservableCollection<TableViewModel> Tables => tables;

        public WallDocument Document => document;

        public TableViewModel ActiveTable
        {
            get => activeTable;
            set => Set(ref activeTable, value);
        }
    }
}
