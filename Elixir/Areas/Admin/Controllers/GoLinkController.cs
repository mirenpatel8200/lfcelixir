using System;
using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.Admin.Models;
using Elixir.Areas.Admin.Views.Article;
using Elixir.Areas.Admin.Views.GoLink;
using Elixir.Attributes;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using Elixir.Models.Utils;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator)]
    public class GoLinkController : BaseController
    {
        private readonly IGoLinksProcessor _goLinksProcessor;
        private readonly IGoLinkLogsProcessor _goLinkLogsProcessor;

        public GoLinkController(IGoLinksProcessor goLinksProcessor, IGoLinkLogsProcessor goLinkLogsProcessor)
        {
            _goLinksProcessor = goLinksProcessor;
            _goLinkLogsProcessor = goLinkLogsProcessor;
        }

        // GET: Admin/GoLink
        public ActionResult Index(GoLinksSortOrder sortBy = GoLinksSortOrder.GoLinkID, SortDirection direction = SortDirection.Descending)
        {
            if (!Enum.IsDefined(typeof(GoLinksSortOrder), sortBy))
                throw new ContentNotFoundException("Incorrect sorting parameter.");
            if (!Enum.IsDefined(typeof(SortDirection), direction))
                throw new ContentNotFoundException("Incorrect sort direction parameter.");

            var goLinks = _goLinksProcessor.Get100GoLinks(sortBy, direction).Select(x => new GoLinkModel(x));

            return SortableListView<GoLinksSortOrder, GoLinksListViewModel, GoLinkModel>(goLinks, sortBy, direction);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var vm = new CreateGoLinkViewModel()
            {
                Model = new GoLinkModel()
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult Create(CreateGoLinkViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _goLinksProcessor.CreateGoLink(vm.Model);
                    return RedirectToAction("Index");
                }
                catch (ArgumentException e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var vm = new EditGoLinkViewModel
            {
                Model = new GoLinkModel(_goLinksProcessor.GetById(id))
            };

            if (vm.Model == null)
                throw new ContentNotFoundException("Unable to find GoLink with specified Id.");

            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit(EditGoLinkViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _goLinksProcessor.UpdateGoLink(vm.Model);
                    return RedirectToAction("Index");
                }
                catch (ArgumentException e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            return View(vm);
        }

        public ActionResult Delete(int id)
        {
            _goLinksProcessor.SoftDeleteGoLink(id);

            return RedirectToAction("Index");
        }
    }
}