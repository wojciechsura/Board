using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace Board.Data.Entities
{
    [Index(nameof(Id), IsUnique = true)]
    [Index(new[] { nameof(Order), nameof(TableId)}, Name = "IX_ColumnOrder", IsUnique = true)]
    public class Column
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }        
        [Required]
        public long Order { get; set; }
        public int? LimitShownItems { get; set; }
        [Required]
        public Table Table { get; set; }
        [ForeignKey(nameof(Table))]
        public int TableId { get; set; }
        public List<Entry> Entries { get; set; } = new List<Entry>();
        public bool IsDeleted { get; set; }
    }
}
