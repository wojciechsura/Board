using AutoMapper;
using Board.Models.Data;
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
            var table = context.Tables.First(t => t.Id == updatedTable.Id);
            mapper.Map(updatedTable, table);
            context.SaveChanges();
        }

        public override void DeleteTable(TableModel deletedTable, bool permanent)
        {
            var table = context.Tables.First(t => t.Id == deletedTable.Id);

            if (permanent)
            {
                context.Remove<Table>(table);
                context.SaveChanges();
            }
            else
            {
                table.IsDeleted = true;
                context.SaveChanges();
            }
        }

        public override void AddColumn(ColumnModel newColumn)
        {
            var column = mapper.Map<Column>(newColumn);
            context.Columns.Add(column);
            context.SaveChanges();
            mapper.Map(column, newColumn);
        }

        public override void UpdateColumn(ColumnModel updatedColumn)
        {
            var column = context.Columns.First(c => c.Id == updatedColumn.Id);
            mapper.Map(updatedColumn, column);
            context.SaveChanges();
        }

        public override void DeleteColumn(ColumnModel deletedColumn, bool permanent)
        {
            var column = context.Columns.First(c => c.Id == deletedColumn.Id);

            if (permanent)
            {
                context.Remove<Column>(column);
                context.SaveChanges();
            }
            else
            {
                column.IsDeleted = true;
                context.SaveChanges();
            }
        }

        public override void AddEntry(EntryModel newEntry)
        {
            var entry = mapper.Map<Entry>(newEntry);
            context.Entries.Add(entry);
            context.SaveChanges();
            mapper.Map(entry, newEntry);
        }

        public override void UpdateEntry(EntryModel updatedEntry)
        {
            var entry = context.Entries.First(e => e.Id == updatedEntry.Id);
            mapper.Map(updatedEntry, entry);
            context.SaveChanges();
        }

        public override void DeleteEntry(EntryModel deletedEntry, bool permanent)
        {
            var entry = context.Entries.First(e => e.Id == deletedEntry.Id);

            if (permanent)
            {
                context.Remove<Entry>(entry);
                context.SaveChanges();
            }
            else
            {
                entry.IsDeleted = true;
                context.SaveChanges();
            }
        }

        public override EntryModel GetFullEntryById(int id)
        {
            var entry = context.Entries.First(e => e.Id == id);
            var result = mapper.Map<EntryModel>(entry);
            return result;
        }

        public override EntryModel GetEntryById(int id)
        {
            var entry = context.Entries.First(e => e.Id == id);
            var result = mapper.Map<EntryModel>(entry);
            return result;
        }
    }
}
