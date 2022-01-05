using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Board.Models.Document
{
    [XmlRoot("Document")]
    public class DocumentDefinition
    {
        [XmlElement("Database")]
        public BaseDatabaseDefinition? Database { get; set; }

        [XmlElement("Filesystem")]
        public BaseFilesystemDefinition? Filesystem { get; set; }
    }
}
