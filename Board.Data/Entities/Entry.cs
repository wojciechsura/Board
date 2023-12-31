﻿using Microsoft.EntityFrameworkCore;
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
    [Index(nameof(Order), new[] { nameof(ColumnId) }, Name = "IX_EntryOrder", IsUnique = true)]
    public class Entry
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public long Order { get; set; }
        [Required]
        public Column Column { get; set; }
        [ForeignKey(nameof(Column))]
        public int ColumnId { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDone { get; set; } = false;
        public bool IsHighPriority { get; set; } = false;

        public virtual List<Tag> Tags { get; set; }
        public virtual List<Comment> Comments { get; set; }
    }
}
