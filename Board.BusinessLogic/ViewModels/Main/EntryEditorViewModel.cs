﻿using Board.Models.Data;
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
using Board.BusinessLogic.Services.Config;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class EntryEditorViewModel : BaseViewModel, IEntryEditorHandler, IEntryDateEditorHandler
    {
        private const string DATES_PROPERTY_GROUP = "Dates";

        // Private fields -----------------------------------------------------

        private readonly EntryDateEditorViewModel dateEditorViewModel;

        private readonly int entryId;
        private readonly int tableId;
        private readonly EntryViewModel editedEntryViewModel;
        private readonly WallDocument document;
        private readonly IDocumentHandler handler;
        private readonly IDialogService dialogService;
        private readonly IConfigurationService configurationService;
        private bool isEditingTitle = false;
        private bool isEditingDescription = false;
        private bool areButtonsExpanded = true;

        [SyncWithModel(nameof(EntryModel.Title))]
        private string title;
        [SyncWithModel(nameof(EntryModel.Description))]
        private string description;
        [SyncWithModel(nameof(EntryModel.StartDate))]
        private DateTime? startDate;
        [SyncWithModel(nameof(EntryModel.EndDate))]
        private DateTime? endDate;
        [SyncWithModel(nameof(EntryModel.IsDone))]
        private bool isDone;
        [SyncWithModel(nameof(EntryModel.IsHighPriority))]
        private bool isHighPriority;
        private string addedDate;

        // Private methods ----------------------------------------------------

        private void DoClose()
        {
            Access.NotifyClosing();
            handler.RequestEditorClose(editedEntryViewModel);
        }

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

            // Tags

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

            // Comments

            Comments.Add(new InplaceCommentEditorViewModel(this, new CommentModel() { EntryId = entryId }, true));

            foreach (var comment in model.Comments.OrderByDescending(c => c.Added))
            {
                CommentViewModel commentViewModel = new(comment, this);
                Comments.Add(commentViewModel);
            }

            // Added date

            AddedDate = model.CreatedDate.ToString("dddd, yyyy-MM-dd");
        }

        private void ReplaceCommentEditor(InplaceCommentEditorViewModel inplaceCommentEditorViewModel, CommentModel commentModel)
        {
            var commentViewModel = new CommentViewModel(commentModel, this);
            var index = Comments.IndexOf(inplaceCommentEditorViewModel);
            Comments.RemoveAt(index);
            Comments.Insert(index, commentViewModel);
        }

        private void HandleIsDoneChanged()
        {
            var model = document.Database.GetEntryById(entryId);
            model.IsDone = isDone;
            document.Database.UpdateEntry(model);

            OnPropertyChanged(nameof(IsOverdue));
        }

        private void HandleIsHighPriorityChanged()
        {
            var model = document.Database.GetEntryById(entryId);
            model.IsHighPriority = isHighPriority;
            document.Database.UpdateEntry(model);

            OnPropertyChanged(nameof(IsHighPriority));
        }


        private void DoToggleButtonsExpanded()
        {
            areButtonsExpanded = !areButtonsExpanded;

            configurationService.Configuration.UI.EntryEditorButtonsExpanded = areButtonsExpanded;
            configurationService.Save();

            OnPropertyChanged(nameof(AreButtonsExpanded));
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

        // IEntryDateEditorHandler implementation -----------------------------

        (DateTime? startDate, DateTime? endDate) IEntryDateEditorHandler.GetCurrentDates()
        {
            return (startDate, endDate);
        }

        void IEntryDateEditorHandler.SetCurrentDates(DateTime? newStartDate, DateTime? newEndDate)
        {
            var model = document.Database.GetEntryById(entryId);
            model.StartDate = newStartDate;
            model.EndDate = newEndDate;
            document.Database.UpdateEntry(model);

            startDate = newStartDate;
            endDate = newEndDate;

            PropertyGroupChanged(DATES_PROPERTY_GROUP);
        }

        // Public methods -----------------------------------------------------

        public EntryEditorViewModel(int entryId, 
            int columnId,
            int tableId,
            EntryViewModel editedEntryViewModel,
            WallDocument document,
            IDocumentHandler handler,
            IDialogService dialogService,
            IConfigurationService configurationService)
        {
            var columnModel = document.Database.GetColumn(columnId);
            ColumnName = columnModel.Name;

            this.entryId = entryId;
            this.tableId = tableId;
            this.editedEntryViewModel = editedEntryViewModel;
            this.document = document;
            this.handler = handler;
            this.dialogService = dialogService;
            this.configurationService = configurationService;

            areButtonsExpanded = configurationService.Configuration.UI.EntryEditorButtonsExpanded;

            CloseCommand = new AppCommand(obj => DoClose());
            ToggleButtonsExpandedCommand = new AppCommand(obj => DoToggleButtonsExpanded());

            AddedTags = new();
            AvailableTags = new();
            Comments = new();

            var editEntryModel = document.Database.GetEntryEdit(entryId);
            UpdateFromModel(editEntryModel);

            dateEditorViewModel = new EntryDateEditorViewModel(editEntryModel.StartDate, editEntryModel.EndDate, this);
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

        // Public properties --------------------------------------------------

        public IEntryEditorAccess Access { get; set; }

        public string Id => $"#{entryId}";

        public EntryDateEditorViewModel DateEditorViewModel => dateEditorViewModel;

        public string ColumnName { get; }

        public string AddedDate
        {
            get => addedDate; 
            set => Set(ref addedDate, value);
        }

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

        public ObservableCollection<AddedTagViewModel> AddedTags { get; }

        public ObservableCollection<AvailableTagViewModel> AvailableTags { get; }

        public ObservableCollection<BaseCommentViewModel> Comments { get; }

        public ICommand CloseCommand { get; }

        public ICommand ToggleButtonsExpandedCommand { get; }

        [PropertyNotificationGroup(DATES_PROPERTY_GROUP)]
        public DateTime? StartDate => startDate;

        [PropertyNotificationGroup(DATES_PROPERTY_GROUP)]
        public string StartDateDisplay => startDate?.ToString("dddd, dd-MM-yyyy");

        [PropertyNotificationGroup(DATES_PROPERTY_GROUP)]
        public DateTime? EndDate => endDate;

        [PropertyNotificationGroup(DATES_PROPERTY_GROUP)]
        public string EndDateDisplay => endDate?.ToString("dddd, dd-MM-yyyy");

        [PropertyNotificationGroup(DATES_PROPERTY_GROUP)]
        public bool AnyDateSet => startDate != null || endDate != null;

        [PropertyNotificationGroup(DATES_PROPERTY_GROUP)]
        public bool IsOverdue => !isDone && endDate != null && endDate.Value < DateTime.Now;

        public bool IsDone
        {
            get => isDone;
            set => Set(ref isDone, value, changeHandler: HandleIsDoneChanged);
        }

        public bool IsHighPriority
        {
            get => isHighPriority;
            set => Set(ref isHighPriority, value, changeHandler: HandleIsHighPriorityChanged);
        }

        public bool AreButtonsExpanded
        {
            get => areButtonsExpanded;
        }
    }
}
