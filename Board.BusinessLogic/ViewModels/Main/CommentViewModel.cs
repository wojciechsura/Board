using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.ViewModels.Base;
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
    public class CommentViewModel : BaseCommentViewModel
    {
        private readonly CommentModel comment;

#pragma warning disable CS0649 // Add readonly modifier
        [SyncWithModel(nameof(CommentModel.Content))]
        private string content;
        [SyncWithModel(nameof(CommentModel.Added))]
        private DateTime added;
#pragma warning restore CS0649 // Add readonly modifier

        public CommentViewModel(CommentModel comment, IEntryEditorHandler handler)
            : base(handler)
        {
            this.comment = comment;

            EditCommentCommand = new AppCommand(obj => handler.EditCommentRequest(this));
            DeleteCommentCommand = new AppCommand(obj => handler.DeleteCommentRequest(this));

            UpdateFromModel(comment);
        }

        public string Content => content;
        public string Added => added.ToString("dd-MM-yyyy, HH:mm:ss");
        public ICommand EditCommentCommand { get; }
        public ICommand DeleteCommentCommand { get; }
        public CommentModel Comment => comment;
    }
}
