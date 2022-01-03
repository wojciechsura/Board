using Board.BusinessLogic.Infrastructure.Collections;
using Board.BusinessLogic.Models.Data;
using Board.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class EntryViewModel : BaseViewModel, IParentedItem<ColumnViewModel>
    {
        private readonly EntryModel entry;
        private readonly IDocumentHandler handler;

        public EntryViewModel(EntryModel entry, IDocumentHandler handler)
        {
            this.entry = entry;
            this.handler = handler;
        }

        public ColumnViewModel Parent { get; set; }

        public string Title => entry.Title;
    }
}
