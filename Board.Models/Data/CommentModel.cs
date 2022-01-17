using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace Board.Models.Data
{
    public class CommentModel
    {
        public int Id { get; set; }
        public DateTime Added { get; set; }
        public DateTime Modified { get; set; }
        public string Content { get; set; }
        public int EntryId { get; set; }
    }
}
