using Board.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class TagViewModel
    {
        private readonly TagModel tag;

        public TagViewModel(TagModel tag)
        {
            this.tag = tag;
        }

        public string Name => tag.Name;

        public int Color => tag.Color;
        public TagModel Tag => tag;
    }
}
