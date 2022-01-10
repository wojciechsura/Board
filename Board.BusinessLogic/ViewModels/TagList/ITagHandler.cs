﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.TagList
{
    public interface ITagHandler
    {
        void RequestEdit(TagViewModel tagViewModel);
        void RequestDelete(TagViewModel tagViewModel);
    }
}
