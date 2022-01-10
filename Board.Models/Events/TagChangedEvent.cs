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
        public TagChangedEvent(ChangeKind changeKind, int id)
        {
            ChangeKind = changeKind;
            Id = id;
        }

        public ChangeKind ChangeKind { get; }
        public int Id { get; set; }
    }
}
