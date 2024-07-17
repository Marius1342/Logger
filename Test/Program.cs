using LoggerSystem;
using System;
using System.Threading;
namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {


            LoggerSystem.Logger.minLogLevel = Levels.None;
            LoggerSystem.Logger.Init("123", "127.0.0.1", 27);
            while (true)
            {

                LoggerSystem.Logger.Log("Test23");
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            LoggerSystem.Logger.Close();
        }
    }
}
