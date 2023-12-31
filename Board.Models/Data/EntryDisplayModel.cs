﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace Board.Models.Data
{
    public class EntryDisplayModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDone { get; set; }
        public bool IsHighPriority { get; set; }
        public List<TagModel> Tags { get; set; }
        public int CommentCount { get; set; }
        public long Order { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
