using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Data.Entities
{
    [Index(nameof(Id), IsUnique = true)]
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public DateTime Added { get; set; }
        public DateTime Modified { get; set; }
        public string Content { get; set; }
        [Required]
        public Entry Entry { get; set; }
        public int EntryId { get; set; }
    }
}
