using Board.BusinessLogic.Services.Dialogs;
using Board.BusinessLogic.Services.Messaging;
using Board.Services.DialogService;
using Board.Services.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace Board.Dependencies
{
    public static class Configuration
    {
        private static bool configured = false;

        public static void Configure(IUnityContainer container)
        {
            if (configured)
                return;

            container.RegisterType<IMessagingService, MessagingService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());

            Board.BusinessLogic.Dependencies.Configuration.Configure(container);

            configured = true;
        }
    }
}
