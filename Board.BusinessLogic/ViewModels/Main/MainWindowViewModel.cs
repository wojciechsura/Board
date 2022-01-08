﻿using AutoMapper;
using Board.Models.Data;
using Board.Models.Document;
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
    public class MainWindowViewModel : BaseViewModel, IDocumentHandler
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
        private EntryEditorViewModel entryEditor;

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
            var tableViewModel = activeDocument.ActiveTable;
            var message = String.Format(Strings.Message_TableDeletion, tableViewModel.Name);

            (bool result, bool? permanent) = dialogService.ShowDeleteDialog(message);

            if (result)
            {
                activeDocument.DeleteTable(tableViewModel, permanent.Value);
            }
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

            var newDocument = new DocumentViewModel(mapper, documentFactory, info, this);
            ActiveDocument = newDocument;
        }

        // IDocumentHandler implementation ------------------------------------

        void IDocumentHandler.NewColumnRequest(TableViewModel tableViewModel)
        {
            (bool result, ColumnModel newColumn) = dialogService.ShowNewColumnDialog();
            if (result)
            {
                activeDocument.AddColumnFromModel(tableViewModel, newColumn);
            }
        }

        void IDocumentHandler.EditColumnRequest(ColumnViewModel columnViewModel)
        {
            var modelClone = mapper.Map<ColumnModel>(columnViewModel.Column);

            var result = dialogService.ShowEditColumnDialog(modelClone);
            if (result)
            {
                activeDocument.UpdateColumnFromModel(columnViewModel, modelClone);
            }
        }

        void IDocumentHandler.DeleteColumnRequest(ColumnViewModel columnViewModel)
        {
            var message = String.Format(Strings.Message_ColumnDeletion, columnViewModel.Name);

            (bool result, bool? permanent) = dialogService.ShowDeleteDialog(message);

            if (result)
            {
                activeDocument.DeleteColumn(columnViewModel, permanent.Value);
            }
        }

        void IDocumentHandler.SaveNewInplaceEntryRequest(NewInplaceEntryViewModel newInplaceEntryViewModel)
        {
            activeDocument.AddEntryFromInplaceNew(newInplaceEntryViewModel);
        }

        void IDocumentHandler.CancelNewInplaceEntryRequest(NewInplaceEntryViewModel newInplaceEntryViewModel)
        {
            activeDocument.RemoveInplaceNewEntry(newInplaceEntryViewModel);
        }

        void IDocumentHandler.NewInplaceEntryRequest(ColumnViewModel columnViewModel)
        {
            activeDocument.AddNewInplaceEntry(columnViewModel);            
        }

        void IDocumentHandler.DeleteEntryRequest(EntryViewModel entryViewModel)
        {
            var shortenedTitle = entryViewModel.Title.Length > 32 ? $"{entryViewModel.Title.Substring(0, 32)}..." : entryViewModel.Title;

            var message = String.Format(Strings.Message_EntryDeletion, shortenedTitle);

            (bool result, bool? permanent) = dialogService.ShowDeleteDialog(message);
            
            if (result)
            {
                activeDocument.DeleteEntry(entryViewModel, permanent.Value);
            }
        }

        void IDocumentHandler.EditEntryRequest(EntryViewModel entryViewModel)
        {            
            EntryEditor = new EntryEditorViewModel(entryViewModel.Entry.Id, entryViewModel, activeDocument.Document, this);
        }

        void IDocumentHandler.RequestEditorClose(EntryViewModel entryToUpdate)
        {
            activeDocument.UpdateEntry(entryToUpdate);
            EntryEditor = null;
        }

        void IDocumentHandler.RequestMoveEntry(EntryViewModel entryViewModel, ColumnViewModel targetColumnViewModel, int newIndex)
        {
            activeDocument.MoveEntry(entryViewModel, targetColumnViewModel, newIndex);
        }

        void IDocumentHandler.RequestMoveColumn(ColumnViewModel columnViewModel, TableViewModel tableViewModel, int newIndex)
        {
            activeDocument.MoveColumn(columnViewModel, tableViewModel, newIndex);
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

        public EntryEditorViewModel EntryEditor
        {
            get => entryEditor;
            set => Set(ref entryEditor, value);
        }
    }
}
