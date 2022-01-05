using Board.BusinessLogic.Infrastructure.Document;
using Board.BusinessLogic.Infrastructure.Document.Database;
using Board.Models.Document;
using Board.BusinessLogic.Services.DatabaseBuilder;
using Board.BusinessLogic.Services.FilesystemBuilder;
using Board.BusinessLogic.Services.Paths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Board.BusinessLogic.Services.Document
{
    public class DocumentFactory : IDocumentFactory
    {
        private readonly IDatabaseBuilderService databaseBuilder;
        private readonly IFilesystemBuilderService filesystemBuilder;
        private readonly IPathService pathService;

        private void Validate(DocumentInfo info)
        {
            if (info.Definition == null)
                throw new ArgumentException("Document info lacks document definition!", nameof(info));
            if (info.Definition.Filesystem == null)
                throw new ArgumentException("Document definition lacks filesystem definition!", nameof(info));
            if (info.Definition.Database == null)
                throw new ArgumentException("Document definition lacks database definition!", nameof(info));
        }

        public DocumentFactory(IDatabaseBuilderService databaseBuilder, IFilesystemBuilderService filesystemBuilder, IPathService pathService)
        {
            this.databaseBuilder = databaseBuilder;
            this.filesystemBuilder = filesystemBuilder;
            this.pathService = pathService;
        }

        public WallDocument Create(DocumentInfo info)
        {
            Validate(info);

            var database = databaseBuilder.Create(info.ProjectPath, info.Definition.Database);
            var filesystem = filesystemBuilder.Create(info.ProjectPath, info.Definition.Filesystem);

            return new WallDocument(info.ProjectPath, database, filesystem);
        }

        public WallDocument Open(DocumentInfo info)
        {
            Validate(info);

            var database = databaseBuilder.Open(info.ProjectPath, info.Definition.Database);
            var filesystem = filesystemBuilder.Open(info.ProjectPath, info.Definition.Filesystem);

            return new WallDocument(info.ProjectPath, database, filesystem);
        }

        public void SaveDefinition(DocumentInfo info)
        {
            // Ensure, that project directory exists
            System.IO.Directory.CreateDirectory(info.ProjectPath);

            // Store project definition
            var definitionPath = pathService.GetProjectDefinitionPath(info.ProjectPath);

            XmlSerializer serializer = new XmlSerializer(typeof(DocumentDefinition));
            using (var fs = new FileStream(definitionPath, FileMode.Create, FileAccess.Write))
                serializer.Serialize(fs, info.Definition);
        }

        public DocumentInfo OpenDefinition(string definitionPath)
        {
            var projectPath = System.IO.Path.GetDirectoryName(definitionPath);

            XmlSerializer serializer = new XmlSerializer(typeof(DocumentDefinition));
            DocumentDefinition definition;
            using (var fs = new FileStream(definitionPath, FileMode.Open, FileAccess.Read))
                definition = (DocumentDefinition)serializer.Deserialize(fs);

            return new DocumentInfo(projectPath, definition);
        }
    }
}
