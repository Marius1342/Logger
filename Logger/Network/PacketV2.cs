using LoggerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerSystem.NetworkingLogger
{
    public class PacketV2
    {
        public string Message { get; set; }
        public Levels Level { get; set; }
        public string Token { get; set; }
        public Commands Command { get; set; } = Commands.WriteLog;
        public string[] Data { get; set; } = null;
        public enum Commands
        {
            WriteLog,
            GetXmlFiles,
            GetFileContent,
            AllXmlFiles,
            FileContent
        }



    }
}
