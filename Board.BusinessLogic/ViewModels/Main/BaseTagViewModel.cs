using Board.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class BaseTagViewModel
    {
        private readonly TagModel tag;
        private readonly IEntryEditorHandler handler;

        public BaseTagViewModel(TagModel tag, IEntryEditorHandler handler)
        {
            this.tag = tag;
            this.handler = handler;
        }

        public string Name => tag.Name;
        public int Color => tag.Color;
        public TagModel Tag => tag;
    }
}
