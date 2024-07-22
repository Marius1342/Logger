using LoggerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Logger.FileManagement.Xml
{
    [Serializable]
    [XmlRoot(ElementName = "row")]
    public class LogXml
    {
        [XmlAttribute("dateTime")]
        public  DateTime dateTime { get; set; }
        [XmlAttribute("SystemName")]
        public string SystemName { get; set; } = "";
        [XmlAttribute("ApiToken")]
        public string ApiToken { get; set; } = "";
        [XmlAttribute("Levels")]
        public  Levels Levels { get; set; }
        [XmlAttribute("Message")]
        public  string Message { get; set; } = "";
    }
}
