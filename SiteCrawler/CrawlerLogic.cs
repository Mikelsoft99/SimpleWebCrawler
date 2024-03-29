﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;

namespace SiteCrawler
{
    class CrawlerLogic
    {
        public string BaseDomain;
        public bool ExtractLinks { get; set; }
        public bool IncludeMetaData { get; set; }
        public string ClassSelector { get; set; }

        protected List<string> PagesToCrawl = new List<string>();
        public List<string> PagesCraweled = new List<string>();
        public Dictionary<string, List<string>> PagesCrawledLinks = new Dictionary<string, List<string>>();
        public List<Models.MetaData> MetaData = new List<Models.MetaData>();
        protected List<string> PagesFailed = new List<string>();


        protected PoliteWebCrawler Crawler;
        protected FileWriter FileWriter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BaseUrl">a valid domain like http://www.ahrntal.com</param>
        public CrawlerLogic(string BaseUrl)
        {
            FileWriter = new FileWriter();


            AddToPagesToCrawl(BaseUrl);
            SetBaseDomain(PagesToCrawl.First());

        }

        /// <summary>
        /// define the option for extracing the links too
        /// </summary>
        /// <param name="s"></param>
        public void SetOptionExtractLink(string s)
        {
            ExtractLinks = s == "y" ? true : false;
        }

        /// <summary>
        /// Set the class selector for extracting links only form that element
        /// example .className
        /// </summary>
        /// <param name="c"></param>
        public void SetOptionClassSelector(string c)
        {
            ClassSelector = c;
        }

        /// <summary>
        /// define the option for including the meta data
        /// </summary>
        /// <param name="s"></param>
        public void SetOptionIncludeMetaData(string s)
        {
            IncludeMetaData = s == "y" ? true : false;
        }


        private void SetBaseDomain(string url)
        {
            Uri urlClass = new Uri(url);
            this.BaseDomain = urlClass.Host;
        }

        private CrawlConfiguration CreateConfig()
        {
            return new CrawlConfiguration()
            {
                CrawlTimeoutSeconds = 100,
                MaxConcurrentThreads = 10,
                MaxPagesToCrawl = 1000000,
                MaxPagesToCrawlPerDomain = 100000,
                IsSslCertificateValidationEnabled = false,
                MaxCrawlDepth = 1000,
                MaxLinksPerPage = 1000,
                DownloadableContentTypes = "text/html",
            };
        }

        private void AddToCrawledPages(string url)
        {
            if (!this.PagesCraweled.Contains(url))
            {
                this.PagesCraweled.Add(url);
            }
        }
        private void AddToPagesToCrawl(string urls)
        {
            if (!string.IsNullOrWhiteSpace(urls))
            {
                // load from file if requested
                if (urls[0] == '/')
                {
                    string filename = urls.Trim('/');
                    List<string> urlsToAdd = FileWriter.LoadFileLines(filename);
                    PagesToCrawl = urlsToAdd;
                }
                // or parse the string
                else
                {
                    List<string> parts = urls.Split(',').ToList();
                    foreach (string p in parts)
                    {
                        PagesToCrawl.Add(p.Trim());
                    }
                }

            }
        }



        public void Run()
        {
            foreach (string p in this.PagesToCrawl)
            {
                var config = CreateConfig();

                this.Crawler = new PoliteWebCrawler(config);
                this.Crawler.PageCrawlStarting += crawler_CrawlerStart;
                this.Crawler.PageCrawlCompleted += crawler_CrawlerComplete;
                this.Crawler.PageCrawlDisallowed += crawler_CrawlerDisalowed;
                this.Crawler.PageLinksCrawlDisallowed += crawler_CrawlerLinkDisalowed;

                CrawlResult result = this.Crawler.Crawl(new Uri(p));
            }

        }

        private void crawler_CrawlerStart(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl page = e.PageToCrawl;
            Console.WriteLine("Starting with {0}", page.Uri.ToString());
        }
        private void crawler_CrawlerComplete(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage page = e.CrawledPage;

            if (page.WebException != null || page.HttpWebResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("## Error on {0}", page.Uri.ToString());
                Console.WriteLine();
                AddToCrawledPages(page.Uri.ToString());
            }
            else
            {
                Console.WriteLine("Crawl OK: {0}", page.Uri.ToString());
                string currentURL = page.Uri.ToString();
                AddToCrawledPages(currentURL);

                if(IncludeMetaData)
                {
                    Console.Write("Extracting meta: ");
                    // identiyfying non page link
                    if (IsPageUrl(currentURL))
                    {

                        AngleSharp.Dom.Html.IHtmlDocument htmlPage = page.AngleSharpHtmlDocument;

                        // output
                        Models.MetaData metaDataDTO = new Models.MetaData();
                        metaDataDTO.Url = currentURL;

                        // options
                        string titleSelector = "title";
                        string metaSelector = "meta";
                        string[] metaAttributes = new[] { "description", "keywords" };

                        // title
                        AngleSharp.Dom.IElement titleElement = htmlPage.QuerySelector(titleSelector);
                        metaDataDTO.Title = (titleElement != null) ? titleElement.TextContent : "";
                        Console.Write("title ");

                        // description
                        AngleSharp.Dom.IHtmlCollection<AngleSharp.Dom.IElement> metaElements = htmlPage.QuerySelectorAll(metaSelector);
                        foreach (var item in metaElements)
                        {
                            if (item.HasAttribute("name"))
                            {
                                if (item.Attributes["name"].Value == metaAttributes[0])
                                {
                                    try
                                    {
                                        metaDataDTO.Description = item.Attributes["content"].Value;
                                    }
                                    catch (Exception)
                                    {

                                        throw;
                                    }
                                    Console.Write(metaAttributes[0] + " ");
                                }
                                if (item.Attributes["name"].Value == metaAttributes[1])
                                {
                                    try
                                    {
                                        metaDataDTO.Keywords = item.Attributes["content"].Value;
                                    }
                                    catch (Exception)
                                    {

                                        throw;
                                    }
                                    Console.Write(metaAttributes[1] + " ");
                                }
                            }
                        }

                        // add
                        MetaData.Add(metaDataDTO);

                        // output to console
                        Console.WriteLine("     OK");
                    }
                    else
                    {
                        Console.WriteLine("     NO VALID PAGE");
                    }
                }

                if (ExtractLinks)
                {


                    AngleSharp.Dom.Html.IHtmlDocument htmlPage = page.AngleSharpHtmlDocument;
                    // get all Links in class selector
                    // generate selector

                    // add prefix if it is set
                    string selector = "a";
                    if (!String.IsNullOrWhiteSpace(ClassSelector))
                    {
                        selector = ClassSelector + " " + selector;
                    }

                    // all links
                    AngleSharp.Dom.IHtmlCollection<AngleSharp.Dom.IElement> links = htmlPage.QuerySelectorAll(selector);

                    // extract links
                    List<string> linksFound = links.Select((x) =>
                    {
                        if (x.HasAttribute("href"))
                        {
                            string linkValue = x.Attributes["href"].Value;

                            // dismiss non valid values
                            if (linkValue.Contains("javascript:")) return null;
                            if (linkValue.Contains("mailto:")) return null;
                            if (linkValue.Contains("tel:")) return null;

                            // base uri
                            string baseUri = page.Uri.GetLeftPart(UriPartial.Authority);

                            if (!linkValue.StartsWith("http"))
                            {
                                linkValue = baseUri + "/" + linkValue;
                            }
                            return linkValue;
                        }
                        return null;
                    }
                    )
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList();

                    // store in dictionary
                    this.PagesCrawledLinks.Add(currentURL, linksFound);
                }

                // new line
                Console.WriteLine();
            }

        }
        private void crawler_CrawlerLinkDisalowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            PageToCrawl page = e.CrawledPage;
            Console.WriteLine("Disallowed: {0}", page.Uri.ToString());
        }
        private void crawler_CrawlerDisalowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl page = e.PageToCrawl;
            Console.WriteLine("Disallowed: {0}", page.Uri.ToString());
        }


        /* 
         * helpers
         */

        public bool IsPageUrl(string url)
        {
            string[] nonValidEndings = new[] { ".jpg", ".png", ".pdf", ".gif", ".ttf" };
            foreach (string e in nonValidEndings)
            {
                if (url.EndsWith(e)) return false;
            }

            return true;
        }
    }
}
