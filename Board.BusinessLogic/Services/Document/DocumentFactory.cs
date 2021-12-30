using Board.BusinessLogic.Infrastructure.Document;
using Board.BusinessLogic.Models.Document;
using Board.BusinessLogic.Services.DatabaseBuilder;
using Board.BusinessLogic.Services.FilesystemBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Board.BusinessLogic.Services.Document
{
    public class DocumentFactory : IDocumentFactory
    {
        private readonly IDatabaseBuilderService databaseBuilder;
        private readonly IFilesystemBuilderService filesystemBuilder;

        private void Validate(DocumentInfo info)
        {
            if (info.Definition == null)
                throw new ArgumentException("Document info lacks document definition!", nameof(info));
            if (info.Definition.Filesystem == null)
                throw new ArgumentException("Document definition lacks filesystem definition!", nameof(info));
            if (info.Definition.Database == null)
                throw new ArgumentException("Document definition lacks database definition!", nameof(info));
            if (!System.IO.File.Exists(info.Path))
                throw new ArgumentException("Document info points to non-existing path!", nameof(info));
        }

        public DocumentFactory(IDatabaseBuilderService databaseBuilder, IFilesystemBuilderService filesystemBuilder)
        {
            this.databaseBuilder = databaseBuilder;
            this.filesystemBuilder = filesystemBuilder;
        }

        public WallDocument Create(DocumentInfo info)
        {
            Validate(info);
            // TODO
            throw new NotImplementedException();
        }

        public WallDocument Open(DocumentInfo info)
        {
            Validate(info);
            // TODO
            throw new NotImplementedException();
        }
    }
}
