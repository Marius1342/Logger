using Logger.FileManagement.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerRead
{
    [Serializable]
    public class Entry
    {
        public List<LogXml> LogXml { get; set; }
    }
}
