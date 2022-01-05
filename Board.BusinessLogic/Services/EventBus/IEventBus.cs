using Board.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.Services.EventBus
{
    public interface IEventBus
    {
        void Register<T>(IEventListener<T> listener) where T : BaseEvent;
        void Unregister<T>(IEventListener<T> listener) where T : BaseEvent;
        void Send<T>(T @event) where T : BaseEvent;
    }
}
