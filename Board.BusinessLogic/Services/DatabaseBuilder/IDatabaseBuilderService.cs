using Board.BusinessLogic.Infrastructure.Document.Database;
using Board.BusinessLogic.Infrastructure.Document.Filesystem;
using Board.BusinessLogic.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Board.BusinessLogic.Services.DatabaseBuilder
{
    public interface IDatabaseBuilderService
    {
        BaseDatabase Create(string projectPath, BaseDatabaseDefinition database);
        BaseDatabase Open(string projectPath, BaseDatabaseDefinition database);
    }
}
