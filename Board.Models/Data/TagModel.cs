using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace Board.Models.Data
{
    public class TagModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Color { get; set; }
        public int TableId { get; set; }
    }
}
