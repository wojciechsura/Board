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
using Spooksoft.VisualStateManager.Conditions;

namespace Board.BusinessLogic.ViewModels.ColumnEditor
{
    public class ColumnEditorWindowViewModel : BaseViewModel
    {
        private readonly IColumnEditorWindowAccess access;
        private readonly ColumnModel column;
        private string name;
        private string limitVisibleItemsCount;
        private bool isLimitVisibleItems;
        private bool dimItems;
        private readonly BaseCondition limitVisibleItemsCountValidCondition;
        private readonly BaseCondition formValidCondition;

        private void DoOk()
        {
            UpdateToModel(column);
            access.Close(true);
        }

        private void DoCancel()
        {
            access.Close(false);
        }

        protected override void UpdateFromModel<TModel>(TModel model)
        {
            base.UpdateFromModel(model);

            var columnModel = model as ColumnModel;

            limitVisibleItemsCount = (columnModel.LimitShownItems ?? 0).ToString();
            isLimitVisibleItems = columnModel.LimitShownItems.HasValue;
        }

        protected override void UpdateToModel<TModel>(TModel model)
        {
            base.UpdateToModel(model);

            var columnModel = model as ColumnModel;

            columnModel.LimitShownItems = isLimitVisibleItems ? int.Parse(limitVisibleItemsCount) : null;
        }

        private static bool IsStringValidInt(string str) => int.TryParse(str, out _);

        public ColumnEditorWindowViewModel(IColumnEditorWindowAccess access, ColumnModel column, bool isNew)
        {
            this.access = access;
            this.column = column ?? throw new ArgumentNullException(nameof(column));

            if (isNew)
                Title = Strings.ColumnEditor_Title_New;
            else
                Title = Strings.ColumnEditor_Title_Edit;

            limitVisibleItemsCountValidCondition = new ChainedLambdaCondition<ColumnEditorWindowViewModel>(this, vm => !vm.IsLimitVisibleItems || (vm.IsLimitVisibleItems && IsStringValidInt(vm.LimitVisibleItemsCount)), false);
            formValidCondition = limitVisibleItemsCountValidCondition;

            OkCommand = new AppCommand(obj => DoOk(), formValidCondition);
            CancelCommand = new AppCommand(obj => DoCancel());

            UpdateFromModel(column);
        }

        [SyncWithModel(nameof(ColumnModel.Name))]
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        public bool IsLimitVisibleItems
        {
            get => isLimitVisibleItems;
            set => Set(ref isLimitVisibleItems, value);
        }

        public string LimitVisibleItemsCount
        {
            get => limitVisibleItemsCount;
            set => Set(ref limitVisibleItemsCount, value);
        }

        [SyncWithModel(nameof(ColumnModel.DimItems))]
        public bool DimItems
        {
            get => dimItems;
            set => Set(ref dimItems, value);
        }

        public ICommand OkCommand { get; }

        public ICommand CancelCommand { get; }

        public string Title { get; }

        public ColumnModel Result => column;
    }
}
