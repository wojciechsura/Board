using Board.BusinessLogic.Infrastructure.Collections;
using Board.Models.Data;
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
using System.IO;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class TableViewModel : BaseViewModel
    {
        private readonly TableModel table;
        private readonly MemoryStream background;
        private readonly ObservableParentedCollection<ColumnViewModel, TableViewModel> columns;
        private readonly IDocumentHandler handler;

        public TableViewModel(TableModel table, MemoryStream background, List<ColumnViewModel> columns, IDocumentHandler handler)
        {
            this.columns = new(this);
            this.table = table;
            this.background = background;
            this.handler = handler;

            foreach (var column in columns)
                this.columns.Add(column);

            NewColumnCommand = new AppCommand(obj => handler.NewColumnRequest(this));
        }

        public void RequestMoveColumn(ColumnViewModel columnViewModel, int newIndex)
        {
            handler.RequestMoveColumn(columnViewModel, this, newIndex);
        }

        public ObservableParentedCollection<ColumnViewModel, TableViewModel> Columns => columns;

        public string Name => table.Name;

        public MemoryStream Background => background;

        public TableModel Table => table;

        public ICommand NewColumnCommand { get; }
    }
}
