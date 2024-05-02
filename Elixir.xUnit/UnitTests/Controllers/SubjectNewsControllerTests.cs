using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using Elixir.Models.Utils;
using Elixir.ViewModels.SubjectNews;
using Elixir.Views;
using Moq;
using Xunit;

namespace Elixir.xUnit.UnitTests.Controllers
{
    public class SubjectNewsControllerTests
    {
        private WebPage CreateValidWebPage() => new WebPage()
        {
            Id = 1,
            IsEnabled = true,
            IsDeleted = false,
            IsSubjectPage = true
        };

        [Fact]
        public void Controller_PubliclyAccessible()
        {
            var controller = new SubjectNewsController(null, null, null);
            var type = controller.GetType();
            var attr = type.GetCustomAttributes(typeof(AllowAnonymousAttribute));

            Assert.True(attr.Any());
        }

        [Fact]
        public void Index_ReturnsSubjectNewsViewModel()
        {
            var wpMock = new Mock<IWebPagesProcessor>();

            var wp = CreateValidWebPage();
            wp.UrlName = "diet";
            wp.WebPageName = "Diet";

            wpMock.Setup(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page)).Returns(wp).Verifiable();
            var wptp = new Mock<IWebPageXTopicProcessor>();
            var ap = new Mock<IArticlesProcessor>();

            var controller = new SubjectNewsController(ap.Object, wpMock.Object, null);
            var vr = Assert.IsType<ViewResult>(controller.Index("diet"));
            Assert.IsType<SubjectNewsViewModel>(vr.Model);
        }

        [Fact]
        public void Index_WebpageNameIsDiet_TitleIsDietNews()
        {
            var wpMock = new Mock<IWebPagesProcessor>();

            var wp = CreateValidWebPage();
            wp.UrlName = "diet";
            wp.WebPageName = "Diet";

            wpMock.Setup(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page)).Returns(wp).Verifiable();
            var wptp = new Mock<IWebPageXTopicProcessor>();
            var ap = new Mock<IArticlesProcessor>();

            var controller = new SubjectNewsController(ap.Object, wpMock.Object, null);
            var vr = Assert.IsType<ViewResult>(controller.Index("diet"));
            var vm = vr.Model as SubjectNewsViewModel;

            Assert.Equal("Diet News", vm.Title);

            wpMock.Verify(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page), Times.Exactly(1));
        }

        [Fact]
        public void Index_WebpageNameIsDiet_DescriptionIsAccordingToSpec()
        {
            var wpMock = new Mock<IWebPagesProcessor>();

            var wp = CreateValidWebPage();
            wp.UrlName = "diet";
            wp.WebPageName = "Diet";

            wpMock.Setup(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page)).Returns(wp).Verifiable();
            var wptp = new Mock<IWebPageXTopicProcessor>();
            var ap = new Mock<IArticlesProcessor>();

            var controller = new SubjectNewsController(ap.Object, wpMock.Object, null);
            var vr = Assert.IsType<ViewResult>(controller.Index("diet"));
            var vm = vr.Model as SubjectNewsViewModel;

            Assert.Equal("Latest news and articles regarding Diet", vm.Description);

            wpMock.Verify(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page), Times.Exactly(1));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Index_PageDoesntHaveBanner_DefaultBannerReturned(string banner)
        {
            var wpMock = new Mock<IWebPagesProcessor>();

            var wp = CreateValidWebPage();
            wp.UrlName = "diet";
            wp.WebPageName = "Diet";
            wp.BannerImageFileName = banner;

            wpMock.Setup(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page)).Returns(wp).Verifiable();
            var wptp = new Mock<IWebPageXTopicProcessor>();
            var ap = new Mock<IArticlesProcessor>();

            var controller = new SubjectNewsController(ap.Object, wpMock.Object, null);
            var vr = Assert.IsType<ViewResult>(controller.Index("diet"));

            Assert.True(vr.ViewData.GetValue<bool>(ViewDataKeys.EnableImageBanner));
            Assert.Equal(AppConstants.DefaultBannerName, vr.ViewData.GetValue<string>(ViewDataKeys.BannerImagePath));

            wpMock.Verify(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page), Times.Exactly(1));
        }

        [Fact]
        public void Index_PageHasBanner_ItsBannerPathIsReturned()
        {
            var wpMock = new Mock<IWebPagesProcessor>();

            var wp = CreateValidWebPage();
            wp.UrlName = "diet";
            wp.WebPageName = "Diet";
            wp.BannerImageFileName = "pages-banner-image.jpg";

            wpMock.Setup(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page)).Returns(wp).Verifiable();
            var wptp = new Mock<IWebPageXTopicProcessor>();
            var ap = new Mock<IArticlesProcessor>();

            var controller = new SubjectNewsController(ap.Object, wpMock.Object, null);
            var vr = Assert.IsType<ViewResult>(controller.Index("diet"));

            var vdBanner = vr.ViewData.GetValue<string>(ViewDataKeys.BannerImagePath);
            Assert.True(vr.ViewData.GetValue<bool>(ViewDataKeys.EnableImageBanner));
            Assert.NotNull(vdBanner);
            Assert.Equal("pages-banner-image.jpg", vdBanner);

            wpMock.Verify(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page), Times.Exactly(1));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Index_WebpageUrlIsEmpty_ThrowException(string url)
        {
            var wpMock = new Mock<IWebPagesProcessor>();
            wpMock.Setup(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page)).Verifiable();
            var wptp = new Mock<IWebPageXTopicProcessor>();
            var ap = new Mock<IArticlesProcessor>();

            var controller = new SubjectNewsController(ap.Object, wpMock.Object, null);
            Assert.Throws<ContentNotFoundException>(() => { controller.Index(url); });

            wpMock.Verify(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page), Times.Never);
        }

        [Fact]
        public void Index_WebpageIsDisabled_ThrowException()
        {
            var wpMock = new Mock<IWebPagesProcessor>();

            var wp = CreateValidWebPage();
            wp.UrlName = "diet";
            wp.IsEnabled = false;

            wpMock.Setup(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page)).Returns(wp).Verifiable();
            var wptp = new Mock<IWebPageXTopicProcessor>();
            var ap = new Mock<IArticlesProcessor>();

            var controller = new SubjectNewsController(ap.Object, wpMock.Object, null);
            Assert.Throws<ContentNotFoundException>(() => { controller.Index("diet"); });
        }

        [Fact]
        public void Index_WebpageIsDeleted_ThrowException()
        {
            var wpMock = new Mock<IWebPagesProcessor>();

            var wp = CreateValidWebPage();
            wp.UrlName = "diet";
            wp.IsDeleted = true;

            wpMock.Setup(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page)).Returns(wp).Verifiable();
            var wptp = new Mock<IWebPageXTopicProcessor>();
            var ap = new Mock<IArticlesProcessor>();

            var controller = new SubjectNewsController(ap.Object, wpMock.Object, null);
            Assert.Throws<ContentNotFoundException>(() => { controller.Index("diet"); });
        }

        [Fact]
        public void Index_WebpageIsNotSubjectPage_ThrowNotFoundException()
        {
            var wpMock = new Mock<IWebPagesProcessor>();

            var wp = CreateValidWebPage();
            wp.UrlName = "diet";
            wp.IsDeleted = true;
            wp.IsSubjectPage = false;

            wpMock.Setup(x => x.GetWebPageByUrlName(It.IsAny<string>(), (int)EnumWebPageType.Page)).Returns(wp).Verifiable();
            var wptp = new Mock<IWebPageXTopicProcessor>();
            var ap = new Mock<IArticlesProcessor>();

            var controller = new SubjectNewsController(ap.Object, wpMock.Object, null);
            Assert.Throws<ContentNotFoundException>(() => { controller.Index("diet"); });
        }

        [Fact]
        public void Index_HomePageNewsRequested_ThrowsNotFoundException()
        {
            var wpMock = new Mock<IWebPagesProcessor>();
            var wptp = new Mock<IWebPageXTopicProcessor>();
            var ap = new Mock<IArticlesProcessor>();

            var controller = new SubjectNewsController(ap.Object, wpMock.Object, null);
            Assert.Throws<ContentNotFoundException>(() => { controller.Index("home"); });
        }
    }
}
