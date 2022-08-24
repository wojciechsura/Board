using Board.BusinessLogic.Infrastructure.Collections;
using Board.Models.Data;
using Board.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class EntryViewModel : BaseEntryViewModel
    {
        private readonly EntryDisplayModel entry;

        public EntryViewModel(EntryDisplayModel entry, IDocumentHandler handler)
            : base(handler)
        {
            this.entry = entry;

            Tags = new ObservableCollection<TagViewModel>();
            foreach (var tag in entry.Tags)
            {
                var tagViewModel = new TagViewModel(tag);
                Tags.Add(tagViewModel);
            }

            DeleteEntryCommand = new AppCommand(obj => handler.DeleteEntryRequest(this));
            EditEntryCommand = new AppCommand(obj => handler.EditEntryRequest(this));
        }

        public bool CanDragDrop => handler.CanDragDrop;
        public string Title => entry.Title;
        public DateTime? StartDate => entry.StartDate;
        public DateTime? EndDate => entry.EndDate;
        public bool AnyDateSet => entry.StartDate != null || entry.EndDate != null;
        public bool IsDone => entry.IsDone;
        public bool IsHighPriority => entry.IsHighPriority;
        public bool IsOverdue => !IsDone && entry.EndDate != null && entry.EndDate.Value < DateTime.Now;

        public ICommand DeleteEntryCommand { get; }
        public ICommand EditEntryCommand { get; }
        public EntryDisplayModel Entry => entry;

        public ObservableCollection<TagViewModel> Tags { get; }

        public bool ShowDescriptionIcon => !string.IsNullOrEmpty(entry.Description);
        public bool ShowCommentsIcon => entry.CommentCount > 0;
        public int CommentCount => entry.CommentCount;
        public bool HasDetails => ShowCommentsIcon || ShowDescriptionIcon;
    }
}
