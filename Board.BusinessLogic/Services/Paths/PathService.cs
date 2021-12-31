using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.Services.Paths
{
    internal class PathService : IPathService
    {
        private const string PUBLISHER = "Publisher";
        private const string APPNAME = "Application";
        private const string SQLITE_DB_NAME = "wall.db";
        private const string PROJECT_DEFINITION_NAME = "wall.project";
        private const string LOCAL_DATA_NAME = "Data";

        private readonly string appDataPath;

        public PathService()
        {
            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PUBLISHER, APPNAME);
            Directory.CreateDirectory(appDataPath);
        }

        public string AppDataPath => appDataPath;

        public string ProjectDefinitionFilename => PROJECT_DEFINITION_NAME;

        public string GetLocalFilesystemPath(string projectPath)
        {
            return System.IO.Path.Combine(projectPath, LOCAL_DATA_NAME);
        }

        public string GetProjectDefinitionPath(string projectPath)
        {
            return System.IO.Path.Combine(projectPath, PROJECT_DEFINITION_NAME);
        }

        public string GetSQLiteDatabasePath(string projectPath)
        {
            return System.IO.Path.Combine(projectPath, SQLITE_DB_NAME);
        }        
    }
}
