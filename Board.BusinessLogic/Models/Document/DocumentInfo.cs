using System;
using System.Collections.Generic;
using System.Text;

namespace Board.BusinessLogic.Models.Document
{
    public class DocumentInfo
    {
        public DocumentInfo(string path, DocumentDefinition definition)
        {
            Path = path;
            Definition = definition;
        }

        public string Path { get; }
        public DocumentDefinition Definition { get; }
    }
}
