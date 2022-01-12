using Board.BusinessLogic.ViewModels.Base;
using Board.BusinessLogic.ViewModels.Document;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class BaseCommentViewModel : BaseViewModel
    {
        protected readonly IEntryEditorHandler handler;

        public BaseCommentViewModel(IEntryEditorHandler handler)
        {
            this.handler = handler;
        }
    }
}