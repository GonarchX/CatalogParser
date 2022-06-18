using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CatalogParser
{
    public class ProductParser
    {
        public string ProductUrl { get; }
        public IDocument Document { get; }
        public ProductParser(string productUrl, IDocument document)
        {
            ProductUrl = productUrl;
            Document = document;
        }

        public class ProductInfo
        {
            public string regionName;
            public List<string> breadCrumb;
            public string productName;
            public string actualPrice;
            public string oldPrice;
            public string availableStatus;
            public List<string> picUrls;
            public string productUrl;

            public string ToString(string sep)
            {
                string listSep = " ";
                List<string> result = new();
                result.Add(regionName + sep);
                result.AddRange(breadCrumb.Select(x => x + listSep));
                result.Add(productName + sep);
                result.Add(actualPrice + sep);
                result.Add(oldPrice + sep);
                result.Add(availableStatus + sep);
                result.AddRange(picUrls.Select(x => x + listSep));
                result.Add(productUrl + sep);

                return String.Join("", result);
            }
        }

        //Async constructor
        public static async Task<ProductParser> BuildProductParserAsync(string productUrl)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(productUrl);
            return new ProductParser(productUrl, document);
        }

        readonly Dictionary<string, string> selectors = new()
        {
            {"regionName", @"a[data-src=""#region""]"},
            {"breadCrumb", @"nav.breadcrumb > :not(:last-child)"},
            {"productName", "h1.detail-name"},
            {"actualPrice", "span.price"},
            {"oldPrice", "span.old-price"},
            {"availableStatus", "span.ok"},
            {"picUrls", @"div.row.align-content-stretch.my-4 div.col-12.ol-md-10.col-lg-7 div.card-slider-for"}
        };
        public string ExtractDataBySelector(string selector)
        {
            var dataFromSite = GetContentIfExist(selector).First();
            //Deleting unnecessary symbols from region name
            dataFromSite = Regex.Replace(dataFromSite, @"\t|\n|\r", "").Trim();
            return dataFromSite;
        }
        public List<string> ExtractAllDataBySelector(string selector)
        {            
            var dataFromSite = GetContentIfExist(selector).Select(x => Regex.Replace(x, @"\t|\n|\r", "").Trim()).ToList();
            return dataFromSite;
        }

        //This method looks ugly because angle sharp cannot find the URLs of product images using the css selector.
        public List<string> ExtractPicUrls()
        {
            string picUrlsSelector = @"div.row.align-content-stretch.my-4 div.col-12.col-md-10.col-lg-7 div.card-slider-for";
            var listOfImgUrls = Document.QuerySelector(picUrlsSelector).Children.Select(x => x.Children.First().Children.First()).ToList();
            List<string> pictureUrls = listOfImgUrls.Select(x => x.GetType().GetProperty("Source").GetValue(x).ToString()).ToList();

            return pictureUrls;
        }

        private List<string> GetContentIfExist(string selector)
        {
            var content = Document.QuerySelector(selector);
            if (content != null)
            {
                return Document.QuerySelectorAll(selector).Select(x => x.TextContent).ToList();
            }
            else return new List<string>() { "" };
        }

        public ProductInfo ParseProductPage()
        {
            ProductInfo productInfo = new()
            {
                regionName = ExtractDataBySelector(selectors["regionName"]),
                breadCrumb = ExtractAllDataBySelector(selectors["breadCrumb"]),
                productName = ExtractDataBySelector(selectors["productName"]),
                actualPrice = ExtractDataBySelector(selectors["actualPrice"]),
                oldPrice = ExtractDataBySelector(selectors["oldPrice"]),
                availableStatus = ExtractDataBySelector(selectors["availableStatus"]),
                picUrls = ExtractPicUrls(),
                productUrl = ProductUrl
            };

            return productInfo;
        }
    }
}
