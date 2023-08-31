# Logger
Simple logger for .NetFramework >=4.8, writes all logs to a files.
Async logging is active

## How to use ?
- Import your Project and build it or use the newest [release](https://github.com/Marius1342/Logger/releases)
- Call first the init function of the logger
- The logs are wriiten when 3 logs are in the buffer or Logger.Close(); is called
## Example code
```cs
using LoggerSystem;
namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Call the init
            Logger.Init();

            //Write a log
            Logger.Log("Test23");

            //Close the stream and write all data to the file
            Logger.Close();
        }
    }
}

```