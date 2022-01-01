using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Data.Entities
{
    [Index(nameof(Id), IsUnique = true)]
    public class Column
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public Table? Table { get; set; }
        public List<Entry> Entries { get; set; } = new List<Entry>();
        public bool IsDeleted { get; set; }
    }
}
