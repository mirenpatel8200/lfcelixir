using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Elixir.Contracts.Interfaces.Core;
using Elixir.Contracts.Interfaces.Repositories;

namespace Elixir.BusinessLogic.Core.Sitemap
{
    public class SiteMapGenerator : ISiteMapGenerator
    {
        private const string SitemapsFolderName = "sitemap";
        private const string PagesFileName = "pages-sitemap.xml";
        private const string BlogFileName = "blog-sitemap.xml";

        private readonly IWebPagesRepository _webPagesRepository;
        private readonly IBlogPostsRepository _blogPostsRepository;

        public SiteMapGenerator(IWebPagesRepository webPagesRepository, IBlogPostsRepository blogPostsRepository)
        {
            _webPagesRepository = webPagesRepository;
            _blogPostsRepository = blogPostsRepository;
        }

        public IEnumerable<SitemapInfo> GetSitemapInfo()
        {
            var infos = new List<SitemapInfo>
            {
                new SitemapInfo(ServerRootPath, SitemapsFolderName, PagesFileName),
                new SitemapInfo(ServerRootPath, SitemapsFolderName, BlogFileName)
            };

            return infos;
        }

        /// <summary>
        /// Generates root sitemap.xml that points to specific sitemap files.
        /// </summary>
        /// <returns></returns>
        public string GenerateRootSiteMap()
        {
            var fileNames = new[] { PagesFileName, BlogFileName };

            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var root = new XElement(xmlns + "urlset");

            foreach (var fileName in fileNames)
            {
                var xElement = new XElement(xmlns + "sitemap");
                xElement.Add(new XElement(xmlns + "loc", $"https://liveforever.club/sitemap/{fileName}"));
                xElement.Add(new XElement(xmlns + "lastmod", DateTime.Now));

                root.Add(xElement);
            }

            var document = new XDocument(root);
            return document.ToString();
        }

        private string GeneratePagesSitemap()
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var root = new XElement(xmlns + "urlset");
            var webPages = _webPagesRepository.GetAllWebPagesForSiteMapGenerator().ToList();

            foreach (var webPage in webPages)
            {
                if (webPage.UrlName.Equals("404", StringComparison.OrdinalIgnoreCase)
                    || webPage.UrlName.Equals("500", StringComparison.OrdinalIgnoreCase))
                    continue;

                var xPage = new XElement(xmlns + "url", new XElement(xmlns + "loc", $"https://liveforever.club/page/{webPage.UrlName}"));
                root.Add(xPage);

                if (webPage.IsSubjectPage && !webPage.UrlName.Equals("home", StringComparison.OrdinalIgnoreCase))
                {
                    var xNewsPage = new XElement(xmlns + "url", new XElement(xmlns + "loc", $"https://liveforever.club/page/{webPage.UrlName}/news"));
                    root.Add(xNewsPage);
                }
            }

            var document = new XDocument(root);
            return document.ToString();
        }

        private string GenerateBlogSitemap()
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var root = new XElement(xmlns + "urlset");

            var xBlogRoot = new XElement(xmlns + "url", new XElement(xmlns + "loc", "https://liveforever.club/blog/"));
            root.Add(xBlogRoot);

            for (var y = 2013; y <= DateTime.Now.Year; y++)
            {
                var xBlogYear = new XElement(xmlns + "url", new XElement(xmlns + "loc", $"https://liveforever.club/blog/year/{y}"));
                root.Add(xBlogYear);
            }

            foreach (var blogPost in _blogPostsRepository.GetAll().Where(x => !x.IsDeleted && x.IsEnabled).OrderBy(x => x.UrlName))
            {
                var xBlogPost = new XElement(xmlns + "url", new XElement(xmlns + "loc", $"https://liveforever.club/blog/{blogPost.UrlName}"));
                root.Add(xBlogPost);
            }

            var document = new XDocument(root);
            return document.ToString();
        }

        public void SaveSiteMaps()
        {
            var sitemapsDir = new DirectoryInfo(Path.Combine(ServerRootPath, SitemapsFolderName));
            if (!sitemapsDir.Exists)
            {
                sitemapsDir.Create();
                sitemapsDir.Refresh();
            }

            var fiPages = new FileInfo(Path.Combine(sitemapsDir.FullName, PagesFileName));
            using (var sw = fiPages.CreateText())
            {
                var xmlPages = GeneratePagesSitemap();

                sw.Write(xmlPages);
                sw.Close();

                fiPages.Refresh();
            }

            var fiBlogs = new FileInfo(Path.Combine(ServerRootPath, SitemapsFolderName, BlogFileName));
            using (var sw = fiBlogs.CreateText())
            {
                var xmlBlogs = GenerateBlogSitemap();

                sw.Write(xmlBlogs);
                sw.Close();

                fiBlogs.Refresh();
            }
        }

        public string GetSiteMapUrl()
        {
            return "/" + ServerRootPath;
        }

        public string ServerRootPath { get; set; }
    }
}
