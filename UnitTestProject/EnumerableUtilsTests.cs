using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.BusinessLogic.Core.Utils;
using Elixir.Models;
using Elixir.Models.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class EnumerableUtilsTests
    {
        private readonly Random _random = new Random();

        [TestMethod]
        public void SortByEnum_AscSortArticleById_ListSortedByIdAsc()
        {
            var list = new List<Article>()
            {
                new Article("Article 1", "url 1", DateTime.Now) {Id = 1},
                new Article("Article 2", "url 2", DateTime.Now) {Id = 2},
                new Article("Article 3", "url 3", DateTime.Now) {Id = 3},
                new Article("Article 4", "url 4", DateTime.Now) {Id = 4},
            }.OrderBy(x => _random.Next());

            var sorted = list.SortByEnum(ArticlesSortField.IsEnabled, SortDirection.Ascending).ToList();

            for (int i = 1; i < sorted.Count; i++)
            {
                Assert.IsTrue(sorted[i].Id.Value > sorted[i - 1].Id.Value);
            }
        }
    }
}
