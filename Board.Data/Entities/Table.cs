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
    public class Table
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public List<Column> Columns { get; set; } = new List<Column>();
    }
}
