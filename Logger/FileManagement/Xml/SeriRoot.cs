﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Logger.FileManagement.Xml
{
    public class SeriRoot
    {
        [XmlElement]
        public LogXml[] LogXmls { get; set; }
    }
}
