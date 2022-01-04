using Board.BusinessLogic.Infrastructure.Collections;
using Board.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class BaseEntryViewModel : BaseViewModel, IParentedItem<ColumnViewModel>
    {
        protected readonly IDocumentHandler handler;

        public BaseEntryViewModel(IDocumentHandler handler)
        {
            this.handler = handler;
        }

        public ColumnViewModel Parent { get; set; }
    }
}
