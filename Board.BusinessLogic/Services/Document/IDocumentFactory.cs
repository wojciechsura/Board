using Board.BusinessLogic.Infrastructure.Document;
using Board.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Board.BusinessLogic.Services.Document
{
    public interface IDocumentFactory
    {
        void SaveDefinition(DocumentInfo info);
        DocumentInfo OpenDefinition(string definitionPath);
        WallDocument Create(DocumentInfo info);
        WallDocument Open(DocumentInfo info);
    }
}
