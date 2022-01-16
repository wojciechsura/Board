using System;
using System.Collections.Generic;
using System.IO;
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

        public override void DeleteFile(string relativePath)
        {
            var completePath = System.IO.Path.Combine(dataPath, relativePath);
            if (File.Exists(completePath))
                File.Delete(completePath);
        }

        public override MemoryStream LoadFile(string relativePath)
        {
            var completePath = System.IO.Path.Combine(dataPath, relativePath);
            var ms = new MemoryStream();

            using (var fs = new FileStream(completePath, FileMode.Open, FileAccess.Read))
                fs.CopyTo(ms);

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public override void SaveFile(string relativePath, MemoryStream stream, bool overwriteIfExists = false)
        {
            var completePath = System.IO.Path.Combine(dataPath, relativePath);
            if (File.Exists(completePath) && !overwriteIfExists)
                throw new InvalidOperationException($"Cannot save file {completePath}, because file already exists!");

            string directory = System.IO.Path.GetDirectoryName(completePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var fs = new FileStream(completePath, FileMode.Create, FileAccess.Write))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fs);
            }
        }
    }
}
