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
    public class NewInplaceCommentViewModel : BaseCommentViewModel
    {
        private readonly bool isNew;
        private readonly CommentModel comment;

        public NewInplaceCommentViewModel(IDocumentHandler handler, CommentModel comment, bool isNew)
            : base(handler)
        {
            this.comment = comment;
            this.isNew = isNew;

            SaveCommand = new AppCommand(obj => handler.SaveNewInplaceCommentRequest(this));
            CancelCommand = new AppCommand(obj => handler.CancelNewInplaceCommentRequest(this));
        }

        public string Content
        {
            get => comment.Content;
            set => comment.Content = value;
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public CommentModel Comment => comment;
    }
}
