using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace Board.Data.Entities
{
    [Index(nameof(Id), IsUnique = true)]
    public class Tag
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int Color { get; set; }
        public bool IsDeleted { get; set; }

        public Table Table { get; set; }
        public int TableId { get; set; }
        public virtual List<Entry> Entries { get; set; }
    }
}
