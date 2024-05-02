using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Web.Mvc;
using Elixir.Contracts.Interfaces.Database;
using Elixir.Contracts.Interfaces.Repositories;
using Microsoft.Practices.ServiceLocation;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public abstract class AbstractRepository<TEntity> : EntitiesMap, IRepository<TEntity>
    {
        protected const string TableNameBookSections = "BookSection";
        protected const string TableNameBookPages = "BookPage";
        protected const string TableNameUsers = "[Users]";
        //protected const string TableNameUserRoles = "UserRole";
        protected const string TableNameTopics = "Topic";
        protected const string TableNameArticles = "Article";
        protected const string TableNameWebPages = "WebPage";
        protected const string TableNameWebPageTopic = "WebPageTopic";
        protected const string TableNameBookChapter = "BookChapter";
        protected const string TableNameBlogPost = "BlogPost";
        protected const string TableNameSettings = "[Settings]";
        protected const string TableNameGoLinks = "[GoLink]";
        protected const string TableNameGoLinkLogs = "[GoLinkLog]";
        protected const string TableNameResources = "Resource";
        protected const string TablenameWebPageTypes = "WebPageType";
        protected const string TableNameArticleResources = "ArticleResource";
        protected const string TableNameBlogPostResources = "BlogPostResource";
        protected const string TableNameSearchLogs = "SearchLog";
        protected const string TableNameResourceResources = "ResourceResource";
        protected const string TableNameAuditLogs = "[AuditLog]";
        protected const string TableNameShopProduct = "[ShopProduct]";
        protected const string TableNameShopCategory = "[ShopCategory]";
        protected const string TableNameShopOrder = "[ShopOrder]";
        protected const string TableNameShopOrderProduct = "[ShopOrderProduct]";
        protected const string TableNameShopProductOption = "[ShopProductOption]";
        protected const string TableNameRole = "[Role]";
        protected const string TableNameUserRole = "[UserRole]";
        protected const string TableNameCountry = "[Country]";
        protected const string TableNamePayment = "[Payment]";

        protected IDbManager MsAccessDbManager;

        private void AddParameters(OleDbCommand command, Dictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
            }
        }

        protected IEnumerable<TEntity> QueryAllData(string sql, Action<List<TEntity>> constructObjectMethod, Dictionary<string, object> parameters = null)
        {
            return QueryAllData<TEntity>(sql, constructObjectMethod, parameters);
        }

        protected IEnumerable<TResult> QueryAllData<TResult>(string sql, Action<List<TResult>> constructObjectMethod, Dictionary<string, object> parameters = null)
        {
            var list = new List<TResult>();

            using (MsAccessDbManager = DependencyResolver.Current.GetService<IDbManager>())
            {
                var command = MsAccessDbManager.CreateCommand(sql);
                AddParameters(command, parameters);

                DataReader = command.ExecuteReader();

                while (DataReader.Read())
                {
                    constructObjectMethod?.Invoke(list);
                }

                DataReader.Close();
            }

            return list;
        }

        protected int QueryScalar(string sql, Dictionary<string, object> parameters = null)
        {
            int result;
            using (MsAccessDbManager = DependencyResolver.Current.GetService<IDbManager>())
            {
                var command = MsAccessDbManager.CreateCommand(sql);
                AddParameters(command, parameters);

                result = Convert.ToInt32(command.ExecuteScalar());
            }

            return result;
        }

        protected String SafeGetStringValue(String inputValue)
        {
            return inputValue ?? "";
        }

        public abstract void Insert(TEntity entity);

        public abstract void Update(TEntity entity);

        public abstract void Delete(int id);

        public abstract IEnumerable<TEntity> GetAll();

        public DateTime GetDateWithoutMilliseconds(DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
        }
    }
}
