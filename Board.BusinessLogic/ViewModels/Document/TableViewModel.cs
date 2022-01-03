using Board.BusinessLogic.Infrastructure.Collections;
using Board.BusinessLogic.Models.Data;
using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class TableViewModel : BaseViewModel
    {
        private readonly TableModel table;
        private readonly ObservableParentedCollection<ColumnViewModel, TableViewModel> columns;
        private readonly IDocumentHandler handler;

        public TableViewModel(TableModel table, List<ColumnViewModel> columns, IDocumentHandler handler)
        {
            this.columns = new(this);
            this.table = table;
            this.handler = handler;

            foreach (var column in columns)
                this.columns.Add(column);

            NewColumnCommand = new AppCommand(obj => handler.NewColumnRequest(this));
        }

        public ObservableParentedCollection<ColumnViewModel, TableViewModel> Columns => columns;

        public string Name => table.Name;

        public TableModel Table => table;

        public ICommand NewColumnCommand { get; }
    }
}
