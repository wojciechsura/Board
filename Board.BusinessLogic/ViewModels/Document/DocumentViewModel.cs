﻿using Board.BusinessLogic.Infrastructure.Document;
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
using System.IO;
using Board.BusinessLogic.Infrastructure.Document.Filesystem;
using Board.BusinessLogic.Infrastructure.Collections;

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
            private readonly BaseFilesystem filesystem;
            private readonly IDocumentHandler handler;            

            public TableLoadingWorker(BaseDatabase database, BaseFilesystem filesystem, IDocumentHandler handler)
            {
                this.database = database;
                this.filesystem = filesystem;
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
                    TableViewModel tableViewModel = BuildTableViewModel(table, database, filesystem, handler);

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
                return database.GetEntryCount(groupId, null, false);
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

        private const int LOADED_ENTRY_COUNT = 10;

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

        private readonly ObservableParentedCollection<TableViewModel, DocumentViewModel> tables;
        private TableViewModel activeTable = null;

        // Private methods ----------------------------------------------------

        private DocumentViewModel(IDocumentHandler handler)
        {
            tables = new(this);

            isLoading = false;
            this.handler = handler;
        }

        private static (List<BaseEntryViewModel> entries, bool canLoadMore) LoadEntries(ColumnModel column, BaseDatabase database, IDocumentHandler handler, string filter = null)
        {
            List<EntryDisplayModel> entries;
            bool canLoadMore;

            if (column.LimitShownItems == null)
            {
                entries = database.GetDisplayEntries(column.Id, filter, false);
                canLoadMore = false;
            }
            else
            {
                entries = database.GetDisplayEntries(column.Id, long.MinValue, column.LimitShownItems.Value, filter, false);
                int totalEntries = database.GetEntryCount(column.Id, filter, false);
                canLoadMore = totalEntries > entries.Count;
            }

            List<BaseEntryViewModel> entryViewModels = entries
                .Select(e => BuildEntryViewModel(e, database, handler))
                .Cast<BaseEntryViewModel>()
                .ToList();

            return (entryViewModels, canLoadMore);
        }

        private static TableViewModel BuildTableViewModel(TableModel table, BaseDatabase database, BaseFilesystem filesystem, IDocumentHandler handler)
        {
            List<ColumnModel> columns = database.GetColumns(table.Id, false);
            List<ColumnViewModel> columnViewModels = new();

            foreach (var column in columns)
            {
                ColumnViewModel columnViewModel = BuildColumnViewModel(column, database, handler);
                columnViewModels.Add(columnViewModel);
            }

            MemoryStream background = null;
            if (!string.IsNullOrEmpty(table.Background))
                background = filesystem.LoadFile(table.Background);

            var tableViewModel = new TableViewModel(table, background, columnViewModels, handler);
            return tableViewModel;
        }

        private static ColumnViewModel BuildColumnViewModel(ColumnModel column, BaseDatabase database, IDocumentHandler handler)
        {
            (List<BaseEntryViewModel> entryViewModels, bool canLoadMore) = LoadEntries(column, database, handler);

            var columnViewModel = new ColumnViewModel(column, entryViewModels, handler, canLoadMore);
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

            loadingWorker = new TableLoadingWorker(document.Database, document.Filesystem, handler);
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

            if (ActiveTable == null)
                ActiveTable = tables.Last();
        }

        private void UpdateBackground(TableModel model, MemoryStream backgroundStream)
        {
            // If old model is not null, remove old background (if any)
            if (!string.IsNullOrEmpty(model.Background))
            {
                document.Filesystem.DeleteFile(model.Background);
            }

            // Store new file
            if (backgroundStream != null)
            {
                string path = @$"Tables\{model.Id}\background.dat";
                document.Filesystem.SaveFile(path, backgroundStream);
                model.Background = path;
            }        
            else
            {
                model.Background = null;
            }                
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

        public void ApplyFilter(TableViewModel tableViewModel)
        {
            foreach (var column in tableViewModel.Columns)
            {
                column.Entries.Clear();

                (List<BaseEntryViewModel> entries, bool canLoadMore) = LoadEntries(column.Column, document.Database, handler, tableViewModel.FilterText);

                foreach (var entry in entries)
                    column.Entries.Add(entry);

                column.CanLoadMore = canLoadMore;
            }
        }

        public void AddTableFromModel(TableEditModel newTable)
        {
            // Update data
            var tableModel = mapper.Map<TableModel>(newTable);
            var tableCount = document.Database.GetTableCount(false);
            tableOrdering.SetNewOrder(tableModel, tableCount, 0);
            document.Database.AddTable(tableModel);

            // Having new ID we can now save background if any
            if (newTable.BackgroundChanged)
            {
                UpdateBackground(tableModel, newTable.BackgroundStream);
                document.Database.UpdateTable(tableModel);
            }

            // Update view
            var tableViewModel = BuildTableViewModel(tableModel, document.Database, document.Filesystem, handler);
            tables.Add(tableViewModel);

            if (ActiveTable == null)
                ActiveTable = tableViewModel;
        }

        public void UpdateTableFromModel(TableViewModel tableViewModel, TableEditModel updatedTable)
        {
            // Update data

            var tableModel = mapper.Map<TableModel>(updatedTable);
            if (updatedTable.BackgroundChanged)
            {
                UpdateBackground(tableModel, updatedTable.BackgroundStream);
            }
            document.Database.UpdateTable(tableModel);

            // Update view

            var newTableViewModel = BuildTableViewModel(tableModel, document.Database, document.Filesystem, handler);
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
                        var newTagDisplay = document.Database.GetTag(tagId);

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
                                var oldTag = entry.Tags.FirstOrDefault(t => t.Tag.Id == tagId);
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
            columnViewModel.Entries.Insert(0, new NewInplaceEntryViewModel(handler));
        }

        public void AddEntryFromInplaceNew(NewInplaceEntryViewModel newInplaceEntryViewModel)
        {
            var columnViewModel = newInplaceEntryViewModel.Parent;

            var newEntry = newInplaceEntryViewModel.Entry;
            newEntry.ColumnId = columnViewModel.Column.Id;
            newEntry.CreatedDate = DateTime.Now;

            // Fill in order
            var entryCount = document.Database.GetEntryCount(columnViewModel.Column.Id, null, false);
            entryOrdering.SetNewOrder(newEntry, 0, columnViewModel.Column.Id);
            document.Database.AddEntry(newEntry);

            var displayEntry = document.Database.GetEntryDisplay(newEntry.Id);
            var entryViewModel = BuildEntryViewModel(displayEntry, document.Database, handler);

            // Add new entry in the end
            columnViewModel.Entries.Remove(newInplaceEntryViewModel);
            columnViewModel.Entries.Insert(0, entryViewModel);
        }

        public void RemoveInplaceNewEntry(NewInplaceEntryViewModel newInplaceEntryViewModel)
        {
            newInplaceEntryViewModel.Parent.Entries.Remove(newInplaceEntryViewModel);
        }

        public void ReloadTable(TableViewModel updatedTable)
        {
            int tableId = updatedTable.Table.Id;

            var tableModel = document.Database.GetTable(tableId);
            var newTableViewModel = BuildTableViewModel(tableModel, document.Database, document.Filesystem, handler);

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

                var currentIndex = targetColumnViewModel.Entries.IndexOf(entryViewModel);
                if (newIndex == currentIndex || newIndex == currentIndex + 1)
                    return;

                var entryModel = document.Database.GetEntryById(entryViewModel.Entry.Id);
                entryOrdering.SetNewOrder(entryModel, newIndex, targetColumnViewModel.Column.Id);
                document.Database.UpdateEntry(entryModel);

                // Recreate entry viewmodel
                var updatedEntryDisplayModel = document.Database.GetEntryDisplay(entryViewModel.Entry.Id);
                var updatedEntryViewModel = new EntryViewModel(updatedEntryDisplayModel, handler);

                bool afterCurrent = newIndex > currentIndex;
                if (afterCurrent)
                    newIndex -= 1;

                targetColumnViewModel.Entries.RemoveAt(currentIndex);
                targetColumnViewModel.Entries.Insert(newIndex, updatedEntryViewModel);
            }
            else
            {
                // Move entry from column to column

                var entryModel = document.Database.GetEntryById(entryViewModel.Entry.Id);
                entryOrdering.SetNewOrder(entryModel, newIndex, targetColumnViewModel.Column.Id);
                entryModel.ColumnId = targetColumnViewModel.Column.Id;
                document.Database.UpdateEntry(entryModel);

                // Recreate entry viewmodel
                var updatedEntryDisplayModel = document.Database.GetEntryDisplay(entryViewModel.Entry.Id);
                var updatedEntryViewModel = new EntryViewModel(updatedEntryDisplayModel, handler);

                entryViewModel.Parent.Entries.Remove(entryViewModel);
                targetColumnViewModel.Entries.Insert(newIndex, updatedEntryViewModel);
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

        public void LoadMoreEntries(ColumnViewModel columnViewModel)
        {
            var maxOrder = columnViewModel.Entries.OfType<EntryViewModel>().Max(e => e.Entry.Order);
            var moreEntryModels = document.Database.GetDisplayEntries(columnViewModel.Column.Id, maxOrder + 1, LOADED_ENTRY_COUNT, columnViewModel.Parent.FilterText, false);
            bool canLoadMore = moreEntryModels.Count == LOADED_ENTRY_COUNT;

            List<BaseEntryViewModel> moreEntryViewModels = moreEntryModels
                .Select(e => BuildEntryViewModel(e, document.Database, handler))
                .Cast<BaseEntryViewModel>()
                .ToList();

            foreach (var entryViewModel in moreEntryViewModels)
                columnViewModel.Entries.Add(entryViewModel);
            columnViewModel.CanLoadMore = canLoadMore;
        }

        public TableEditModel GetTableEditModel(TableViewModel activeTable)
        {
            var result = mapper.Map<TableEditModel>(activeTable.Table);

            if (!string.IsNullOrEmpty(activeTable.Table.Background))
            {
                MemoryStream stream = document.Filesystem.LoadFile(activeTable.Table.Background);
                result.BackgroundStream = stream;
                result.BackgroundChanged = false;
            }

            return result;
        }

        // Public properties --------------------------------------------------

        public ObservableParentedCollection<TableViewModel, DocumentViewModel> Tables => tables;

        public WallDocument Document => document;

        public TableViewModel ActiveTable
        {
            get => activeTable;
            set => Set(ref activeTable, value);
        }

        public bool IsLoading
        {
            get => isLoading;
            set => Set(ref isLoading, value);
        }
    }
}
