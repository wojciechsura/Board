using Board.BusinessLogic.Infrastructure.Collections;
using Board.BusinessLogic.ViewModels.Document;
using Board.Models.Data;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class InplaceCommentEditorViewModel : BaseCommentViewModel
    {
        private readonly bool isNew;
        private readonly CommentModel comment;

        private void DoSave(IEntryEditorHandler handler)
        {
            comment.Added = DateTime.Now;
            comment.Modified = comment.Added;
            handler.SaveCommentRequest(this);
        }

        public InplaceCommentEditorViewModel(IEntryEditorHandler handler, CommentModel comment, bool isNew)
            : base(handler)
        {
            this.comment = comment;
            this.isNew = isNew;

            SaveCommand = new AppCommand(obj => DoSave(handler));
            CancelCommand = new AppCommand(obj => handler.CancelCommentRequest(this));
        }

        public string Content
        {
            get => comment.Content;
            set => comment.Content = value;
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public CommentModel Comment => comment;

        public bool IsNew => isNew;
    }
}
