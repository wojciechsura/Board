using Board.BusinessLogic.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Board.BusinessLogic.Infrastructure.Document.Database
{
    public abstract class BaseDatabase
    {
        public abstract void AddTable(TableModel newTable);
        public abstract List<TableModel> GetTables();
        public abstract List<ColumnModel> GetColumnsForTable(int id);
        public abstract List<EntryModel> GetEntriesForColumn(int id);
    }
}
