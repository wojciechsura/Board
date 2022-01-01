using Board.BusinessLogic.Models.Data;
using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Main.Document
{
    public class TableViewModel : BaseViewModel
    {
        private const string MODEL_GROUP = "Model";

        private ObservableCollection<ColumnViewModel> columns = new();
        private TableModel table;

        public TableViewModel(TableModel table, List<ColumnViewModel> columns)
        {
            this.table = table;

            foreach (var column in columns)
                this.columns.Add(column);
        }

        public void NotifyModelUpdated()
        {
            PropertyGroupChanged(MODEL_GROUP);
        }

        public ObservableCollection<ColumnViewModel> Columns => columns;

        [PropertyNotificationGroup(MODEL_GROUP)]
        public string Name => table.Name;
    }
}
