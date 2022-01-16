using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace Board.Models.Data
{
    public class TableEditModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public long Order { get; set; }
        public string Background { get; set; }

        public MemoryStream BackgroundStream { get; set; } = null;
        public bool BackgroundChanged { get; set; } = false;
    }
}
