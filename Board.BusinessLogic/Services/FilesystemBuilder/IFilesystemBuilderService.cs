using Board.BusinessLogic.Infrastructure.Document.Filesystem;
using Board.BusinessLogic.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Board.BusinessLogic.Services.FilesystemBuilder
{
    public interface IFilesystemBuilderService
    {
        BaseFilesystem Create(string projectPath, BaseFilesystemDefinition filesystem);
        BaseFilesystem Open(string projectPath, BaseFilesystemDefinition filesystem);
    }
}
