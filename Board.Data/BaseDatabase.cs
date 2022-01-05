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

        public abstract void AddTable(TableModel newTable);
        public abstract void UpdateTable(TableModel updatedTable);
        public abstract void DeleteTable(TableModel table, bool permanent);

        public abstract void AddColumn(ColumnModel newColumn);
        public abstract void UpdateColumn(ColumnModel updatedColumn);
        public abstract void DeleteColumn(ColumnModel column, bool permanent);

        public abstract void AddEntry(EntryModel newEntry);
        public abstract void UpdateEntry(EntryModel updatedEntry);
        public abstract void DeleteEntry(EntryModel deletedEntry, bool permanent);

        public abstract EntryModel GetFullEntryById(int id);
        public abstract EntryModel GetEntryById(int id);        
    }
}
