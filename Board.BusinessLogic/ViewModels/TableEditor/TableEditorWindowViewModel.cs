using Board.Models.Data;
using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.ViewModels.Base;
using Board.Resources;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using Board.BusinessLogic.Services.Dialogs;
using System.Drawing;
using Board.BusinessLogic.Services.Messaging;

namespace Board.BusinessLogic.ViewModels.TableEditor
{
    public class TableEditorWindowViewModel : BaseViewModel
    {
        private readonly ITableEditorWindowAccess access;
        private readonly TableEditModel table;
        private readonly IDialogService dialogService;
        private readonly IMessagingService messagingService;
        
        private string name;
        private MemoryStream background;

        private bool backgroundChanged;

        private void DoOk()
        {
            UpdateToModel(table);

            table.BackgroundChanged = backgroundChanged;
            if (backgroundChanged)
                table.BackgroundStream = background;

            access.Close(true);
        }

        private void DoCancel()
        {
            access.Close(false);
        }

        private void DoOpenBackground()
        {
            (bool result, string path) = dialogService.ShowOpenDialog(Strings.ImageFilter, Strings.Dialog_OpenTableBackground_Title);

            if (result)
            {
                // Sanity check
                try
                {
                    Bitmap bitmap = new Bitmap(path);
                }
                catch
                {
                    messagingService.ShowError(Strings.Message_InvalidImageFile);
                    return;
                }

                MemoryStream ms = new MemoryStream();
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    fs.CopyTo(ms);

                backgroundChanged = true;
                Background = ms;
            }
        }

        public TableEditorWindowViewModel(ITableEditorWindowAccess access, TableEditModel table, bool isNew, IDialogService dialogService, IMessagingService messagingService)
        {
            this.dialogService = dialogService;
            this.messagingService = messagingService;
            this.access = access;
            this.table = table ?? throw new ArgumentNullException(nameof(table));

            backgroundChanged = false;

            if (isNew)
                Title = Strings.TableEditor_Title_New;
            else
                Title = Strings.TableEditor_Title_Edit;

            OkCommand = new AppCommand(obj => DoOk());
            CancelCommand = new AppCommand(obj => DoCancel());
            OpenBackgroundCommand = new AppCommand(obj => DoOpenBackground());

            UpdateFromModel(table);
        }

        [SyncWithModel(nameof(TableEditModel.Name))]
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        [SyncWithModel(nameof(TableEditModel.BackgroundStream))]
        public MemoryStream Background
        {
            get => background;
            set => Set(ref background, value);
        }

        public ICommand OkCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand OpenBackgroundCommand { get; }

        public string Title { get; }

        public TableEditModel Result => table;
    }
}
