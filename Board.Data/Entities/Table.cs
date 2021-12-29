using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Data.Entities
{
    [Index(nameof(Id), IsUnique = true)]
    public class Table
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<Entry> Entries { get; set; } = new List<Entry>();
    }
}
