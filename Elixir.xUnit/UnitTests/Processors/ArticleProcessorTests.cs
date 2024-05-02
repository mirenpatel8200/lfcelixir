using System;
using System.Linq;
using Elixir.BusinessLogic.Processors;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Moq;
using Xunit;

namespace Elixir.xUnit.UnitTests.Processors
{
    public class ArticleProcessorTests
    {
        private readonly Article[] _articles = {
            new Article("Article 1", "article-url-1", DateTime.Now),
            new Article("Article 2", "article-url-2", DateTime.Now),
            new Article("Article 3", "article-url-3", DateTime.Now),
            new Article("Article 4", "article-url-4", DateTime.Now),
        };

        [Fact]
        public void GetArticleByUrlName_UrlNameExists_ReturnArticle()
        {
            var repoMock = new Mock<IArticlesRepository>();
            repoMock.Setup(x => x.GetArticleByUrlName(It.IsAny<string>())).Returns((string x) => _articles.FirstOrDefault(a => a.UrlName.Equals(x)));
            var ap = new ArticlesProcessor(repoMock.Object, null);

            var articleUrl = "article-url-1";
            var articleFound = ap.GetArticleByUrlName(articleUrl);

            Assert.NotNull(articleFound);
            Assert.Same(articleUrl, articleFound.UrlName);
        }

        [Fact]
        public void GetRelatedArticles_WebPageSpecified_ReturnListOfArticles()
        {
            var ar = new Mock<IArticlesRepository>();

            var ap = new ArticlesProcessor(ar.Object, null) as IArticlesProcessor;

            var art = ap.GetWebPageRelatedArticles(1);
        }
    }
}
