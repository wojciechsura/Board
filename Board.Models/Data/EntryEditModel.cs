using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Models.Data
{
    public class EntryEditModel
    {
        public int Id { get; set; }
        public int ColumnId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public long Order { get; set; }
        public List<TagModel> Tags { get; set; }
    }
}
