using System;
using System.Web.Mvc;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models;
using Elixir.Models.Exceptions;
using Elixir.ViewModels.Articles;
using Moq;
using Xunit;

namespace Elixir.xUnit.UnitTests.Controllers
{
    public class PublicArticleControllerTests
    {
        [Fact]
        public void Index_ArticleIdIsNull_ExceptionThrown()
        {
            var article = new Article("Article", "article", DateTime.Today)
            {
                Id = 2
            };

            var ap = new Mock<IArticlesProcessor>();
            ap.Setup(x => x.GetArticleByHash(It.IsAny<string>())).Returns(article).Verifiable();

            var controller = new PublicArticleController(ap.Object, null, null, null, null);

            Assert.Throws<ContentNotFoundException>(() =>
            {
                controller.Index(null);
            });
            ap.Verify(x => x.GetArticleByHash(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Index_ArticleIdDoesNotExist_ExceptionThrown()
        {
            var mock = new Mock<IArticlesProcessor>();
            mock.Setup(x => x.GetArticleByHash(It.IsAny<string>())).Returns(() => null).Verifiable();

            var controller = new PublicArticleController(mock.Object, null, null, null, null);

            Assert.Throws<ContentNotFoundException>(() =>
            {
                controller.Index("zzzzzzz999");
            });
            mock.Verify();
        }

        [Fact]
        public void Index_ArticleIdProvided_ArticleWithRequestedId()
        {
            var article = new Article("Article", "article", DateTime.Today)
            {
                Id = 2,
                IdHashCode = "testhash",
                IsEnabled = true
            };

            var ap = new Mock<IArticlesProcessor>();
            ap.Setup(x => x.GetArticleByHash(It.IsAny<string>())).Returns(article);

            var controller = new PublicArticleController(ap.Object, null, null, null, null);

            var ar = controller.Index("testhash") as ViewResult;

            Assert.NotNull(ar);
            var vm = Assert.IsType<PublicArticleViewModel>(ar.Model);

            Assert.Equal(article.UrlName, vm.Article.UrlName);
        }

        [Fact]
        public void Index_ArticleIsDeleted_ExceptionThrown()
        {
            var article = new Article("Article", "article", DateTime.Today)
            {
                Id = 2,
                IdHashCode = "testhash",
                IsDeleted = true
            };

            var mock = new Mock<IArticlesProcessor>();
            mock.Setup(x => x.GetArticleByHash(It.IsAny<string>())).Returns(article).Verifiable();

            var controller = new PublicArticleController(mock.Object, null, null, null, null);

            Assert.Throws<ContentNotFoundException>(() => { controller.Index("testhash"); });
            mock.Verify(x => x.GetArticleByHash(It.IsAny<string>()), Times.Exactly(1));
        }

        [Fact]
        public void Index_ArticleIsDisabled_ExceptionThrown()
        {
            var article = new Article("Article", "article", DateTime.Today)
            {
                Id = 1,
                IdHashCode = "testhash",
                IsEnabled = false
            };

            var mock = new Mock<IArticlesProcessor>();
            mock.Setup(x => x.GetArticleByHash(It.IsAny<string>())).Returns(article).Verifiable();

            var controller = new PublicArticleController(mock.Object, null, null, null, null);

            Assert.Throws<ContentNotFoundException>(() => { controller.Index("testhash"); });
            mock.Verify(x => x.GetArticleByHash(It.IsAny<string>()), Times.Exactly(1));
        }
    }
}
