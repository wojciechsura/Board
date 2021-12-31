using System;
using System.Collections.Generic;
using System.Text;

namespace Board.BusinessLogic.Infrastructure.Document.Filesystem
{
    public class LocalFilesystem : BaseFilesystem
    {
        private readonly string dataPath;

        public LocalFilesystem(string dataPath)
        {
            this.dataPath = dataPath;
        }
    }
}
