using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CatalogParser
{
    internal class Program
    {
        const string catalogPath = "catalog/boy_transport";
        static async Task Main(string[] args)
        {
            if (!IsCorrectInput(args))
            {
                Console.WriteLine("Press ENTER to exit from the application");
                Console.ReadLine();
                return;
            }

            var filePath = args[0];
            var sep = args[1];
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                var parsedCatalog = await CatalogParser.ParseCatalogAsync(catalogPath);
                CsvUtils.WriteProductInfoInFile(filePath, parsedCatalog, sep);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            watch.Stop();
            TimeSpan timeSpan = watch.Elapsed;
            Console.WriteLine("Time: {0}h {1}m {2}s {3}ms", timeSpan.Hours, timeSpan.Minutes,
            timeSpan.Seconds, timeSpan.Milliseconds);
            Console.WriteLine("Press ENTER to exit from the application");
            Console.ReadLine();
        }

        static private bool IsCorrectInput(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Instruction: ");
                Console.WriteLine("Make sure that you are starting application from the console");
                Console.WriteLine("Make sure that you don't forget to add the arguments: the path to the file and the delimiter string");
                Console.WriteLine("Example: 'start CatalogParser.exe test.exe ;'");
                return false;
            }
            if (string.IsNullOrEmpty(args[0]))
            {
                Console.WriteLine("File path shouldn't be empty or null!");
                return false;
            }
            if (string.IsNullOrEmpty(args[1]))
            {
                Console.WriteLine("Sep shouldn't be empty or null!");
                return false;
            }
            return true;
        }
    }
}
