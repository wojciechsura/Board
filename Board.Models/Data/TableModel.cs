using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Models.Data
{
    public class TableModel : IOrderedModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public long Order { get; set; }
    }
}
