using System;
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

        protected List<string> PagesToCrawl = new List<string>();
        public List<string> PagesCraweled = new List<string>();
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
                Console.WriteLine();
                AddToCrawledPages(page.Uri.ToString());
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
    }
}
