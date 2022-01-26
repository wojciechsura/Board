using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Models.Data
{
    public class ColumnModel : IOrderedModel
    {
        public string? Name { get; set; }
        public int TableId { get; set; }
        public int Id { get; set; }
        public long Order { get; set; }
        public int? LimitShownItems { get; set; }
    }
}
