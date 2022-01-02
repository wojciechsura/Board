﻿using AutoMapper;
using Board.BusinessLogic.Models.Data;
using Board.BusinessLogic.Models.Document;
using Board.BusinessLogic.Services.Dialogs;
using Board.BusinessLogic.Services.Document;
using Board.BusinessLogic.Services.Paths;
using Board.BusinessLogic.ViewModels.Base;
using Board.BusinessLogic.ViewModels.Document;
using Board.Resources;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
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
        private readonly IMapper mapper;

        private readonly BaseCondition documentNotLoadingCondition;
        private readonly BaseCondition documentExistsCondition;
        private readonly BaseCondition tableSelectedCondition;

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

        private void DoNewTable()
        {
            (bool result, TableModel newTable) = dialogService.ShowNewTableDialog();
            if (result)
            {
                activeDocument.AddTableFromModel(newTable);
            }
        }

        private void DoDeleteTable()
        {
            throw new NotImplementedException();
        }

        private void DoEditTable()
        {
            var modelClone = mapper.Map<TableModel>(activeDocument.ActiveTable.Table);

            var result = dialogService.ShowEditTableDialog(modelClone);
            if (result)
            {
                activeDocument.UpdateTableFromModel(activeDocument.ActiveTable, modelClone);
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

        public MainWindowViewModel(IMainWindowAccess access, IDialogService dialogService, IDocumentFactory documentFactory, IPathService pathService, IMapper mapper)
        {
            this.access = access;
            this.dialogService = dialogService;
            this.documentFactory = documentFactory;

            documentExistsCondition = new LambdaCondition<MainWindowViewModel>(this, vm => vm.ActiveDocument != null, false);
            documentNotLoadingCondition = new LambdaCondition<MainWindowViewModel>(this, vm => !vm.ActiveDocument.IsLoading, true);
            tableSelectedCondition = new LambdaCondition<MainWindowViewModel>(this, vm => vm.ActiveDocument.ActiveTable != null, false);

            NewCommand = new AppCommand(obj => DoNew());
            OpenCommand = new AppCommand(obj => DoOpen());
            NewTableCommand = new AppCommand(obj => DoNewTable(), documentExistsCondition & documentNotLoadingCondition);
            EditTableCommand = new AppCommand(obj => DoEditTable(), documentExistsCondition & tableSelectedCondition);
            DeleteTableCommand = new AppCommand(obj => DoDeleteTable(), documentExistsCondition & tableSelectedCondition);

            this.pathService = pathService;
            this.mapper = mapper;
        }

        // Public properties --------------------------------------------------

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand NewTableCommand { get; }
        public ICommand EditTableCommand { get; }
        public ICommand DeleteTableCommand { get; }

        public DocumentViewModel ActiveDocument
        {
            get => activeDocument;
            set => Set(ref activeDocument, value);
        }
    }
}
