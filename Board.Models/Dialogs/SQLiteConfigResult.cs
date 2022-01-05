using System;
using System.Collections.Generic;
using System.Text;

namespace Board.Models.Dialogs
{
    public class SQLiteConfigResult
    {
        public SQLiteConfigResult(string wallName, string path)
        {
            WallName = wallName;
            Path = path;
        }

        public string WallName { get; }
        public string Path { get; }
    }
}
