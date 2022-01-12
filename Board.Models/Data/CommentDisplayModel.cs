using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Models.Data
{
    public class CommentDisplayModel
    {
        public int Id { get; set; }
        public DateTime Added { get; set; }
        public DateTime Modified { get; set; }
        public string Content { get; set; }
        public int EntryId { get; set; }
    }
}
