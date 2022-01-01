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

namespace Board.BusinessLogic.ViewModels.Main.Document
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

                    List<ColumnModel> columns = database.GetColumnsForTable(table.Id);
                    List<ColumnViewModel> columnViewModels = new();

                    foreach (var column in columns)
                    {
                        List<EntryModel> entries = database.GetEntriesForColumn(column.Id);

                        List<EntryViewModel> entryViewModels = entries.Select(e => new EntryViewModel(e)).ToList();

                        var columnViewModel = new ColumnViewModel(column, entryViewModels);
                        columnViewModels.Add(columnViewModel);
                    }

                    var tableViewModel = new TableViewModel(table, columnViewModels);

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

        // Private methods ----------------------------------------------------

        private DocumentViewModel()
        {
            isLoading = false;
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

        public ObservableCollection<TableViewModel> Tables => tables;

        public WallDocument Document => document;
    }
}
