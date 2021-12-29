using Board.BusinessLogic.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.Services.EventBus
{
    public interface IEventListener<T> where T : BaseEvent
    {
        void Receive(T @event);
    }
}
