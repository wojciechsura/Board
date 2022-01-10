namespace Board.BusinessLogic.ViewModels.Main
{
    public interface IEntryEditorHandler
    {
        void AddTag(AvailableTagViewModel tag);
        void RemoveTag(AddedTagViewModel tag);
    }
}