using Board.BusinessLogic.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class EntryViewModel
    {
        private readonly EntryModel entry;

        public EntryViewModel(EntryModel entry)
        {
            this.entry = entry;
        }

        public string Title => entry.Title;
    }
}
