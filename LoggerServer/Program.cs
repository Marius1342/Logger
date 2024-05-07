﻿using LoggerSystem;


namespace LoggerServer
{
    internal class Program
    {
        public static int Port = 8080;

        static void Main(string[] args)
        {

            Logger.Init();



            if(args.Length > 0)
            {
                if (args[0] == "add-client")
                {


                    return; 
                }

                try
                {
                    Port = int.Parse(args[0]);
                }catch
                {
                    Logger.Error($"System: Error with port to phrase");
                }
            }


            


        }
    }
}