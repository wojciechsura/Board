﻿using Board.BusinessLogic.Models.Document;
using Board.BusinessLogic.Services.Dialogs;
using Board.BusinessLogic.Services.Document;
using Board.BusinessLogic.Services.Paths;
using Board.BusinessLogic.ViewModels.Base;
using Board.BusinessLogic.ViewModels.Main.Document;
using Board.Resources;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class MainWindowViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly IMainWindowAccess access;
        private readonly IDialogService dialogService;
        private readonly IDocumentFactory documentFactory;
        private readonly IPathService pathService;

        private DocumentViewModel activeDocument;

        // Private methods ----------------------------------------------------

        private void DoNew()
        {
            if (activeDocument != null && !activeDocument.CanClose())
                return;

            (bool result, DocumentInfo info) = dialogService.ShowNewWallDialog();
            
            if (result)
            {
                SetNewDocument(info);
            }
        }

        private void DoOpen()
        {
            if (activeDocument != null && !activeDocument.CanClose())
                return;

            string projectDefFilename = pathService.ProjectDefinitionFilename;
            (bool result, string path) = dialogService.ShowOpenDialog($"{Strings.Dialog_OpenWall_ProjectFilterDesc}|{projectDefFilename}",
                Strings.Dialog_OpenWall_Title);

            if (result)
            {
                DocumentInfo info = documentFactory.OpenDefinition(path);
                SetNewDocument(info);
            }
        }

        private void SetNewDocument(DocumentInfo info)
        {
            // Close active document
            if (activeDocument != null)
            {
                activeDocument.Close();
                ActiveDocument = null;
            }

            var documentFactory = Board.Dependencies.Container.Instance.Resolve<IDocumentFactory>();

            var newDocument = new DocumentViewModel(documentFactory, info);
            ActiveDocument = newDocument;
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainWindowAccess access, IDialogService dialogService, IDocumentFactory documentFactory, IPathService pathService)
        {
            this.access = access;
            this.dialogService = dialogService;
            this.documentFactory = documentFactory;

            NewCommand = new AppCommand(obj => DoNew());
            OpenCommand = new AppCommand(obj => DoOpen());

            this.pathService = pathService;
        }

        // Public properties --------------------------------------------------

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }

        public DocumentViewModel ActiveDocument
        {
            get => activeDocument;
            set => Set(ref activeDocument, value);
        }
    }
}
