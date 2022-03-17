using Board.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Board.BusinessLogic.Infrastructure.Document.Database
{
    public abstract class BaseDatabase
    {
        #region Tables

        public abstract void AddTable(TableModel newTable);
        public abstract void DeleteTable(int tableId, bool permanent);

        public abstract long GetFirstTableOrder(bool includeDeleted);
        public abstract long GetLastTableOrder(bool includeDeleted);
        public abstract TableModel GetNextTable(TableModel table, bool includeDeleted);
        public abstract TableModel GetTable(int tableId);
        public abstract int GetTableCount(bool includeDeleted);
        public abstract List<TableModel> GetTables(bool includeDeleted);
        public abstract List<TableModel> GetTables(int skip, int take, bool includeDeleted);
        public abstract void UpdateTable(TableModel updatedTable);
        public abstract void UpdateTables(List<TableModel> updatedItems);

        #endregion

        #region Columns

        public abstract void AddColumn(ColumnModel newColumn);
        public abstract void DeleteColumn(int columnId, bool permanent);
        public abstract ColumnModel GetColumn(int columnId);
        public abstract int GetColumnCount(int tableId, bool includeDeleted);
        public abstract List<ColumnModel> GetColumns(int tableId, bool includeDeleted);
        public abstract List<ColumnModel> GetColumns(int tableId, int skip, int take, bool includeDeleted);
        public abstract long GetFirstColumnOrder(int tableId, bool includeDeleted);
        public abstract long GetLastColumnOrder(int tableId, bool includeDeleted);
        public abstract ColumnModel GetNextColumn(ColumnModel column, bool includeDeleted);
        public abstract void UpdateColumn(ColumnModel updatedColumn);
        public abstract void UpdateColumns(List<ColumnModel> updatedItems);

        #endregion

        #region Entries

        public abstract void AddEntry(EntryModel newEntry);
        public abstract void AddTagToEntry(int entryId, int tagId);
        public abstract void DeleteEntry(int entryId, bool permanent);
        public abstract List<EntryDisplayModel> GetDisplayEntries(int columnId, string filter, bool includeDeleted);
        public abstract List<EntryDisplayModel> GetDisplayEntries(int columnId, long fromOrderInclusive, int count, string filter, bool includeDeleted);
        public abstract List<EntryModel> GetEntries(int columnId, bool includeDeleted);
        public abstract List<EntryModel> GetEntries(int columnId, int skip, int take, bool includeDeleted);
        public abstract EntryModel GetEntryById(int id);
        public abstract int GetEntryCount(int columnId, string filter, bool includeDeleted);
        public abstract EntryDisplayModel GetEntryDisplay(int entryId);
        public abstract EntryEditModel GetEntryEdit(int entryId);
        public abstract long GetFirstEntryOrder(int columnId, bool includeDeleted);
        public abstract long GetLastEntryOrder(int columnId, bool includeDeleted);
        public abstract EntryModel GetNextEntry(EntryModel entry, bool includeDeleted);
        public abstract void RemoveTagFromEntry(int entryId, int tagId);
        public abstract void UpdateEntries(List<EntryModel> updatedItems);
        public abstract void UpdateEntry(EntryModel updatedEntry);

        #endregion

        #region Tags

        public abstract void AddTag(TagModel newTag);
        public abstract void DeleteTag(int tagId, bool permanent);
        public abstract List<TagModel> GetTags(int tableId, bool includeDeleted);
        public abstract void UpdateTag(TagModel updatedTag);
        public abstract TagModel GetTag(int tagId);

        #endregion

        #region Comments

        public abstract void AddComment(CommentModel commentModel);
        public abstract void UpdateComment(CommentModel commentModel);
        public abstract void DeleteComment(int commentId, bool permanent);
        public abstract CommentModel GetComment(int commentId);

        #endregion
    }
}
