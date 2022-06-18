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
            if (!isCorrectInput(args))
            {
                Console.ReadLine(); 
                return;
            }

            var filePath = args[0];            
            var sep = args[1];
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                var parsedCatalog = await CatalogParser.ParseCatalogAsync(catalogPath, filePath, sep);
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
            Console.ReadLine();
        }

        static private bool isCorrectInput(string[] args) {
            if (args.Length != 2)
            {
                Console.WriteLine("Start application from the console!");
                Console.WriteLine("Please add arguments: the path to the file, as well as the separator character");
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
