using System;
using System.Collections.Generic;
using System.Text;

namespace Board.BusinessLogic.Models.Document
{
    public class DocumentInfo
    {
        public DocumentInfo(string projectPath, DocumentDefinition definition)
        {
            ProjectPath = projectPath;
            Definition = definition;
        }

        public string ProjectPath { get; }
        public DocumentDefinition Definition { get; }
    }
}
