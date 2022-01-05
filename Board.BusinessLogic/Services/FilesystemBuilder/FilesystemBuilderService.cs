using Board.BusinessLogic.Infrastructure.Document.Filesystem;
using Board.Models.Document;
using Board.BusinessLogic.Services.Paths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Board.BusinessLogic.Services.FilesystemBuilder
{
    internal class FilesystemBuilderService : IFilesystemBuilderService
    {
        private readonly IPathService pathService;

        private BaseFilesystem CreateLocalFilesystem(string projectPath, LocalFilesystemDefinition localFilesystem)
        {
            var dataPath = pathService.GetLocalFilesystemPath(projectPath);
            System.IO.Directory.CreateDirectory(dataPath);

            return new LocalFilesystem(dataPath);
        }

        private BaseFilesystem OpenLocalFilesystem(string projectPath, LocalFilesystemDefinition localFilesystem)
        {
            var dataPath = pathService.GetLocalFilesystemPath(projectPath);

            return new LocalFilesystem(dataPath);
        }

        public FilesystemBuilderService(IPathService pathService)
        {
            this.pathService = pathService;
        }

        public BaseFilesystem Create(string projectPath, BaseFilesystemDefinition filesystem)
        {
            switch (filesystem)
            {
                case LocalFilesystemDefinition localFilesystem:
                    {
                        return CreateLocalFilesystem(projectPath, localFilesystem);
                    }
                default:
                    throw new InvalidOperationException("Unsupported filesystem!");
            }
        }

        public BaseFilesystem Open(string projectPath, BaseFilesystemDefinition filesystem)
        {
            switch (filesystem)
            {
                case LocalFilesystemDefinition localFilesystem:
                    {
                        return OpenLocalFilesystem(projectPath, localFilesystem);
                    }
                default:
                    throw new InvalidOperationException("Unsupported filesystem!");
            }
        }
    }
}
