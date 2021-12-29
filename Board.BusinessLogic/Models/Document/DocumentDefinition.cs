using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Board.BusinessLogic.Models.Document
{
    [XmlRoot("Document")]
    public class DocumentDefinition
    {
        [XmlElement("Database")]
        public BaseDatabaseDefinition Database { get; set; }

        [XmlElement("Attachments")]
        public BaseAttachmentsDefinition Attachments { get; set; }
    }
}
