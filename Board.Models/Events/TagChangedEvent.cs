using Board.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Models.Events
{
    public class TagChangedEvent : BaseEvent
    {
        public TagChangedEvent(ChangeKind changeKind, int tableId, int tagId)
        {
            ChangeKind = changeKind;
            TableId = tableId;
            TagId = tagId;
        }

        public ChangeKind ChangeKind { get; }
        public int TableId { get; }
        public int TagId { get; set; }
    }
}
