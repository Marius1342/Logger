using System;

namespace LoggerSystem.ConsoleSystem
{
    public class ConsoleHelper
    {
        private static object consoleLock = new object();
        /// <summary>
        /// Write the log to the console
        /// </summary>
        /// <param name="message">The text must be preprocessed</param>
        /// <param name="level">The log level</param>
        public static void WriteToConsole(string message, Levels level)
        {
            lock (consoleLock)
            {
                SetConsoleColor(level);
                System.Console.WriteLine(message);
                System.Console.ResetColor();
            }
        }
        public static void SetConsoleColor(Levels levels)
        {
            switch (levels)
            {
                case Levels.None:
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case Levels.Log:
                    System.Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case Levels.Warning:
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case Levels.Error:
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    break;

            }

        }

    }
}
