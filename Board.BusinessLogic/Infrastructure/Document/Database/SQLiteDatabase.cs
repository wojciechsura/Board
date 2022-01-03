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

        public override List<ColumnModel> GetColumnsForTable(int id)
        {
            var columnEntities = context.Columns
                .Where(c => c.Table.Id == id && !c.IsDeleted)
                .ToList();
            return mapper.Map<List<ColumnModel>>(columnEntities);
        }

        public override List<EntryModel> GetEntriesForColumn(int id)
        {
            var entryEntities = context.Entries
                .Where(e => e.Column.Id == id && !e.IsDeleted)
                .ToList();
            return mapper.Map<List<EntryModel>>(entryEntities);
        }

        public override List<TableModel> GetTables()
        {
            var tableEntities = context.Tables.Where(t => !t.IsDeleted)
                .ToListAsync().Result;
            return mapper.Map<List<TableModel>>(tableEntities);
        }

        public override void AddTable(TableModel newTable)
        {
            var tableEntity = mapper.Map<Table>(newTable);

            context.Tables.Add(tableEntity);
            context.SaveChanges();

            mapper.Map(tableEntity, newTable);
        }

        public override void UpdateTable(TableModel updatedTable)
        {
            var table = mapper.Map<Table>(updatedTable);
            context.Update<Table>(table);
            context.SaveChanges();
        }

        public override void DeleteTable(TableModel deletedTable, bool permanent)
        {
            var table = mapper.Map<Table>(deletedTable);

            if (permanent)
            {
                context.Remove<Table>(table);
                context.SaveChanges();
            }
            else
            {
                table.IsDeleted = true;
                context.Update(table);
                context.SaveChanges();
            }
        }

        public override void AddColumn(ColumnModel newColumn)
        {
            var column = mapper.Map<Column>(newColumn);
            context.Columns.Add(column);
            context.SaveChanges();
        }
    }
}
