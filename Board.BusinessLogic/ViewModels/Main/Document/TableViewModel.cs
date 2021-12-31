using Board.BusinessLogic.Models.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Main.Document
{
    public class TableViewModel
    {
        private ObservableCollection<ColumnViewModel> columns = new();
        private TableModel table;

        public TableViewModel(TableModel table, List<ColumnViewModel> columns)
        {
            this.table = table;

            foreach (var column in columns)
                this.columns.Add(column);
        }

        public ObservableCollection<ColumnViewModel> Columns => columns;

        public string Name => table.Name;
    }
}
