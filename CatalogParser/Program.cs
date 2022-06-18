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
            if (args.Length != 2)
            {
                Console.WriteLine("Start application from the console!");
                Console.WriteLine("Please add arguments: the path to the file, as well as the separator character (without extra spaces)");
                Console.WriteLine("Example: 'start CatalogParser.exe test.exe ;'");
                Console.ReadLine();
            }

            var filePath = args[0];
            var sep = args[1];
            var watch = new Stopwatch();
            watch.Start();

            var parsedCatalog = await CatalogParser.ParseCatalogAsync(catalogPath, filePath, sep);
            CsvUtils.WriteProductInfoInFile(filePath, parsedCatalog, sep);

            watch.Stop();
            TimeSpan timeSpan = watch.Elapsed;
            Console.WriteLine("Time: {0}h {1}m {2}s {3}ms", timeSpan.Hours, timeSpan.Minutes,
            timeSpan.Seconds, timeSpan.Milliseconds);
        }
    }
}
