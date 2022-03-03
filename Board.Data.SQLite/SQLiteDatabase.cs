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
        private readonly string path;
        private readonly IMapper mapper;

        private void CreateRollingBackup(string path)
        {
            string filePath = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            
            for (int i = 9; i >= 1; i--)
            {
                string from = Path.Combine(filePath, $"{fileName}-{i}.bak");
                string to = Path.Combine(filePath, $"{fileName}-{i + 1}.bak");

                if (File.Exists(to))
                    File.Delete(to);

                if (File.Exists(from))
                    File.Move(from, to);
            }

            File.Copy(path, Path.Combine(filePath, $"{fileName}-1.bak"));
        }

        public SQLiteDatabase(string path, IMapper mapper)
        {
            CreateRollingBackup(path);

            var context = new TableContext(path);

            context.Database.Migrate();
            this.path = path;
            this.mapper = mapper;
        }

        #region Tables

        public override void AddTable(TableModel newTable)
        {
            var context = new TableContext(path);

            var tableEntity = mapper.Map<Table>(newTable);

            context.Tables.Add(tableEntity);
            context.SaveChanges();

            mapper.Map(tableEntity, newTable);
        }

        public override void DeleteTable(int tableId, bool permanent)
        {
            var context = new TableContext(path);

            var table = context.Tables.First(t => t.Id == tableId);

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
            var context = new TableContext(path);

            if (GetTableCount(includeDeleted) == 0)
                return 0;

            return context.Tables
                .Where(t => includeDeleted || !t.IsDeleted)
                .Min(t => t.Order);
        }

        public override long GetLastTableOrder(bool includeDeleted)
        {
            var context = new TableContext(path);

            if (GetTableCount(includeDeleted) == 0)
                return 0;

            return context.Tables
                .Where(t => includeDeleted || !t.IsDeleted)
                .Max(t => t.Order);
        }

        public override TableModel GetNextTable(TableModel table, bool includeDeleted)
        {
            var context = new TableContext(path);

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
            var context = new TableContext(path);

            var table = context.Tables
                .Single(t => t.Id == tableId);

            var result = mapper.Map<TableModel>(table);
            return result;
        }

        public override int GetTableCount(bool includeDeleted)
        {
            var context = new TableContext(path);

            return context.Tables.Count(t => includeDeleted || !t.IsDeleted);
        }

        public override List<TableModel> GetTables(bool includeDeleted)
        {
            var context = new TableContext(path);

            var tableEntities = context.Tables.Where(t => includeDeleted || !t.IsDeleted)
                .OrderBy(t => t.Order)
                .ToList();
            return mapper.Map<List<TableModel>>(tableEntities);
        }

        public override List<TableModel> GetTables(int skip, int take, bool includeDeleted)
        {
            var context = new TableContext(path);

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
            var context = new TableContext(path);

            var table = context.Tables.First(t => t.Id == updatedTable.Id);
            mapper.Map(updatedTable, table);
            context.SaveChanges();
        }

        public override void UpdateTables(List<TableModel> updatedItems)
        {
            var context = new TableContext(path);

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
            var context = new TableContext(path);

            var column = mapper.Map<Column>(newColumn);
            context.Columns.Add(column);
            context.SaveChanges();
            mapper.Map(column, newColumn);
        }

        public override void DeleteColumn(int columnId, bool permanent)
        {
            var context = new TableContext(path);

            var column = context.Columns.First(c => c.Id == columnId);

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

        public override ColumnModel GetColumn(int columnId)
        {
            var context = new TableContext(path);

            return mapper.Map<ColumnModel>(context.Columns.Find(columnId));
        }

        public override int GetColumnCount(int tableId, bool includeDeleted)
        {
            var context = new TableContext(path);

            return context.Columns
                .Count(c => c.TableId == tableId && (includeDeleted || !c.IsDeleted));
        }

        public override List<ColumnModel> GetColumns(int tableId, bool includeDeleted)
        {
            var context = new TableContext(path);

            var columnEntities = context.Columns
                .Where(c => c.Table.Id == tableId && (includeDeleted || !c.IsDeleted))
                .OrderBy(c => c.Order)
                .ToList();
            return mapper.Map<List<ColumnModel>>(columnEntities);
        }

        public override List<ColumnModel> GetColumns(int tableId, int skip, int take, bool includeDeleted)
        {
            var context = new TableContext(path);

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
            var context = new TableContext(path);

            if (GetColumnCount(tableId, includeDeleted) == 0)
                return 0;

            return context.Columns
                .Where(c => c.TableId == tableId && (includeDeleted || !c.IsDeleted))
                .Min(c => c.Order);
        }

        public override long GetLastColumnOrder(int tableId, bool includeDeleted)
        {
            var context = new TableContext(path);

            if (GetColumnCount(tableId, includeDeleted) == 0)
                return 0;

            return context.Columns
                .Where(c => c.TableId == tableId && (includeDeleted || !c.IsDeleted))
                .Max(c => c.Order);
        }

        public override ColumnModel GetNextColumn(ColumnModel column, bool includeDeleted)
        {
            var context = new TableContext(path);

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
            var context = new TableContext(path);

            var column = context.Columns.First(c => c.Id == updatedColumn.Id);
            mapper.Map(updatedColumn, column);
            context.SaveChanges();
        }

        public override void UpdateColumns(List<ColumnModel> updatedItems)
        {
            var context = new TableContext(path);

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
            var context = new TableContext(path);

            var entry = mapper.Map<Entry>(newEntry);
            context.Entries.Add(entry);
            context.SaveChanges();
            mapper.Map(entry, newEntry);
        }

        public override void AddTagToEntry(int entryId, int tagId)
        {
            var context = new TableContext(path);

            var entry = context.Entries
                .Include(e => e.Tags)
                .First(e => e.Id == entryId);

            if (!entry.Tags.Any(t => t.Id == tagId))
            {
                var tag = context.Tags.Find(tagId);
                entry.Tags.Add(tag);
                context.SaveChanges();
            }
        }

        public override void DeleteEntry(int entryId, bool permanent)
        {
            var context = new TableContext(path);

            var entry = context.Entries.First(e => e.Id == entryId);

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

        public override List<EntryDisplayModel> GetDisplayEntries(int columnId, bool includeDeleted)
        {
            var context = new TableContext(path);

            var entryEntities = context.Entries
                .Include(e => e.Tags.Where(t => (includeDeleted || !t.IsDeleted)).OrderBy(t => t.Name))
                .Include(e => e.Comments.Where(c => (includeDeleted || !c.IsDeleted)).OrderByDescending(c => c.Added))
                .Where(e => e.ColumnId == columnId && (includeDeleted || !e.IsDeleted))
                .OrderBy(e => e.Order)
                .ToList();

            var result = mapper.Map<List<EntryDisplayModel>>(entryEntities);

            return result;
        }

        public override List<EntryDisplayModel> GetDisplayEntries(int columnId, long fromOrderInclusive, int count, bool includeDeleted)
        {
            var context = new TableContext(path);

            var entryEntities = context.Entries
                .Include(e => e.Tags.Where(t => (includeDeleted || !t.IsDeleted)).OrderBy(t => t.Name))
                .Include(e => e.Comments.Where(c => (includeDeleted || !c.IsDeleted)).OrderByDescending(c => c.Added))
                .Where(e => e.ColumnId == columnId && (includeDeleted || !e.IsDeleted) && e.Order >= fromOrderInclusive)
                .OrderBy(e => e.Order)
                .Take(count)
                .ToList();

            var result = mapper.Map<List<EntryDisplayModel>>(entryEntities);

            return result;
        }

        public override List<EntryModel> GetEntries(int columnId, bool includeDeleted)
        {
            var context = new TableContext(path);

            var entryEntities = context.Entries
                .Where(e => e.Column.Id == columnId && (includeDeleted || !e.IsDeleted))
                .OrderBy(e => e.Order)
                .ToList();

            return mapper.Map<List<EntryModel>>(entryEntities);
        }

        public override List<EntryModel> GetEntries(int columnId, int skip, int take, bool includeDeleted)
        {
            var context = new TableContext(path);

            var entries = context.Entries
                .Where(e => e.ColumnId == columnId && (includeDeleted || !e.IsDeleted))
                .OrderBy(e => e.Order)
                .Skip(skip)
                .Take(take)
                .ToList();

            var result = mapper.Map<List<EntryModel>>(entries);
            return result;
        }

        public override EntryEditModel GetEntryEdit(int entryId)
        {
            var context = new TableContext(path);

            var entry = context.Entries
                .Include(e => e.Tags.Where(t => !t.IsDeleted))
                .Include(e => e.Comments.Where(c => !c.IsDeleted))
                .FirstOrDefault(e => e.Id == entryId);

            var result = mapper.Map<EntryEditModel>(entry);
            return result;
        }

        public override EntryModel GetEntryById(int id)
        {
            var context = new TableContext(path);

            var entry = context.Entries.First(e => e.Id == id && !e.IsDeleted);
            var result = mapper.Map<EntryModel>(entry);
            return result;
        }

        public override int GetEntryCount(int columnId, bool includeDeleted)
        {
            var context = new TableContext(path);

            return context.Entries
                .Count(e => e.ColumnId == columnId && (includeDeleted || !e.IsDeleted));
        }

        public override long GetFirstEntryOrder(int columnId, bool includeDeleted)
        {
            var context = new TableContext(path);

            if (GetEntryCount(columnId, includeDeleted) == 0)
                return 0;

            return context.Entries
                .Where(e => e.ColumnId == columnId && (includeDeleted || !e.IsDeleted))
                .Min(e => e.Order);
        }

        public override EntryDisplayModel GetEntryDisplay(int entryId)
        {
            var context = new TableContext(path);

            var entry = context.Entries
                .Include(e => e.Tags.Where(t => !t.IsDeleted))
                .Include(e => e.Comments.Where(c => !c.IsDeleted))
                .First(e => e.Id == entryId && !e.IsDeleted);
            var result = mapper.Map<EntryDisplayModel>(entry);
            return result;
        }

        public override long GetLastEntryOrder(int columnId, bool includeDeleted)
        {
            var context = new TableContext(path);

            if (GetEntryCount(columnId, includeDeleted) == 0)
                return 0;

            return context.Entries
                .Where(e => e.ColumnId == columnId && (includeDeleted || !e.IsDeleted))
                .Max(e => e.Order);
        }

        public override EntryModel GetNextEntry(EntryModel entry, bool includeDeleted)
        {
            var context = new TableContext(path);

            var entryEntity = context.Entries
                .Where(e => e.ColumnId == entry.ColumnId && e.Order > entry.Order && (includeDeleted || !e.IsDeleted))
                .OrderBy(e => e.Order)
                .FirstOrDefault();

            if (entryEntity == null)
                return null;

            return mapper.Map<EntryModel>(entryEntity);
        }

        public override void RemoveTagFromEntry(int entryId, int tagId)
        {
            var context = new TableContext(path);

            var entry = context.Entries
                .Include(e => e.Tags)
                .First(e => e.Id == entryId);
                
            var tag = context.Tags.Find(tagId);
            entry.Tags.Remove(tag);
            context.SaveChanges();
        }

        public override void UpdateEntries(List<EntryModel> updatedItems)
        {
            var context = new TableContext(path);

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
            var context = new TableContext(path);

            var entry = context.Entries.First(e => e.Id == updatedEntry.Id);
            mapper.Map(updatedEntry, entry);
            context.SaveChanges();
        }

        #endregion

        #region Tags

        public override void AddTag(TagModel newTag)
        {
            var context = new TableContext(path);

            var tag = mapper.Map<Tag>(newTag);
            context.Tags.Add(tag);
            context.SaveChanges();
            mapper.Map(tag, newTag);
        }

        public override void DeleteTag(int tagId, bool permanent)
        {
            var context = new TableContext(path);

            var tag = context.Tags.First(e => e.Id == tagId);

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
            var context = new TableContext(path);

            var tagEntries = context.Tags
                .Where(t => t.TableId == tableId && (includeDeleted || !t.IsDeleted))
                .OrderBy(t => t.Name)
                .ToList();

            var result = mapper.Map<List<TagModel>>(tagEntries);
            return result;
        }

        public override TagModel GetTag(int tagId)
        {
            var context = new TableContext(path);

            return mapper.Map<TagModel>(context.Tags.Find(tagId));
        }

        public override void UpdateTag(TagModel updatedTag)
        {
            var context = new TableContext(path);

            var tag = context.Tags.Find(updatedTag.Id);
            mapper.Map(updatedTag, tag);
            context.SaveChanges();
        }

        #endregion

        #region Comments

        public override void AddComment(CommentModel commentModel)
        {
            var context = new TableContext(path);

            var comment = mapper.Map<Comment>(commentModel);
            context.Add<Comment>(comment);
            context.SaveChanges();
            mapper.Map(comment, commentModel);
        }

        public override void UpdateComment(CommentModel updatedComment)
        {
            var context = new TableContext(path);

            var comment = context.Comments.Find(updatedComment.Id);
            mapper.Map(updatedComment, comment);
            context.SaveChanges();
        }

        public override void DeleteComment(int commentId, bool permanent)
        {
            var context = new TableContext(path);

            var comment = context.Comments.Find(commentId);

            if (permanent)
            {
                context.Remove<Comment>(comment);
                context.SaveChanges();
            }
            else
            {
                comment.IsDeleted = true;
                context.SaveChanges();
            }
        }

        public override CommentModel GetComment(int commentId)
        {
            var context = new TableContext(path);

            return mapper.Map<CommentModel>(context.Comments.Find(commentId));
        }

        #endregion
    }
}
