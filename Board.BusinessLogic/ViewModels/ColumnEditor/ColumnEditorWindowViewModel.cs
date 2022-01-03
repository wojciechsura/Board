﻿using Board.BusinessLogic.Models.Data;
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

namespace Board.BusinessLogic.ViewModels.ColumnEditor
{
    public class ColumnEditorWindowViewModel : ModelEditorViewModel<ColumnModel>
    {
        private readonly IColumnEditorWindowAccess access;
        private readonly ColumnModel editedColumn;

        private string name;

        private void DoOk()
        {
            UpdatePropertiesToModel(editedColumn);
            access.Close(true);
        }

        private void DoCancel()
        {
            access.Close(false);
        }

        public ColumnEditorWindowViewModel(IColumnEditorWindowAccess access, ColumnModel editedColumn, bool isNew)
        {
            this.access = access;
            this.editedColumn = editedColumn ?? throw new ArgumentNullException(nameof(editedColumn));

            if (isNew)
                Title = Strings.ColumnEditor_Title_New;
            else
                Title = Strings.ColumnEditor_Title_Edit;

            OkCommand = new AppCommand(obj => DoOk());
            CancelCommand = new AppCommand(obj => DoCancel());

            UpdatePropertiesFromModel(editedColumn);
        }

        [RepresentsModelProperty(nameof(ColumnModel.Name))]
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        public ICommand OkCommand { get; }

        public ICommand CancelCommand { get; }

        public string Title { get; }

        public ColumnModel Result => editedColumn;
    }
}