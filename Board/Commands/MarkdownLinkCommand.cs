using Board.BusinessLogic.Services.Dialogs;
using Board.BusinessLogic.Services.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace Board.Commands
{
    public class MarkdownLinkCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var href = (string)parameter;

            try
            {
                Process.Start(new ProcessStartInfo(href)
                {
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch
            {
                var messagingService = Dependencies.Container.Instance.Resolve<IMessagingService>();
                messagingService.ShowError(String.Format(Resources.Strings.Message_CannotOpenLink, href));
            }
        }
    }
}
