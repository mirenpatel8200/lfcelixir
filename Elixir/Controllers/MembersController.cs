using Elixir.Contracts.Interfaces;
using Elixir.Models.Exceptions;
using Elixir.Models.Utils;
using Elixir.ViewModels;
using Elixir.ViewModels.Enums;
using Elixir.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class MembersController : BaseController
    {
        private readonly IUsersProcessor _usersProcessor;
        private readonly ISettingsProcessor _settingsProcessor;

        public MembersController(
            IUsersProcessor usersProcessor,
            ISettingsProcessor settingsProcessor)
        {
            _usersProcessor = usersProcessor;
            _settingsProcessor = settingsProcessor;
        }

        // GET: members/{name}
        public ActionResult Index(string name = null)
        {
            ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);

            if (string.IsNullOrWhiteSpace(name))
                throw new ContentNotFoundException("Member's name has to be specified.");

            var user = _usersProcessor.GetByIdHashCode(name);
            if (user == null || !user.IsEnabled || !user.ProfileIsPublic)
            {
                throw new ContentNotFoundException(
                    "The page with the specified user does not exist.");
            }

            if (name.Equals("404", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 404;
            else if (name.Equals("500", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 500;

            var isUserName = !string.IsNullOrEmpty(user.UserNameDisplay);
            ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);

            var bannerImageName = AppConstants.DefaultBannerName;
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath, AppConstants.FileSystem.WebPageBannerImageFolderServerPath + bannerImageName);

            var ogImagePath = AppConstants.DefaultBannerNameSocial;
            ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName, "/" + AppConstants.FileSystem.PublicImagesFolderName + "/" + ogImagePath);

            var pageTitle = isUserName ? user.UserNameDisplay : "Member Profile";

            // Meta-description.
            var metaDescription = isUserName ? $"{user.UserNameDisplay} member's details" : "Live Forever Club member's details";
            ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription, metaDescription);
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImageMeta, new BannerImageMetaVm()
            {
                AltText = "Member profile page banner",
                ImgTitle = metaDescription
            });

            var isVisibleNewFlashText = _settingsProcessor.GetSettingsByName("IsVisibleNewFlashText");
            if (isVisibleNewFlashText != null)
            {
                ViewData.AddOrUpdateValue(ViewDataKeys.IsVisibleNewFlashText, isVisibleNewFlashText);
                if (!string.IsNullOrEmpty(isVisibleNewFlashText.PairValue) && Convert.ToBoolean(isVisibleNewFlashText.PairValue))
                {
                    var newFlashText = _settingsProcessor.GetSettingsByName("NewFlashText");
                    if (newFlashText != null)
                        ViewData.AddOrUpdateValue(ViewDataKeys.NewFlashText, newFlashText);
                }
            }

            var viewModel = new MemberViewModel
            {
                Member = user
            };

            return View(viewModel);
        }
    }
}