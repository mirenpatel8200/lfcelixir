using System;
using Elixir.BusinessLogic.Processors;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestProject.Processors
{
    [TestClass]
    public class ArticleProcessorTests
    {
        private IArticlesProcessor _articlesProcessor;
        private readonly Article _dbArticle = new Article("Article1", "url 1", DateTime.Today.AddDays(-10))
        {
            Id = 2
        };


        [TestInitialize]
        public void Init()
        {
            var mockDep = new Mock<IArticlesRepository>();
            mockDep.Setup((x) => x.GetArticleById(It.IsAny<int>())).Returns(() => _dbArticle);
            mockDep.Setup(x => x.Update(It.IsAny<Article>())).Callback((Article newArticle) =>
                {
                    _dbArticle.Title = newArticle.Title;
                });

            _articlesProcessor = new ArticlesProcessor(mockDep.Object, null);
        }

        [TestMethod]
        public void Edit_SetNewArticleTitle_ArticleDateTimeShouldNotBeChangedByProcessor()
        {
            var articleId = 4;
            var newName = "TestName2";

            var article = _articlesProcessor.GetArticleById(articleId);

            var prevDate = article.ArticleDate;

            article.Title = newName;

            _articlesProcessor.UpdateArticle(article);

            var updatedArticle = _articlesProcessor.GetArticleById(articleId);

            Assert.AreEqual(newName, updatedArticle.Title);
            Assert.AreEqual(prevDate, updatedArticle.ArticleDate);
        }
    }
}
