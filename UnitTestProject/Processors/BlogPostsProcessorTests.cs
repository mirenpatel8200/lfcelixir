using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.BusinessLogic.Processors;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestProject.Processors
{
    [TestClass]
    public class BlogPostsProcessorTests
    {
        private IBlogPostsProcessor _blogPostsProcessor;

        private readonly BlogPost _dbBlogPost = new BlogPost()
        {
            Id = 2,
            CreatedDt = DateTime.Today.AddDays(-5),
            UpdatedDt = DateTime.Today.AddDays(-3),
            BlogPostTitle = "BpTitle",
            UrlName = "urlname"
        };

        [TestInitialize]
        public void Init()
        {
            var mockObj = new Mock<IBlogPostsRepository>();

            mockObj.Setup(x => x.GetAll()).Returns(new List<BlogPost>()
            {
                _dbBlogPost
            });
            mockObj.Setup(x => x.Update(It.IsAny<BlogPost>())).Callback((BlogPost bp) =>
                {
                    _dbBlogPost.BlogPostTitle = bp.BlogPostTitle;
                    _dbBlogPost.UpdatedDt = bp.UpdatedDt;
                });

            _blogPostsProcessor = new BlogPostsProcessor(mockObj.Object, null, null, null);
        }

        [TestMethod]
        public void Edit_UpdateBlogPostTitle_ProcessorChangesUpdatedTimeOfBlogPost()
        {
            var bp = _blogPostsProcessor.GetAll().FirstOrDefault();
            var newTitle = "BpUpdatedTitle";
            var bpUpdatingId = bp.Id.Value;
            var bpUpdateTime = bp.UpdatedDt;

            bp.BlogPostTitle = newTitle;

            _blogPostsProcessor.UpdateBlogPost(bp);

            var updatedBp = _blogPostsProcessor.GetById(bpUpdatingId);

            Assert.AreEqual(newTitle, updatedBp.BlogPostTitle);
            Assert.AreNotEqual(bpUpdateTime, updatedBp.UpdatedDt);
        }
    }
}
