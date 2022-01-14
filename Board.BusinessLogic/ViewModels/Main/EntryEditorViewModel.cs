using Board.Models.Data;
using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.ViewModels.Base;
using Board.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Board.BusinessLogic.Infrastructure.Document;
using System.Windows.Input;
using Spooksoft.VisualStateManager.Commands;
using System.Collections.ObjectModel;
using Board.BusinessLogic.Services.Dialogs;
using Board.Resources;
using System.Text.RegularExpressions;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class EntryEditorViewModel : BaseViewModel, IEntryEditorHandler
    {
        // Private constants --------------------------------------------------

        private static Regex timeRegex = new Regex("^([01]?[0-9]|2[0-3]):([0-5][0-9])$");

        // Private fields -----------------------------------------------------

        private readonly int entryId;
        private readonly int tableId;
        private readonly WallDocument document;
        private readonly IDocumentHandler handler;
        private readonly IDialogService dialogService;
        private bool isEditingTitle = false;
        private bool isEditingDescription = false;
        private bool isEditingDates = false;

        [SyncWithModel(nameof(EntryModel.Title))]
        private string title;
        [SyncWithModel(nameof(EntryModel.Description))]
        private string description;

        private bool startDateSet;
        private DateTime startDate;
        private string startTime;
        private bool endDateSet;
        private DateTime endDate;
        private string endTime;

        // Private methods ----------------------------------------------------

        private void InsertTag<TTag>(ObservableCollection<TTag> tags, TTag tag)
            where TTag : BaseTagViewModel
        {
            int i = 0;
            while (i < tags.Count && string.Compare(tags[i].Name, tag.Name) < 0)
                i++;

            tags.Insert(i, tag);
        }

        private void UpdateFromModel(EntryEditModel model)
        {
            base.UpdateFromModel(model);

            var tags = document.Database.GetTags(tableId, false)
                .OrderBy(t => t.Name);

            foreach (var tag in tags)
            {
                AvailableTagViewModel availableTag = new AvailableTagViewModel(tag, this);

                if (model.Tags.Any(t => t.Id == tag.Id))
                {
                    AddedTags.Add(new AddedTagViewModel(tag, this));
                    availableTag.IsSelected = true;
                }

                AvailableTags.Add(availableTag);
            }

            Comments.Add(new InplaceCommentEditorViewModel(this, new CommentModel() { EntryId = entryId }, true));

            foreach (var comment in model.Comments.OrderByDescending(c => c.Added))
            {
                CommentViewModel commentViewModel = new(comment, this);
                Comments.Add(commentViewModel);
            }

            UpdateDates(model.StartDate, model.EndDate);
        }

        private void UpdateDates(DateTime? startDate, DateTime? endDate)
        {
            this.startDateSet = startDate != null;
            this.startDate = startDate ?? DateTime.Now;
            this.startTime = startDate?.ToString("HH:mm") ?? DateTime.Now.ToString("HH:mm");

            this.endDateSet = endDate != null;
            this.endDate = endDate ?? DateTime.Now;
            this.endTime = endDate?.ToString("HH.mm") ?? DateTime.Now.ToString("HH:mm");
        }

        private void NotifyDatesChanged()
        {
            OnPropertyChanged(nameof(StartDateSet));
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(StartTime));
            OnPropertyChanged(nameof(EndDateSet));
            OnPropertyChanged(nameof(EndDate));
            OnPropertyChanged(nameof(EndTime));
        }

        private void ReplaceCommentEditor(InplaceCommentEditorViewModel inplaceCommentEditorViewModel, CommentModel commentModel)
        {
            var commentViewModel = new CommentViewModel(commentModel, this);
            var index = Comments.IndexOf(inplaceCommentEditorViewModel);
            Comments.RemoveAt(index);
            Comments.Insert(index, commentViewModel);
        }

        private void HandleDatesChanged()
        {
            IsEditingDates = true;
        }

        private (int h, int m) SanitizeTime(string startTime)
        {
            int h = 0, m = 0;
            if (timeRegex.IsMatch(startTime))
            {
                string[] parts = startTime.Split(':');
                h = int.Parse(parts[0]);
                m = int.Parse(parts[1]);
            }

            return (h, m);
        }

        // IEntryEditorHandler ------------------------------------------------

        void IEntryEditorHandler.ToggleTag(AvailableTagViewModel tag)
        {
            if (tag.IsSelected)
            {
                document.Database.RemoveTagFromEntry(entryId, tag.Tag.Id);
                
                tag.IsSelected = false;

                var addedTag = AddedTags.FirstOrDefault(t => t.Tag.Id == tag.Tag.Id);
                AddedTags.Remove(addedTag);
            }
            else
            {
                document.Database.AddTagToEntry(entryId, tag.Tag.Id);

                var addedTag = new AddedTagViewModel(tag.Tag, this);
                InsertTag(AddedTags, addedTag);
                tag.IsSelected = true;
            }
        }

        void IEntryEditorHandler.SaveCommentRequest(InplaceCommentEditorViewModel inplaceCommentEditorViewModel)
        {
            if (inplaceCommentEditorViewModel.IsNew)
            {
                // Add new comment
                var commentModel = inplaceCommentEditorViewModel.Comment;
                document.Database.AddComment(commentModel);

                ReplaceCommentEditor(inplaceCommentEditorViewModel, commentModel);

                Comments.Insert(0, new InplaceCommentEditorViewModel(this, new CommentModel() { EntryId = entryId }, true));
            }
            else
            {
                var commentModel = inplaceCommentEditorViewModel.Comment;
                document.Database.UpdateComment(commentModel);

                ReplaceCommentEditor(inplaceCommentEditorViewModel, commentModel);
            }
        }

        void IEntryEditorHandler.CancelCommentRequest(InplaceCommentEditorViewModel inplaceCommentEditorViewModel)
        {
            if (inplaceCommentEditorViewModel.IsNew)
                throw new InvalidOperationException("Cannot cancel a new comment.");

            var commentModel = document.Database.GetComment(inplaceCommentEditorViewModel.Comment.Id);
            ReplaceCommentEditor(inplaceCommentEditorViewModel, commentModel);
        }

        void IEntryEditorHandler.EditCommentRequest(CommentViewModel commentViewModel)
        {
            var inplaceEditorViewModel = new InplaceCommentEditorViewModel(this, commentViewModel.Comment, false);
            var index = Comments.IndexOf(commentViewModel);
            Comments.RemoveAt(index);
            Comments.Insert(index, inplaceEditorViewModel);
        }

        void IEntryEditorHandler.DeleteCommentRequest(CommentViewModel commentViewModel)
        {
            (bool result, bool? permanent) = dialogService.ShowDeleteDialog(string.Format(Strings.Message_CommentDeletion, commentViewModel.Added));
            if (result)
            {
                document.Database.DeleteComment(commentViewModel.Comment.Id, permanent.Value);
                Comments.Remove(commentViewModel);
            }
        }

        // Public methods -----------------------------------------------------

        public EntryEditorViewModel(int entryId, 
            int columnId,
            int tableId,
            EntryViewModel editedEntryViewModel,
            WallDocument document,
            IDocumentHandler handler,
            IDialogService dialogService)
        {
            var columnModel = document.Database.GetColumn(columnId);
            ColumnName = columnModel.Name;

            this.entryId = entryId;
            this.tableId = tableId;
            this.document = document;
            this.handler = handler;
            this.dialogService = dialogService;

            CloseCommand = new AppCommand(obj => handler.RequestEditorClose(editedEntryViewModel));

            AddedTags = new ();
            AvailableTags = new ();
            Comments = new();

            var editEntryModel = document.Database.GetEntryEdit(entryId);
            UpdateFromModel(editEntryModel);
        }

        public void SetTitle(string title)
        {
            var model = document.Database.GetEntryById(entryId);
            model.Title = title;
            document.Database.UpdateEntry(model);

            Title = title;
        }

        public void SetDescription(string description)
        {
            var model = document.Database.GetEntryById(entryId);
            model.Description = description;
            document.Database.UpdateEntry(model);

            Description = description;
        }

        public void CancelDateChanges()
        {
            var model = document.Database.GetEntryEdit(entryId);
            UpdateDates(model.StartDate, model.EndDate);
            NotifyDatesChanged();

            IsEditingDates = false;
        }

        public void CommitDateChanges()
        {
            var model = document.Database.GetEntryById(entryId);

            DateTime? startDate = null;

            if (startDateSet)
            {
                (int h, int m) = SanitizeTime(startTime);

                startDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, h, m, 0);
                model.StartDate = startDate;
            }
            else
            {
                model.StartDate = null;
            }

            if (endDateSet)
            {
                (int h, int m) = SanitizeTime(endTime);

                var endDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, h, m, 0);
                if (startDate != null && endDate < startDate)
                    endDate = startDate.Value;

                model.EndDate = endDate;
            }
            else
            {
                model.EndDate = null;
            }

            document.Database.UpdateEntry(model);

            UpdateDates(model.StartDate, model.EndDate);
            NotifyDatesChanged();

            IsEditingDates = false;
        }

        // Public properties --------------------------------------------------

        public string ColumnName { get; }

        public string Title
        {
            get => title;
            private set => Set(ref title, value);
        }

        public string Description
        {
            get => description;
            set => Set(ref description, value);
        } 

        public bool IsEditingTitle
        {
            get => isEditingTitle;
            set => Set(ref isEditingTitle, value);
        }

        public bool IsEditingDescription
        {
            get => isEditingDescription;
            set => Set(ref isEditingDescription, value);
        }

        public bool IsEditingDates
        {
            get => isEditingDates;
            set => Set(ref isEditingDates, value);
        }

        public ObservableCollection<AddedTagViewModel> AddedTags { get; }

        public ObservableCollection<AvailableTagViewModel> AvailableTags { get; }

        public ObservableCollection<BaseCommentViewModel> Comments { get; }

        public ICommand CloseCommand { get; }

        public bool StartDateSet
        {
            get => startDateSet; 
            set => Set(ref startDateSet, value, changeHandler: HandleDatesChanged);
        }

        public DateTime StartDate
        {
            get => startDate; 
            set => Set(ref startDate, value, changeHandler: HandleDatesChanged);
        }

        public string StartTime
        {
            get => startTime; 
            set => Set(ref startTime, value, changeHandler: HandleDatesChanged);
        }

        public bool EndDateSet
        {
            get => endDateSet; 
            set => Set(ref endDateSet, value, changeHandler: HandleDatesChanged);
        }

        public DateTime EndDate
        {
            get => endDate; 
            set => Set(ref endDate, value, changeHandler: HandleDatesChanged);
        }

        public string EndTime
        {
            get => endTime; 
            set => Set(ref endTime, value, changeHandler: HandleDatesChanged);
        }
    }
}
