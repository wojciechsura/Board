using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.ViewModels.Base;
using Board.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Tag
{
    public class TagViewModel : ModelEditorViewModel<TagModel>
    {
        private readonly TagModel tag;

        public TagViewModel(TagModel tag)
        {
            this.tag = tag;
        }

        public string Name => tag.Name;
        public int Color => tag.Color;
    }
}
