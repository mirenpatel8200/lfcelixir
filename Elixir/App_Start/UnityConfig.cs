using System;
using Elixir.BusinessLogic.Core;
using Elixir.BusinessLogic.Core.Api.Buffer;
using Elixir.BusinessLogic.Core.Sitemap;
using Elixir.BusinessLogic.Processors;
using Elixir.BusinessLogic.Processors.SocialPosts;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Core;
using Elixir.Contracts.Interfaces.Database;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.DataAccess.Repositories.MsAccess;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Elixir.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer unityContainer)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            unityContainer.RegisterType<ISectionsProcessor, SectionsProcessor>();
            unityContainer.RegisterType<ISectionsRepository, SectionsRepository>();

            unityContainer.RegisterType<IPagesProcessor, PagesProcessor>();
            unityContainer.RegisterType<IPagesRepository, PagesRepository>();

            unityContainer.RegisterType<IUsersProcessor, UsersProcessor>();
            unityContainer.RegisterType<IUsersRepository, UsersRepository>();

            unityContainer.RegisterType<IRolesProcessor, RolesProcessor>();
            unityContainer.RegisterType<IUserRolesRepository, RolesRepository>();

            unityContainer.RegisterType<ITopicsProcessor, TopicsProcessor>();
            unityContainer.RegisterType<ITopicsRepository, TopicsRepository>();

            unityContainer.RegisterType<IArticlesRepository, ArticlesRepository>();
            unityContainer.RegisterType<IArticlesProcessor, ArticlesProcessor>();

            unityContainer.RegisterType<IResourcesRepository, ResourcesRepository>();
            unityContainer.RegisterType<IResourcesProcessor, ResourcesProcessor>();

            unityContainer.RegisterType<IWebPagesRepository, WebPagesRepository>();
            unityContainer.RegisterType<IWebPagesProcessor, WebPagesProcessor>();

            unityContainer.RegisterType<IWebPageXTopicRepository, WebPageXTopicRepository>();
            unityContainer.RegisterType<IWebPageXTopicProcessor, WebPageXTopicsProcessor>();

            unityContainer.RegisterType<IChaptersRepository, ChaptersRepository>();
            unityContainer.RegisterType<IChaptersProcessor, ChaptersProcessor>();

            unityContainer.RegisterType<IBlogPostsRepository, BlogPostsRepository>();
            unityContainer.RegisterType<IBlogPostsProcessor, BlogPostsProcessor>();

            unityContainer.RegisterType<ISettingsRepository, SettingsRepository>();
            unityContainer.RegisterType<ISettingsProcessor, SettingsProcessor>();

            unityContainer.RegisterType<ISiteMapGenerator, SiteMapGenerator>();
            unityContainer.RegisterType<IRobotsGenerator, RobotsGenerator>();
            unityContainer.RegisterType<IBufferClient, BufferClient>();

            unityContainer.RegisterType<IGoLinksRepository, GoLinksRepository>();
            unityContainer.RegisterType<IGoLinksProcessor, GoLinksProcessor>();

            unityContainer.RegisterType<IGoLinkLogsRepository, GoLinkLogsRepository>();
            unityContainer.RegisterType<IGoLinkLogsProcessor, GoLinkLogsProcessor>();

            unityContainer.RegisterType<IAuditLogsRepository, AuditLogsRepository>();
            unityContainer.RegisterType<IAuditLogsProcessor, AuditLogsProcessor>();

            unityContainer.RegisterType<IWebPageTypesRepository, WebPageTypesRepository>();

            unityContainer.RegisterType<ISocialPostsProcessor, SocialPostsProcessor>();
            unityContainer.RegisterType<IDbManager, MsAccessDbManager>();

            unityContainer.RegisterType<ISearchLogsRepository, SearchLogsRepository>();
            unityContainer.RegisterType<ISearchLogsProcessor, SearchLogsProcessor>();

            unityContainer.RegisterType<IShopProductRepository, ShopProductRepository>();
            unityContainer.RegisterType<IShopProductProcessor, ShopProductProcessor>();

            unityContainer.RegisterType<IShopCategoryRepository, ShopCategoryRepository>();
            unityContainer.RegisterType<IShopCategoryProcessor, ShopCategoryProcessor>();

            unityContainer.RegisterType<IShopOrderRepository, ShopOrderRepository>();
            unityContainer.RegisterType<IShopOrderProcessor, ShopOrderProcessor>();

            unityContainer.RegisterType<IShopOrderProductRepository, ShopOrderProductRepository>();
            unityContainer.RegisterType<IShopOrderProductProcessor, ShopOrderProductProcessor>();

            unityContainer.RegisterType<IShopProductOptionRepository, ShopProductOptionRepository>();
            unityContainer.RegisterType<IShopProductOptionProcessor, ShopProductOptionProcessor>();

            unityContainer.RegisterType<IShopProductOptionRepository, ShopProductOptionRepository>();
            unityContainer.RegisterType<IShopProductOptionProcessor, ShopProductOptionProcessor>();

            unityContainer.RegisterType<IRoleRepository, RoleRepository>();
            unityContainer.RegisterType<IRoleProcessor, RoleProcessor>();

            unityContainer.RegisterType<IUserRoleRepository, UserRoleRepository>();
            unityContainer.RegisterType<IUserRoleProcessor, UserRoleProcessor>();
            
            unityContainer.RegisterType<ICountryRepository, CountryRepository>();
            unityContainer.RegisterType<ICountryProcessor, CountryProcessor>();

            unityContainer.RegisterType<IPaymentRepository, PaymentRepository>();
            unityContainer.RegisterType<IPaymentProcessor, PaymentProcessor>();
        }
    }
}
