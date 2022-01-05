using System;
using System.Collections.Generic;
using System.Text;

namespace Board.Models.Document
{
    [Serializable]
    public class SQLiteDatabaseDefinition : BaseDatabaseDefinition
    {
        public string? DatabasePath { get; set; }
    }
}
