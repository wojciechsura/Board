using Board.BusinessLogic.Infrastructure.Document.Database;
using Board.BusinessLogic.Infrastructure.Document.Filesystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Board.BusinessLogic.Infrastructure.Document
{
    public class WallDocument
    {
        public WallDocument(string path, BaseDatabase database, BaseFilesystem filesystem)
        {
            Path = path;
            Database = database;
            Filesystem = filesystem;
        }

        public string Path { get; }
        public BaseDatabase Database { get; }
        public BaseFilesystem Filesystem { get; }
    }
}
