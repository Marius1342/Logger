using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Logger.FileManagement.Xml
{
    public static class Root
    {
        private static List<LogXml> LogXmls = new List<LogXml>();
        private readonly static object Locker = new object();
        public static void Add(LogXml logXml)
        {
            lock (Locker)
            {

                LogXmls.Add(logXml);
            }
        }
        public static int Count { get { return LogXmls.Count; } }
        public static LogXml[] GetLogXml()
        {
            LogXml[] arr;
            lock (Locker)
            {
                arr = LogXmls.ToArray();

                LogXmls.Clear();
            }
            return arr;
        }
    }
}
