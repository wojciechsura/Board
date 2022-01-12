using Board.Models.Data;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class AvailableTagViewModel : BaseTagViewModel
    {
        private bool isSelected;
        private readonly BaseCondition isSelectedCondition;

        public AvailableTagViewModel(TagModel tag, IEntryEditorHandler handler) : base(tag, handler)
        {
            isSelected = false;
            AddCommand = new AppCommand(obj => handler.ToggleTag(this));
        }

        public ICommand AddCommand { get; }
        public bool IsSelected
        {
            get => isSelected;
            set => Set(ref isSelected, value);
        }
    }
}
