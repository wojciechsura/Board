using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Board.BusinessLogic.Infrastructure.Document.Filesystem
{
    public abstract class BaseFilesystem
    {
        public abstract void DeleteFile(string relativePath);
        public abstract void SaveFile(string relativePath, MemoryStream stream, bool overwriteIfExists = false);
        public abstract MemoryStream LoadFile(string background);
    }
}
