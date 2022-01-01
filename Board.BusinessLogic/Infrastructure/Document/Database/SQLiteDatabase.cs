using AutoMapper;
using Board.BusinessLogic.Models.Data;
using Board.Data.Entities;
using Board.Data.SQLite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Board.BusinessLogic.Infrastructure.Document.Database
{
    public class SQLiteDatabase : BaseDatabase
    {
        private readonly TableContext context;
        private readonly IMapper mapper;

        public SQLiteDatabase(string path, IMapper mapper)
        {
            context = new TableContext(path);
            context.Database.Migrate();
            this.mapper = mapper;
        }

        public override void AddTable(TableModel newTable)
        {
            var tableEntity = mapper.Map<Table>(newTable);

            context.Tables.Add(tableEntity);
            context.SaveChanges();

            mapper.Map(tableEntity, newTable);
        }

        public override List<ColumnModel> GetColumnsForTable(int id)
        {
            var columnEntities = context.Columns.Where(c => c.Table.Id == id);
            return mapper.Map<List<ColumnModel>>(columnEntities);
        }

        public override List<EntryModel> GetEntriesForColumn(int id)
        {
            var entryEntities = context.Entries.Where(e => e.Column.Id == id);
            return mapper.Map<List<EntryModel>>(entryEntities);
        }

        public override List<TableModel> GetTables()
        {
            var tableEntities = context.Tables.ToListAsync().Result;
            return mapper.Map<List<TableModel>>(tableEntities);
        }
    }
}
