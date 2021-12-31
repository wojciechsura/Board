namespace Board.BusinessLogic.Services.Paths
{
    public interface IPathService
    {
        string AppDataPath { get; }
        string ProjectDefinitionFilename { get; }

        string GetSQLiteDatabasePath(string projectPath);
        string GetLocalFilesystemPath(string projectPath);
        string GetProjectDefinitionPath(string projectPath);
    }
}