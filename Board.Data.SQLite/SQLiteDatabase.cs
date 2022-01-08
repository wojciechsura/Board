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
                .OrderBy(c => c.Order)
                .ToList();
            return mapper.Map<List<ColumnModel>>(columnEntities);
        }

        public override List<EntryModel> GetEntriesForColumn(int id)
        {
            var entryEntities = context.Entries
                .Where(e => e.Column.Id == id && !e.IsDeleted)
                .OrderBy(e => e.Order)
                .ToList();
            return mapper.Map<List<EntryModel>>(entryEntities);
        }

        public override List<TableModel> GetTables()
        {
            var tableEntities = context.Tables.Where(t => !t.IsDeleted)
                .OrderBy(t => t.Order)
                .ToList();
            return mapper.Map<List<TableModel>>(tableEntities);
        }

        public override void AddTable(OrderedTableModel newTable)
        {
            var tableEntity = mapper.Map<Table>(newTable);

            context.Tables.Add(tableEntity);
            context.SaveChanges();

            mapper.Map(tableEntity, newTable);
        }

        public override void UpdateOrderedTable(OrderedTableModel updatedTable)
        {
            var table = context.Tables.First(t => t.Id == updatedTable.Id);
            mapper.Map(updatedTable, table);
            context.SaveChanges();
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

        public override void AddColumn(OrderedColumnModel newColumn)
        {
            var column = mapper.Map<Column>(newColumn);
            context.Columns.Add(column);
            context.SaveChanges();
            mapper.Map(column, newColumn);
        }

        public override void UpdateOrderedColumn(OrderedColumnModel updatedColumn)
        {
            var column = context.Columns.First(c => c.Id == updatedColumn.Id);
            mapper.Map(updatedColumn, column);
            context.SaveChanges();
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

        public override void AddEntry(OrderedEntryModel newEntry)
        {
            var entry = mapper.Map<Entry>(newEntry);
            context.Entries.Add(entry);
            context.SaveChanges();
            mapper.Map(entry, newEntry);
        }

        public override void UpdateOrderedEntry(OrderedEntryModel updatedEntry)
        {
            var entry = context.Entries.First(e => e.Id == updatedEntry.Id);
            mapper.Map(updatedEntry, entry);
            context.SaveChanges();
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

        public override long? GetFirstEntryOrder(int columnId)
        {
            if (GetEntryCount(columnId) == 0)
                return null;

            return context.Entries
                .Where(e => e.ColumnId == columnId)
                .Min(e => e.Order);
        }

        public override long? GetLastEntryOrder(int columnId)
        {
            if (GetEntryCount(columnId) == 0)
                return null;

            return context.Entries
                .Where(e => e.ColumnId == columnId)
                .Max(e => e.Order);
        }

        public override int GetEntryCount(int columnId)
        {
            return context.Entries
                .Count(e => e.ColumnId == columnId);
        }

        public override List<OrderedEntryModel> GetOrderedEntries(int columnId, int skip, int take)
        {
            var entries = context.Entries
                .Where(e => e.ColumnId == columnId)
                .OrderBy(e => e.Order)
                .Skip(skip)
                .Take(take)
                .ToList();

            var result = mapper.Map<List<OrderedEntryModel>>(entries);
            return result;
        }

        public override void UpdateOrderedEntries(List<OrderedEntryModel> updatedItems)
        {
            List<Entry> entities = new();

            foreach (var item in updatedItems)
            {
                var entry = context.Entries.Single(e => e.Id == item.Id);
                mapper.Map(item, entry);
            }

            context.SaveChanges();
        }

        public override long? GetFirstColumnOrder(int tableId)
        {
            if (GetColumnCount(tableId) == 0)
                return null;

            return context.Columns
                .Where(c => c.TableId == tableId)
                .Min(c => c.Order);
        }

        public override long? GetLastColumnOrder(int tableId)
        {
            if (GetColumnCount(tableId) == 0)
                return null;

            return context.Columns
                .Where(c => c.TableId == tableId)
                .Max(c => c.Order);
        }

        public override int GetColumnCount(int tableId)
        {
            return context.Columns
                .Count(c => c.TableId == tableId);
        }

        public override List<OrderedColumnModel> GetOrderedColumns(int tableId, int skip, int take)
        {
            var columns = context.Columns
                .Where(c => c.TableId == tableId)
                .OrderBy(c => c.Order)
                .Skip(skip)
                .Take(take)
                .ToList();

            var result = mapper.Map<List<OrderedColumnModel>>(columns);

            return result;
        }

        public override void UpdateOrderedColumns(List<OrderedColumnModel> updatedItems)
        {
            List<Column> entities = new();

            foreach (var item in updatedItems)
            {
                var column = context.Columns.Single(e => e.Id == item.Id);
                mapper.Map(item, column);
            }

            context.SaveChanges();
        }

        public override long? GetFirstTableOrder()
        {
            if (GetTableCount() == 0)
                return null;

            return context.Tables
                .Min(t => t.Order);
        }

        public override long? GetLastTableOrder()
        {
            if (GetTableCount() == 0)
                return null;

            return context.Tables
                .Max(t => t.Order);
        }

        public override int GetTableCount()
        {
            return context.Tables.Count();
        }

        public override List<OrderedTableModel> GetOrderedTables(int skip, int take)
        {
            var tables = context.Tables
                .OrderBy(t => t.Order)
                .Skip(skip)
                .Take(take)
                .ToList();

            var result = mapper.Map<List<OrderedTableModel>>(tables);
            return result;
        }

        public override void UpdateOrderedTables(List<OrderedTableModel> updatedItems)
        {
            List<Table> entities = new();

            foreach (var item in updatedItems)
            {
                var table = context.Tables.Single(e => e.Id == item.Id);
                mapper.Map(item, table);
            }

            context.SaveChanges();
        }
    }
}
