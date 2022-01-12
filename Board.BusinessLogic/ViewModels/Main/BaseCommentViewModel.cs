using Board.BusinessLogic.ViewModels.Base;
using Board.BusinessLogic.ViewModels.Document;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class BaseCommentViewModel : BaseViewModel
    {
        protected readonly IDocumentHandler handler;

        public BaseCommentViewModel(IDocumentHandler handler)
        {
            this.handler = handler;
        }
    }
}