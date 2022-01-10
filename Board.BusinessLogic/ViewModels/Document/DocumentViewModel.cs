using Board.BusinessLogic.Infrastructure.Document;
using Board.BusinessLogic.Infrastructure.Document.Database;
using Board.Models.Data;
using Board.Models.Document;
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
using Board.BusinessLogic.Infrastructure.EntityOrdering;
using AutoMapper;
using Board.Models.Types;

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
            private readonly BaseDatabase database;
            private readonly IDocumentHandler handler;            

            public TableLoadingWorker(BaseDatabase database, IDocumentHandler handler)
            {
                this.database = database;
                this.handler = handler;
                WorkerSupportsCancellation = true;
                WorkerReportsProgress = true;
            }

            protected override void OnDoWork(DoWorkEventArgs e)
            {
                // Load all tables
                List<TableModel> tables = database.GetTables(false);

                foreach (var table in tables)
                {
                    if (CancellationPending)
                        return;
                    TableViewModel tableViewModel = BuildTableViewModel(table, database, handler);

                    // TODO: load columns and entities

                    ReportProgress(0, new TableProgress(tableViewModel));
                }
            }
        }

        private class EntryOrdering : BaseEntityOrdering<EntryModel>
        {
            private readonly BaseDatabase database;

            protected override long GetFirstOrderValue(int groupId)
            {
                return database.GetFirstEntryOrder(groupId, true);
            }

            protected override long GetLastOrderValue(int groupId)
            {
                return database.GetLastEntryOrder(groupId, true);
            }

            protected override int GetModelCount(int groupId)
            {
                return database.GetEntryCount(groupId, false);
            }

            protected override List<EntryModel> GetOrderedModels(int groupId, int skip, int take)
            {
                return database.GetEntries(groupId, skip, take, true);
            }

            protected override void UpdateItems(int groupId, List<EntryModel> updatedItems)
            {
                database.UpdateEntries(updatedItems);
            }

            protected override (EntryModel indexModel, EntryModel nextModel) GetModelWithSuccessor(int groupId, int index)
            {
                var indexModel = database.GetEntries(groupId, index, 1, false)[0];
                var nextModel = database.GetNextEntry(indexModel, true);

                return (indexModel, nextModel);
            }

            public EntryOrdering(BaseDatabase database)
            {
                this.database = database;
            }
        }

        private class ColumnOrdering : BaseEntityOrdering<ColumnModel>
        {
            private readonly BaseDatabase database;

            protected override long GetFirstOrderValue(int groupId)
            {
                return database.GetFirstColumnOrder(groupId, true);
            }

            protected override long GetLastOrderValue(int groupId)
            {
                return database.GetLastColumnOrder(groupId, true);
            }

            protected override int GetModelCount(int groupId)
            {
                return database.GetColumnCount(groupId, false);
            }

            protected override List<ColumnModel> GetOrderedModels(int groupId, int skip, int take)
            {
                return database.GetColumns(groupId, skip, take, true);
            }

            protected override void UpdateItems(int groupId, List<ColumnModel> updatedItems)
            {
                database.UpdateColumns(updatedItems);
            }

            protected override (ColumnModel indexModel, ColumnModel nextModel) GetModelWithSuccessor(int groupId, int index)
            {
                var indexModel = database.GetColumns(groupId, index, 1, false)[0];
                var nextModel = database.GetNextColumn(indexModel, true);

                return (indexModel, nextModel);
            }

            public ColumnOrdering(BaseDatabase database)
            {
                this.database = database;
            }
        }

        private class TableOrdering : BaseEntityOrdering<TableModel>
        {
            private readonly BaseDatabase database;

            protected override long GetFirstOrderValue(int groupId)
            {
                return database.GetFirstTableOrder(true);
            }

            protected override long GetLastOrderValue(int groupId)
            {
                return database.GetLastTableOrder(true);
            }

            protected override int GetModelCount(int groupId)
            {
                return database.GetTableCount(false);
            }

            protected override List<TableModel> GetOrderedModels(int groupId, int skip, int take)
            {
                return database.GetTables(skip, take, true);
            }

            protected override void UpdateItems(int groupId, List<TableModel> updatedItems)
            {
                database.UpdateTables(updatedItems);
            }

            protected override (TableModel indexModel, TableModel nextModel) GetModelWithSuccessor(int groupId, int index)
            {
                var indexModel = database.GetTables(index, 1, false)[0];
                var nextModel = database.GetNextTable(indexModel, true);

                return (indexModel, nextModel);
            }

            public TableOrdering(BaseDatabase database)
            {
                this.database = database;
            }
        }

        // Private fields -----------------------------------------------------

        private readonly WallDocument document;

        private readonly EntryOrdering entryOrdering;
        private readonly ColumnOrdering columnOrdering;
        private readonly TableOrdering tableOrdering;

        private readonly IDocumentFactory documentFactory;
        private readonly IDocumentHandler handler;
        private readonly IMapper mapper;

        private TableLoadingWorker loadingWorker;
        private bool isLoading;

        private readonly ObservableCollection<TableViewModel> tables = new();
        private TableViewModel activeTable = null;

        // Private methods ----------------------------------------------------

        private DocumentViewModel(IDocumentHandler handler)
        {
            isLoading = false;
            this.handler = handler;
        }

        private static TableViewModel BuildTableViewModel(TableModel table, BaseDatabase database, IDocumentHandler handler)
        {
            List<ColumnModel> columns = database.GetColumns(table.Id, false);
            List<ColumnViewModel> columnViewModels = new();

            foreach (var column in columns)
            {
                ColumnViewModel columnViewModel = BuildColumnViewModel(column, database, handler);
                columnViewModels.Add(columnViewModel);
            }

            var tableViewModel = new TableViewModel(table, columnViewModels, handler);
            return tableViewModel;
        }

        private static ColumnViewModel BuildColumnViewModel(ColumnModel column, BaseDatabase database, IDocumentHandler handler)
        {
            List<EntryDisplayModel> entries = database.GetDisplayEntries(column.Id, false);

            List<BaseEntryViewModel> entryViewModels = entries
                .Select(e => BuildEntryViewModel(e, database, handler))
                .Cast<BaseEntryViewModel>()
                .ToList();

            var columnViewModel = new ColumnViewModel(column, entryViewModels, handler);
            return columnViewModel;
        }

        private static EntryViewModel BuildEntryViewModel(EntryDisplayModel entry, BaseDatabase database, IDocumentHandler handler)
        {
            return new EntryViewModel(entry, handler);
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

            loadingWorker = new TableLoadingWorker(document.Database, handler);
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
        public DocumentViewModel(IMapper mapper, IDocumentFactory documentFactory, DocumentInfo info, IDocumentHandler handler)
            : this(handler)
        {
            this.mapper = mapper;
            this.documentFactory = documentFactory;

            documentFactory.SaveDefinition(info);
            document = documentFactory.Create(info);

            entryOrdering = new EntryOrdering(document.Database);
            columnOrdering = new ColumnOrdering(document.Database);
            tableOrdering = new TableOrdering(document.Database);

            LoadTables();
        }

        /// <summary>
        /// Use this ctor for opening existing document from given definition path
        /// </summary>
        public DocumentViewModel(IMapper mapper, IDocumentFactory documentFactory, string definitionPath, IDocumentHandler handler)
            : this(handler)
        {
            this.mapper = mapper;
            this.documentFactory = documentFactory;

            var documentInfo = documentFactory.OpenDefinition(definitionPath);
            document = documentFactory.Open(documentInfo);

            entryOrdering = new EntryOrdering(document.Database);
            columnOrdering = new ColumnOrdering(document.Database);
            tableOrdering = new TableOrdering(document.Database);

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
            // Fill in order
            var tableCount = document.Database.GetTableCount(false);
            tableOrdering.SetNewOrder(newTable, tableCount, 0);
            document.Database.AddTable(newTable);

            var tableViewModel = BuildTableViewModel(newTable, document.Database, handler);
            tables.Add(tableViewModel);

            if (ActiveTable == null)
                ActiveTable = tableViewModel;
        }

        public void UpdateTableFromModel(TableViewModel tableViewModel, TableModel updatedTable)
        {
            document.Database.UpdateTable(updatedTable);

            var newTableViewModel = BuildTableViewModel(updatedTable, document.Database, handler);

            int index = tables.IndexOf(tableViewModel);
            tables[index] = newTableViewModel;

            ActiveTable = newTableViewModel;
        }

        public void UpdateColumnFromModel(ColumnViewModel columnViewModel, ColumnModel updatedColumn)
        {
            document.Database.UpdateColumn(updatedColumn);

            var newColumnViewModel = BuildColumnViewModel(updatedColumn, document.Database, handler);

            var tableViewModel = columnViewModel.Parent;
            int index = tableViewModel.Columns.IndexOf(columnViewModel);
            tableViewModel.Columns[index] = newColumnViewModel;
        }

        public void UpdateEntry(EntryViewModel entryViewModel)
        {
            var entryModel = document.Database.GetEntryDisplay(entryViewModel.Entry.Id);
            var newEntryViewModel = BuildEntryViewModel(entryModel, document.Database, handler);

            var columnViewModel = entryViewModel.Parent;
            var index = columnViewModel.Entries.IndexOf(entryViewModel);
            columnViewModel.Entries[index] = newEntryViewModel;
        }

        public void DeleteTable(TableViewModel tableViewModel, bool permanent)
        {
            document.Database.DeleteTable(tableViewModel.Table.Id, permanent);
            int index = tables.IndexOf(tableViewModel);

            tables.RemoveAt(index);
            index = Math.Max(0, Math.Min(index, tables.Count - 1));

            if (tables.Count > 0)
                ActiveTable = tables[index];
        }

        public void DeleteColumn(ColumnViewModel columnViewModel, bool permanent)
        {
            var tableViewModel = columnViewModel.Parent;
            document.Database.DeleteColumn(columnViewModel.Column.Id, permanent);
            tableViewModel.Columns.Remove(columnViewModel);
        }

        public void ApplyTagChange(ChangeKind changeKind, int tableId, int tagId)
        {
            var tableViewModel = tables.FirstOrDefault(t => t.Table.Id == tableId);

            switch (changeKind)
            {
                case ChangeKind.Add:
                    {
                        // Nothing to do (no entries have this tag yet)
                        break;
                    }
                case ChangeKind.Edit:
                    {
                        var newTagDisplay = document.Database.GetTagDisplay(tagId);

                        foreach (var column in tableViewModel.Columns)
                            foreach (var entry in column.Entries.OfType<EntryViewModel>())
                            {
                                var oldTag = entry.Tags.FirstOrDefault(t => t.Tag.Id == tagId);
                                if (oldTag != null)
                                {
                                    var index = entry.Tags.IndexOf(oldTag);
                                    var newTag = new TagViewModel(newTagDisplay);
                                    entry.Tags[index] = newTag;
                                }
                            }

                        break;
                    }
                case ChangeKind.Delete:
                    {
                        foreach (var column in tableViewModel.Columns)
                            foreach (var entry in column.Entries.OfType<EntryViewModel>())
                            {
                                var oldTag = entry.Tags.First(t => t.Tag.Id == tagId);
                                if (oldTag != null)
                                    entry.Tags.Remove(oldTag);
                            }

                        break;
                    }
                default:
                    throw new InvalidOperationException("Unsupported change kind!");
            }
        }

        public void AddColumnFromModel(TableViewModel tableViewModel, ColumnModel newColumn)
        {
            // Add entity
            newColumn.TableId = tableViewModel.Table.Id;

            // Fill in order
            var columnCount = document.Database.GetColumnCount(tableViewModel.Table.Id, false);
            columnOrdering.SetNewOrder(newColumn, columnCount, tableViewModel.Table.Id);
            document.Database.AddColumn(newColumn);

            var columnViewModel = BuildColumnViewModel(newColumn, document.Database, handler);
            tableViewModel.Columns.Add(columnViewModel);
        }

        public void AddNewInplaceEntry(ColumnViewModel columnViewModel)
        {
            columnViewModel.Entries.Add(new NewInplaceEntryViewModel(handler));
        }

        public void AddEntryFromInplaceNew(NewInplaceEntryViewModel newInplaceEntryViewModel)
        {
            var columnViewModel = newInplaceEntryViewModel.Parent;

            var newEntry = newInplaceEntryViewModel.Entry;
            newEntry.ColumnId = columnViewModel.Column.Id;

            // Fill in order
            var entryCount = document.Database.GetEntryCount(columnViewModel.Column.Id, false);
            entryOrdering.SetNewOrder(newEntry, entryCount, columnViewModel.Column.Id);
            document.Database.AddEntry(newEntry);

            var displayEntry = document.Database.GetEntryDisplay(newEntry.Id);
            var entryViewModel = BuildEntryViewModel(displayEntry, document.Database, handler);

            // Add new entry in the end
            columnViewModel.Entries.Remove(newInplaceEntryViewModel);
            columnViewModel.Entries.Add(entryViewModel);
        }

        public void RemoveInplaceNewEntry(NewInplaceEntryViewModel newInplaceEntryViewModel)
        {
            newInplaceEntryViewModel.Parent.Entries.Remove(newInplaceEntryViewModel);
        }

        public void ReloadTable(TableViewModel updatedTable)
        {
            int tableId = updatedTable.Table.Id;

            var tableModel = document.Database.GetTable(tableId);
            var newTableViewModel = BuildTableViewModel(tableModel, document.Database, handler);

            int index = tables.IndexOf(updatedTable);
            bool selected = ActiveTable == updatedTable;

            tables[index] = newTableViewModel;
            if (selected)
                ActiveTable = newTableViewModel;
        }

        public void DeleteEntry(EntryViewModel entryViewModel, bool permanent)
        {
            var columnViewModel = entryViewModel.Parent;
            document.Database.DeleteEntry(entryViewModel.Entry.Id, permanent);
            columnViewModel.Entries.Remove(entryViewModel);
        }

        public void MoveEntry(EntryViewModel entryViewModel, ColumnViewModel targetColumnViewModel, int newIndex)
        {
            if (entryViewModel.Parent == targetColumnViewModel)
            {
                // Move entry within the same column
                // Note: old order doesn't matter even if the underlying entity will be updated
                // during reordering process.

                var entryModel = document.Database.GetEntryById(entryViewModel.Entry.Id);
                entryOrdering.SetNewOrder(entryModel, newIndex, targetColumnViewModel.Column.Id);
                document.Database.UpdateEntry(entryModel);

                var currentIndex = targetColumnViewModel.Entries.IndexOf(entryViewModel);
                if (newIndex != currentIndex && newIndex != currentIndex + 1)
                {
                    bool afterCurrent = newIndex > currentIndex;
                    if (afterCurrent)
                        newIndex -= 1;

                    targetColumnViewModel.Entries.RemoveAt(currentIndex);
                    targetColumnViewModel.Entries.Insert(newIndex, entryViewModel);
                }
            }
            else
            {
                // Move entry from column to column

                var entryModel = document.Database.GetEntryById(entryViewModel.Entry.Id);
                entryOrdering.SetNewOrder(entryModel, newIndex, targetColumnViewModel.Column.Id);
                entryModel.ColumnId = targetColumnViewModel.Column.Id;
                document.Database.UpdateEntry(entryModel);

                entryViewModel.Parent.Entries.Remove(entryViewModel);
                targetColumnViewModel.Entries.Insert(newIndex, entryViewModel);
            }
        }

        public void MoveColumn(ColumnViewModel columnViewModel, TableViewModel targetTableViewModel, int newIndex)
        {
            if (columnViewModel.Parent != targetTableViewModel)
                throw new ArgumentException("Moving columns between tables is not yet supported!");

            columnOrdering.SetNewOrder(columnViewModel.Column, newIndex, targetTableViewModel.Table.Id);
            document.Database.UpdateColumn(columnViewModel.Column);

            var currentIndex = targetTableViewModel.Columns.IndexOf(columnViewModel);
            if (newIndex != currentIndex && newIndex != currentIndex + 1)
            {
                bool afterCurrent = newIndex > currentIndex;
                if (afterCurrent)
                    newIndex -= 1;

                targetTableViewModel.Columns.RemoveAt(currentIndex);
                targetTableViewModel.Columns.Insert(newIndex, columnViewModel);
            }
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
