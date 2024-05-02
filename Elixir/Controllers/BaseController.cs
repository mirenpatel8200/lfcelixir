using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Elixir.Models.Enums;
using Elixir.Models.Identity;
using Elixir.ViewModels.Base;
using Microsoft.AspNet.Identity.Owin;

namespace Elixir.Controllers
{
    public class BaseController : Controller
    {
        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    var httpEx = filterContext.Exception as ContentNotFoundException;

        //    if (httpEx != null && httpEx.GetHttpCode() == 404)
        //    {
        //        filterContext.ExceptionHandled = true;
        //        filterContext.Result = RedirectToAction("Index", "WebPageVisual", new { name = "404", area = "" });
        //    } 
        //}

        private AccessSignInMagager _signInManager;


        protected AccessSignInMagager AccessSignInManager => _signInManager ?? HttpContext.GetOwinContext().Get<AccessSignInMagager>();

        protected ActionResult SortableListView<TSortOrder, TViewModel, TModel>(
            IEnumerable<TModel> models,
            TSortOrder sortBy,
            SortDirection sortDirection)
            where TViewModel : BaseSortableListViewModel<TModel, TSortOrder>, new()
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            var viewModel = new TViewModel
            {
                SortDirection = FlipSortDirection(sortDirection),
                SortOrder = sortBy,
                CurrentSortDirection = sortDirection,
                CurrentSortOrder = sortBy,
                Models = models
            };

            return View(viewModel);
        }

        protected ActionResult SortableListView<TSortOrder, TViewModel, TModel>(
            IEnumerable<TModel> models,
            TSortOrder sortBy,
            SortDirection sortDirection,
            TViewModel viewModelInstance)
            where TViewModel : BaseSortableListViewModel<TModel, TSortOrder>, new()
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            viewModelInstance.SortDirection = FlipSortDirection(sortDirection);
            viewModelInstance.SortOrder = sortBy;
            viewModelInstance.CurrentSortDirection = sortDirection;
            viewModelInstance.CurrentSortOrder = sortBy;
            viewModelInstance.Models = models;

            return View(viewModelInstance);
        }

        private SortDirection FlipSortDirection(SortDirection sortDirection)
        {
            switch (sortDirection)
            {
                case SortDirection.Ascending:
                    return SortDirection.Descending;
                case SortDirection.Descending:
                    return SortDirection.Ascending;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}