using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Board.Models.Document
{
    [Serializable]
    [XmlInclude(typeof(LocalFilesystemDefinition))]
    public class BaseFilesystemDefinition
    {
    }
}
