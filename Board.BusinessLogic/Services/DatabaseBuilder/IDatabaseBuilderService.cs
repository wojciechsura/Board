using Board.BusinessLogic.Infrastructure.Document.Database;
using Board.BusinessLogic.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Board.BusinessLogic.Services.DatabaseBuilder
{
    public interface IDatabaseBuilderService
    {
        BaseDatabase Build(string path, BaseDatabaseDefinition databaseDefinition);
    }
}
