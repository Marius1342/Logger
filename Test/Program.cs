using LoggerSystem;
namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Logger.Init();
            Logger.Log("Test23");
            Logger.Close();
        }
    }
}
