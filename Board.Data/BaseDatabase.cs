using Board.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Board.BusinessLogic.Infrastructure.Document.Database
{
    public abstract class BaseDatabase
    {
        public abstract List<TableModel> GetTables();
        public abstract List<ColumnModel> GetColumnsForTable(int id);
        public abstract List<EntryModel> GetEntriesForColumn(int id);

        public abstract void AddTable(OrderedTableModel newTable);
        public abstract void UpdateOrderedTable(OrderedTableModel updatedTable);
        public abstract void UpdateTable(TableModel updatedTable);
        public abstract void DeleteTable(TableModel table, bool permanent);

        public abstract void AddColumn(OrderedColumnModel newColumn);
        public abstract void UpdateOrderedColumn(OrderedColumnModel updatedColumn);
        public abstract void UpdateColumn(ColumnModel updatedColumn);
        public abstract void DeleteColumn(ColumnModel column, bool permanent);

        public abstract void AddEntry(OrderedEntryModel newEntry);
        public abstract void UpdateOrderedEntry(OrderedEntryModel updatedEntry);
        public abstract void UpdateEntry(EntryModel updatedEntry);
        public abstract void DeleteEntry(EntryModel deletedEntry, bool permanent);

        public abstract EntryModel GetFullEntryById(int id);
        public abstract EntryModel GetEntryById(int id);

        public abstract long? GetFirstEntryOrder(int columnId);
        public abstract long? GetLastEntryOrder(int columnId);
        public abstract int GetEntryCount(int columnId);
        public abstract List<OrderedEntryModel> GetOrderedEntries(int columnId, int skip, int take);
        public abstract void UpdateOrderedEntries(List<OrderedEntryModel> updatedItems);
        public abstract long? GetFirstColumnOrder(int tableId);
        public abstract long? GetLastColumnOrder(int tableId);
        public abstract int GetColumnCount(int tableId);
        public abstract List<OrderedColumnModel> GetOrderedColumns(int tableId, int skip, int take);
        public abstract void UpdateOrderedColumns(List<OrderedColumnModel> updatedItems);
        public abstract long? GetFirstTableOrder();
        public abstract long? GetLastTableOrder();
        public abstract int GetTableCount();
        public abstract List<OrderedTableModel> GetOrderedTables(int skip, int take);
        public abstract void UpdateOrderedTables(List<OrderedTableModel> updatedItems);
    }
}
