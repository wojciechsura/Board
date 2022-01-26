using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Board.Models.Config
{
    [XmlRoot("Configuration")]
    public class Configuration
    {
        [XmlElement("UI")]
        public UI UI { get; set; } = new UI();
    }
}
