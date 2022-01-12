using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.ViewModels.Base;
using Board.BusinessLogic.ViewModels.Document;
using Board.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class CommentViewModel : BaseCommentViewModel
    {
        private readonly CommentModel comment;

        [SyncWithModel(nameof(CommentModel.Content))]
        private string content;
        [SyncWithModel(nameof(CommentModel.Added))]
        private DateTime added;

        public CommentViewModel(CommentModel comment, IEntryEditorHandler handler)
            : base(handler)
        {
            this.comment = comment;

            UpdateFromModel(comment);
        }

        public string Content => content;
        public string Added => added.ToString("dd-MM-yyyy");
    }
}
