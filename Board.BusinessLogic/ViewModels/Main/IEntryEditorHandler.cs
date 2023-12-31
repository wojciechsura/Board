﻿namespace Board.BusinessLogic.ViewModels.Main
{
    public interface IEntryEditorHandler
    {
        void ToggleTag(AvailableTagViewModel tag);
        void SaveCommentRequest(InplaceCommentEditorViewModel inplaceCommentEditorViewModel);
        void CancelCommentRequest(InplaceCommentEditorViewModel inplaceCommentEditorViewModel);
        void EditCommentRequest(CommentViewModel commentViewModel);
        void DeleteCommentRequest(CommentViewModel commentViewModel);
    }
}