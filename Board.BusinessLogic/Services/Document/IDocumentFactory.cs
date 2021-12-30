using Board.BusinessLogic.Infrastructure.Document;
using Board.BusinessLogic.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Board.BusinessLogic.Services.Document
{
    public interface IDocumentFactory
    {
        WallDocument Create(DocumentInfo info);
        WallDocument Open(DocumentInfo info);
    }
}
