using Board.BusinessLogic.Models.Data;
using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class TableViewModel : BaseViewModel
    {
        private readonly TableModel table;
        private readonly ObservableCollection<ColumnViewModel> columns = new();

        public TableViewModel(TableModel table, List<ColumnViewModel> columns)
        {
            this.table = table;

            foreach (var column in columns)
                this.columns.Add(column);
        }

        public ObservableCollection<ColumnViewModel> Columns => columns;

        public string Name => table.Name;

        public TableModel Table => table;
    }
}
