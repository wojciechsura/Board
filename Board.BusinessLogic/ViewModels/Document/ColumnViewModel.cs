using Board.BusinessLogic.Models.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class ColumnViewModel
    {
        private readonly ColumnModel column;
        private readonly ObservableCollection<EntryViewModel> entries = new();

        public ColumnViewModel(ColumnModel column, List<EntryViewModel> entries)
        {
            this.column = column;

            foreach (var entry in entries)
                this.entries.Add(entry);
        }

        public ObservableCollection<EntryViewModel> Entries => entries;

        public string Name => column.Name;
    }
}
