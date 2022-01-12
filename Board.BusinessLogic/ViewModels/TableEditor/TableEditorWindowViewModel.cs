using Board.Models.Data;
using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.ViewModels.Base;
using Board.Resources;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.TableEditor
{
    public class TableEditorWindowViewModel : BaseViewModel
    {
        private readonly ITableEditorWindowAccess access;
        private readonly TableModel table;

        private string name;

        private void DoOk()
        {
            UpdateToModel(table);
            access.Close(true);
        }

        private void DoCancel()
        {
            access.Close(false);
        }

        public TableEditorWindowViewModel(ITableEditorWindowAccess access, TableModel table, bool isNew)
        {
            this.access = access;
            this.table = table ?? throw new ArgumentNullException(nameof(table));

            if (isNew)
                Title = Strings.TableEditor_Title_New;
            else
                Title = Strings.TableEditor_Title_Edit;

            OkCommand = new AppCommand(obj => DoOk());
            CancelCommand = new AppCommand(obj => DoCancel());

            UpdateFromModel(table);
        }

        [SyncWithModel(nameof(TableModel.Name))]
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        public ICommand OkCommand { get; }

        public ICommand CancelCommand { get; }

        public string Title { get; }

        public TableModel Result => table;
    }
}
