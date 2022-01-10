using AutoMapper;
using Board.Models.Data;
using Board.Data.Entities;
using Board.Data.SQLite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable

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

        #region Tables

        public override void AddTable(TableModel newTable)
        {
            var tableEntity = mapper.Map<Table>(newTable);

            context.Tables.Add(tableEntity);
            context.SaveChanges();

            mapper.Map(tableEntity, newTable);
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

        public override long GetFirstTableOrder(bool includeDeleted)
        {
            if (GetTableCount(includeDeleted) == 0)
                return 0;

            return context.Tables
                .Where(t => includeDeleted || !t.IsDeleted)
                .Min(t => t.Order);
        }

        public override long GetLastTableOrder(bool includeDeleted)
        {
            if (GetTableCount(includeDeleted) == 0)
                return 0;

            return context.Tables
                .Where(t => includeDeleted || !t.IsDeleted)
                .Max(t => t.Order);
        }

        public override TableModel GetNextTable(TableModel table, bool includeDeleted)
        {
            var tableEntity = context.Tables
                .Where(t => t.Order > table.Order && (includeDeleted || !t.IsDeleted))
                .OrderBy(t => t.Order)
                .FirstOrDefault();

            if (tableEntity == null)
                return null;

            return mapper.Map<TableModel>(tableEntity);
        }

        public override TableModel GetTable(int tableId)
        {
            var table = context.Tables
                .Single(t => t.Id == tableId);

            var result = mapper.Map<TableModel>(table);
            return result;
        }

        public override int GetTableCount(bool includeDeleted)
        {
            return context.Tables.Count(t => includeDeleted || !t.IsDeleted);
        }

        public override List<TableModel> GetTables(bool includeDeleted)
        {
            var tableEntities = context.Tables.Where(t => includeDeleted || !t.IsDeleted)
                .OrderBy(t => t.Order)
                .ToList();
            return mapper.Map<List<TableModel>>(tableEntities);
        }

        public override List<TableModel> GetTables(int skip, int take, bool includeDeleted)
        {
            var tables = context.Tables
                .Where(t => includeDeleted || !t.IsDeleted)
                .OrderBy(t => t.Order)
                .Skip(skip)
                .Take(take)
                .ToList();

            var result = mapper.Map<List<TableModel>>(tables);
            return result;
        }

        public override void UpdateTable(TableModel updatedTable)
        {
            var table = context.Tables.First(t => t.Id == updatedTable.Id);
            mapper.Map(updatedTable, table);
            context.SaveChanges();
        }

        public override void UpdateTables(List<TableModel> updatedItems)
        {
            List<Table> entities = new();

            foreach (var item in updatedItems)
            {
                var table = context.Tables.Single(e => e.Id == item.Id);
                mapper.Map(item, table);
            }

            context.SaveChanges();
        }

        #endregion

        #region Columns

        public override void AddColumn(ColumnModel newColumn)
        {
            var column = mapper.Map<Column>(newColumn);
            context.Columns.Add(column);
            context.SaveChanges();
            mapper.Map(column, newColumn);
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

        public override int GetColumnCount(int tableId, bool includeDeleted)
        {
            return context.Columns
                .Count(c => c.TableId == tableId && (includeDeleted || !c.IsDeleted));
        }

        public override List<ColumnModel> GetColumns(int tableId, bool includeDeleted)
        {
            var columnEntities = context.Columns
                .Where(c => c.Table.Id == tableId && (includeDeleted || !c.IsDeleted))
                .OrderBy(c => c.Order)
                .ToList();
            return mapper.Map<List<ColumnModel>>(columnEntities);
        }

        public override List<ColumnModel> GetColumns(int tableId, int skip, int take, bool includeDeleted)
        {
            var columns = context.Columns
                .Where(c => c.TableId == tableId && (includeDeleted || !c.IsDeleted))
                .OrderBy(c => c.Order)
                .Skip(skip)
                .Take(take)
                .ToList();

            var result = mapper.Map<List<ColumnModel>>(columns);

            return result;
        }

        public override long GetFirstColumnOrder(int tableId, bool includeDeleted)
        {
            if (GetColumnCount(tableId, includeDeleted) == 0)
                return 0;

            return context.Columns
                .Where(c => c.TableId == tableId && (includeDeleted || !c.IsDeleted))
                .Min(c => c.Order);
        }

        public override long GetLastColumnOrder(int tableId, bool includeDeleted)
        {
            if (GetColumnCount(tableId, includeDeleted) == 0)
                return 0;

            return context.Columns
                .Where(c => c.TableId == tableId && (includeDeleted || !c.IsDeleted))
                .Max(c => c.Order);
        }

        public override ColumnModel GetNextColumn(ColumnModel column, bool includeDeleted)
        {
            var columnEntity = context.Columns
                .Where(c => c.TableId == column.TableId && c.Order > column.Order && (includeDeleted || !c.IsDeleted))
                .OrderBy(c => c.Order)
                .FirstOrDefault();

            if (columnEntity == null)
                return null;

            return mapper.Map<ColumnModel>(columnEntity);
        }

        public override void UpdateColumn(ColumnModel updatedColumn)
        {
            var column = context.Columns.First(c => c.Id == updatedColumn.Id);
            mapper.Map(updatedColumn, column);
            context.SaveChanges();
        }

        public override void UpdateColumns(List<ColumnModel> updatedItems)
        {
            List<Column> entities = new();

            foreach (var item in updatedItems)
            {
                var column = context.Columns.Single(e => e.Id == item.Id);
                mapper.Map(item, column);
            }

            context.SaveChanges();
        }

        #endregion

        #region Entries

        public override void AddEntry(EntryModel newEntry)
        {
            var entry = mapper.Map<Entry>(newEntry);
            context.Entries.Add(entry);
            context.SaveChanges();
            mapper.Map(entry, newEntry);
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

        public override List<EntryModel> GetEntries(int columnId, bool includeDeleted)
        {
            var entryEntities = context.Entries
                .Where(e => e.Column.Id == columnId && (includeDeleted || !e.IsDeleted))
                .OrderBy(e => e.Order)
                .ToList();

            return mapper.Map<List<EntryModel>>(entryEntities);
        }

        public override List<EntryModel> GetEntries(int columnId, int skip, int take, bool includeDeleted)
        {
            var entries = context.Entries
                .Where(e => e.ColumnId == columnId && (includeDeleted || !e.IsDeleted))
                .OrderBy(e => e.Order)
                .Skip(skip)
                .Take(take)
                .ToList();

            var result = mapper.Map<List<EntryModel>>(entries);
            return result;
        }

        public override EntryModel GetEntryById(int id)
        {
            var entry = context.Entries.First(e => e.Id == id && !e.IsDeleted);
            var result = mapper.Map<EntryModel>(entry);
            return result;
        }

        public override int GetEntryCount(int columnId, bool includeDeleted)
        {
            return context.Entries
                .Count(e => e.ColumnId == columnId && (includeDeleted || !e.IsDeleted));
        }

        public override long GetFirstEntryOrder(int columnId, bool includeDeleted)
        {
            if (GetEntryCount(columnId, includeDeleted) == 0)
                return 0;

            return context.Entries
                .Where(e => e.ColumnId == columnId && (includeDeleted || !e.IsDeleted))
                .Min(e => e.Order);
        }

        public override EntryModel GetFullEntryById(int id)
        {
            var entry = context.Entries.First(e => e.Id == id && !e.IsDeleted);
            var result = mapper.Map<EntryModel>(entry);
            return result;
        }

        public override long GetLastEntryOrder(int columnId, bool includeDeleted)
        {
            if (GetEntryCount(columnId, includeDeleted) == 0)
                return 0;

            return context.Entries
                .Where(e => e.ColumnId == columnId && (includeDeleted || !e.IsDeleted))
                .Max(e => e.Order);
        }

        public override EntryModel GetNextEntry(EntryModel entry, bool includeDeleted)
        {
            var entryEntity = context.Entries
                .Where(e => e.ColumnId == entry.ColumnId && e.Order > entry.Order && (includeDeleted || !e.IsDeleted))
                .OrderBy(e => e.Order)
                .FirstOrDefault();

            if (entryEntity == null)
                return null;

            return mapper.Map<EntryModel>(entryEntity);
        }

        public override void UpdateEntries(List<EntryModel> updatedItems)
        {
            List<Entry> entities = new();

            foreach (var item in updatedItems)
            {
                var entry = context.Entries.Single(e => e.Id == item.Id);
                mapper.Map(item, entry);
            }

            context.SaveChanges();
        }

        public override void UpdateEntry(EntryModel updatedEntry)
        {
            var entry = context.Entries.First(e => e.Id == updatedEntry.Id);
            mapper.Map(updatedEntry, entry);
            context.SaveChanges();
        }

        #endregion

        #region Tags

        public override void AddTag(TagModel newTag)
        {
            var tag = mapper.Map<Tag>(newTag);
            context.Tags.Add(tag);
            context.SaveChanges();
            mapper.Map(tag, newTag);
        }

        public override void DeleteTag(TagModel deletedTag, bool permanent)
        {
            var tag = context.Tags.First(e => e.Id == deletedTag.Id);

            if (permanent)
            {
                context.Remove<Tag>(tag);
                context.SaveChanges();
            }
            else
            {
                tag.IsDeleted = true;
                context.SaveChanges();
            }
        }

        public override List<TagModel> GetTags(int tableId, bool includeDeleted)
        {
            var tagEntries = context.Tags
                .Where(t => t.TableId == tableId && (includeDeleted || !t.IsDeleted))
                .OrderBy(t => t.Name)
                .ToList();

            var result = mapper.Map<List<TagModel>>(tagEntries);
            return result;
        }

        public override void UpdateTag(TagModel updatedTag)
        {
            var tag = context.Tags.First(t => t.Id == updatedTag.Id);
            mapper.Map(updatedTag, tag);
            context.SaveChanges();
        }

        #endregion
    }
}
