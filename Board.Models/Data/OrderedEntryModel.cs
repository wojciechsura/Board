using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Models.Data
{
    public class OrderedEntryModel : EntryModel, IOrderedModel
    {
        public long Order { get; set; }
    }
}
