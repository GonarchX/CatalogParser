using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static CatalogParser.ProductParser;

namespace CatalogParser
{
    public class CatalogParser
    {
        const string domainName = "https://www.toy.ru";
        const string productUrlsSelector = @"div[class*=""h-100 product-card""] a.d-block.img-link.text-center.gtm-click";
        public static async Task ParseCatalogInCsvAsync(string sitePath, string csvFilePath, string sep)
        {
            List<string> productUrlsOnFirstPage = await ExtractAllProductUrlsOnPageAsync(sitePath, 1);
            if (productUrlsOnFirstPage == null) throw new Exception($"Unavailable to extract products from first page in path: {domainName}{sitePath}!");

            List<string> productUrlsOnCurrentPage = null;
            int curCatalogPageNum = 2;

            var tasks = new List<Task<ProductInfo>>();
            //This loop stops when the current and first pages are equal, because the site returns the first page in the catalog when trying to get a non-existent page
            while (!IsEqualsCatalogPages(productUrlsOnFirstPage, productUrlsOnCurrentPage))
            {
                var syncCurCatalogPage = curCatalogPageNum;
                productUrlsOnCurrentPage = curCatalogPageNum == 2 ? productUrlsOnFirstPage : productUrlsOnCurrentPage;

                for (int i = 0; i < productUrlsOnCurrentPage.Count; i++)
                {
                    int syncCurProductPage = i;

                    tasks.Add(Task.Run(async () =>
                    {
                        var prodParser = await BuildProductParserAsync(productUrlsOnCurrentPage[syncCurProductPage]);
                        return prodParser.ParseProductPage();

                        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: I'm parse product page with number {syncCurProductPage} on catalog page with number {syncCurCatalogPage - 1}");                       
                    }));
                }

                try
                {
                    productUrlsOnCurrentPage = await ExtractAllProductUrlsOnPageAsync(sitePath, curCatalogPageNum);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Can't parse catalog page in path: {sitePath}{curCatalogPageNum}!\nReason: {ex}");
                }

                curCatalogPageNum++;
            }
            var result = await Task.WhenAll(tasks);
            CsvUtils.WriteProductInfoInFile(csvFilePath, result.ToList(), sep);
        }

        //Getting all product urls on the catalog page
        private static async Task<List<string>> ExtractAllProductUrlsOnPageAsync(string sitePath, int catalogPageNumber = 1)
        {
            var config = Configuration.Default.WithDefaultLoader();
            string urlParams = $"?filterseccode%5B0%5D=transport&PAGEN_5={catalogPageNumber}";
            string curCatalogPageUrl = $"{domainName}/{sitePath}/{urlParams}";
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(curCatalogPageUrl);
            return document.QuerySelectorAll(productUrlsSelector).Select(x => domainName + x.GetAttribute("href")).ToList();
        }

        private static bool IsEqualsCatalogPages(List<string> firstCatalogPageUrl, List<string> secondCatalogPageUrl)
        {
            if (firstCatalogPageUrl == null || secondCatalogPageUrl == null)
                return false;
            if (firstCatalogPageUrl.Equals(secondCatalogPageUrl))
                return true;
            else if (firstCatalogPageUrl.Count != secondCatalogPageUrl.Count)
                return false;

            return firstCatalogPageUrl.SequenceEqual(secondCatalogPageUrl);
        }
    }
}
