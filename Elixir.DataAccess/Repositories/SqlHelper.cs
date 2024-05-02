using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.DataAccess.Repositories
{
    public static class SqlHelper
    {
        public static string ConditionArticleContains(string termAlias)
        {
            string condition =
                $"(" +
                    $"(" +
                        $" a.ArticleTitle LIKE @{termAlias} " 
                        + $" OR a.ArticleTitle LIKE @start{termAlias} " 
                        + $" OR a.ArticleTitle LIKE @punctuation{termAlias}"
                        +
                    $")" +
                    " OR " +
                    $"(" +
                        $" a.Summary LIKE @{termAlias} " 
                        + $" OR a.Summary LIKE @start{termAlias} " 
                        + $" OR a.Summary LIKE @punctuation{termAlias}"
                        +
                    $")" +
                    " OR " +
                    $"(" +
                        $" a.BulletPoints LIKE @{termAlias} " 
                        + $" OR a.BulletPoints LIKE @start{termAlias} " 
                        + $" OR a.BulletPoints LIKE @punctuation{termAlias}"
                        +
                    $")" +
                $") ";
            return condition;
        }

        public static string ConditionWebPageContains(string termAlias)
        {
            string condition =
                $"(" +
                    $"(" +
                        $" wp.WebPageTitle LIKE @{termAlias} OR " +
                        $" wp.WebPageTitle LIKE @start{termAlias} OR " +
                        $" wp.WebPageTitle LIKE @punctuation{termAlias}" +
                    $")" +
                    " OR " +
                    $"(" +
                        $" wp.ContentMain LIKE @{termAlias} OR " +
                        $" wp.ContentMain LIKE @start{termAlias} OR " +
                        $" wp.ContentMain LIKE @punctuation{termAlias}" +
                    $")" +
                $") ";
            return condition;
        }

        public static string ConditionBlogPostContains(string termAlias)
        {
            string condition =
                $"(" +
                    $"(" +
                        $" bp.BlogPostTitle LIKE @{termAlias} OR " +
                        $" bp.BlogPostTitle LIKE @start{termAlias} OR " +
                        $" bp.BlogPostTitle LIKE @punctuation{termAlias}" +
                    $")" +
                    " OR " +
                    $"(" +
                        $" bp.ContentMain LIKE @{termAlias} OR " +
                        $" bp.ContentMain LIKE @start{termAlias} OR " +
                        $" bp.ContentMain LIKE @punctuation{termAlias}" +
                    $")" +
                $") ";
            return condition;
        }
    }
}
