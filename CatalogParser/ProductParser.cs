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
    internal class ProductParser
    {
        public string ProductUrl { get; }
        public IDocument Document { get; }
        private ProductParser(string productUrl, IDocument document)
        {
            ProductUrl = productUrl;
            Document = document;
        }

        public struct ProductInfo
        {
            public string regionName;
            public List<string> breadCrumb;
            public string productName;
            public string actualPrice;
            public string oldPrice;
            public string availableStatus;
            public List<string> picUrls;
            public string productUrl;
        }

        //Async constructor
        public static async Task<ProductParser> BuildProductParserAsync(string productUrl)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(productUrl);
            return new ProductParser(productUrl, document);
        }

        #region Extract info methods group
        public string ExtractRegionName()
        {
            string regionNameSelector = @"a[data-src=""#region""]";
            var siteRegionName = Document.QuerySelector(regionNameSelector).TextContent;
            //Deleting unnecessary symbols from region name
            siteRegionName = Regex.Replace(siteRegionName, @"\t|\n|\r", "").Trim();
            return siteRegionName;
        }
        public List<string> ExtractBreadCrumb()
        {
            var breadCrumbSelector = @"nav.breadcrumb > :not(:last-child)";
            var breadCrumbs = Document.QuerySelectorAll(breadCrumbSelector).Select(x => x.TextContent.Trim()).ToList();
            return breadCrumbs;
        }
        public string ExtractProductName()
        {
            string ProductNameSelector = "h1.detail-name";
            var productName = Document.QuerySelector(ProductNameSelector).TextContent.Trim();
            return productName;
        }
        public string ExtractActualPrice()
        {
            string ActualPriceSelector = "span.price";
            var actualPrice = Document.QuerySelector(ActualPriceSelector).TextContent;
            return actualPrice;
        }
        public string ExtractOldPrice()
        {
            string OldPriceSelector = "span.old-price";
            var oldPrice = Document.QuerySelector(OldPriceSelector).TextContent.Trim();
            return oldPrice;
        }
        public string ExtractAvailableStatus()
        {
            string AvailableStatusSelector = "span.ok";
            var availableStatus = Document.QuerySelector(AvailableStatusSelector).TextContent.Trim();
            return availableStatus;
        }
        public List<string> ExtractPicUrls()
        {
            string picUrlsSelector = @"div.row.align-content-stretch.my-4 div.col-12.col-md-10.col-lg-7 div.card-slider-for";
            var test = Document.QuerySelector(picUrlsSelector).Children.Select(x => x.Children.First().Children.First()).ToList();
            List<string> pictureUrls = test.Select(x => x.GetType().GetProperty("Source").GetValue(x).ToString()).ToList();

            return pictureUrls;
        }
        #endregion
        public ProductInfo ParseProductPage()
        {
            ProductInfo productInfo = new()
            {
                regionName = ExtractRegionName(),
                breadCrumb = ExtractBreadCrumb(),
                productName = ExtractProductName(),
                actualPrice = ExtractActualPrice(),
                oldPrice = ExtractOldPrice(),
                availableStatus = ExtractAvailableStatus(),
                picUrls = ExtractPicUrls(),
                productUrl = ProductUrl
            };

            return productInfo;
        }
    }
}
