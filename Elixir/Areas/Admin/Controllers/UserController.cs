using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.Admin.Models;
using Elixir.Areas.Admin.ViewModels;
using Elixir.Attributes;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator)]
    public class UserController : BaseController
    {
        private readonly IUsersProcessor _usersProcessor;
        private readonly IRolesProcessor _rolesProcessor;
        private readonly ICountryProcessor _countryProcessor;

        public UserController(IUsersProcessor usersProcessor,
            IRolesProcessor rolesProcessor,
            ICountryProcessor countryProcessor)
        {
            _usersProcessor = usersProcessor;
            _rolesProcessor = rolesProcessor;
            _countryProcessor = countryProcessor;
        }

        // GET: Users
        public ActionResult Index(int sortBy = 0, int direction = 1, int limit = 1)
        {
            var viewModel = new UsersViewModel();

            var sortDirection = (SortDirection)direction;
            var sortOrder = (UsersSortOrder)sortBy;
            var userRecordLimit = (UsersRecordLimit)limit;

            var newSortDirection = SortDirection.Descending;

            switch (sortDirection)
            {
                case SortDirection.Ascending:
                    newSortDirection = SortDirection.Descending;
                    break;
                case SortDirection.Descending:
                    newSortDirection = SortDirection.Ascending;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            viewModel.SortDirection = newSortDirection;
            viewModel.CurrentSortDirection = sortDirection;
            viewModel.SortOrder = sortOrder;
            viewModel.UsersRecordLimit = userRecordLimit;
            viewModel.SelectItems = GetRecordLimitItems(userRecordLimit);
            int? recordsLimit;
            switch (userRecordLimit)
            {
                case UsersRecordLimit.Records_100:
                    recordsLimit = 100;
                    break;
                case UsersRecordLimit.Records_25:
                    recordsLimit = 25;
                    break;
                case UsersRecordLimit.All:
                    recordsLimit = null;
                    break;
                default:
                    recordsLimit = 100;
                    break;
            }
            viewModel.Models = _usersProcessor.GetAllUsers(recordsLimit, sortOrder, sortDirection).Select(x => new BookUserModel(x));
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new CreateUserViewModel
            {
                Model = new BookUserModel()
                {
                    IsEnabled = true
                },
                //SelectItems = _rolesProcessor.GetAllRoles()
                //    .Select(x => new SelectListItem()
                //    {
                //        Text = x.Name,
                //        Value = x.Id.ToString()
                //    })
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateUserViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                userModel = TrimmingData(userModel);
                if (!string.IsNullOrEmpty(userModel.Model.CountryNameWithId) && !string.IsNullOrEmpty(userModel.Model.CountryNameWithId.Trim()))
                {
                    var CountryName = userModel.Model.CountryNameWithId.Trim();
                    var Result = _countryProcessor.SearchCountryByName(CountryName, 1);
                    if (Result != null && Result.Count() > 0)
                    {
                        userModel.Model.CountryId = Result.FirstOrDefault().CountryID;
                    }
                    else
                    {
                        ModelState.AddModelError("Model.CountryNameWithId", "Please select a country from the search results!");
                        return View(userModel);
                    }
                }
                _usersProcessor.CreateUser(userModel.Model);

                return RedirectToAction("Index");
            }

            //userModel.SelectItems = _rolesProcessor.GetAllRoles()
            //    .Select(x => new SelectListItem()
            //    {
            //        Text = x.Name,
            //        Value = x.Id.ToString()
            //    });
            return View(userModel);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var viewModel = new CreateUserViewModel();

            if (id.HasValue == false)
                throw new ArgumentException("Incorrect id of User.", nameof(id));

            var user = _usersProcessor.GetUserById(id.Value);

            if (user == null)
                throw new ContentNotFoundException("The User with specified id does not exist.");

            viewModel.Model = new BookUserModel(user);

            //if (user.BookUserRole != null)
            //{
            //    viewModel.SelectItems = GetSectionsSelectItems(_rolesProcessor.GetAllRoles(), user.BookUserRole.Id);
            //}
            //else
            //{
            //    viewModel.SelectItems = GetSectionsSelectItems(_rolesProcessor.GetAllRoles());
            //}
            return View(viewModel);
        }

        //private IEnumerable<SelectListItem> GetSectionsSelectItems(IEnumerable<BookUserRole> bookSections, int selectedId = -1)
        //{
        //    var items = new List<SelectListItem>();

        //    foreach (var bookSection in bookSections)
        //    {
        //        items.Add(new SelectListItem()
        //        {
        //            Text = bookSection.Name,
        //            Value = bookSection.Id.ToString(),
        //            Selected = bookSection.Id == selectedId
        //        });
        //    }

        //    return items;
        //}

        [HttpPost]
        public ActionResult Edit(CreateUserViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                userModel = TrimmingData(userModel);
                //userModel.Model.DeconstructCountryNameWithId();
                if (!string.IsNullOrEmpty(userModel.Model.CountryNameWithId) && !string.IsNullOrEmpty(userModel.Model.CountryNameWithId.Trim()))
                {
                    var CountryName = userModel.Model.CountryNameWithId.Trim();
                    var Result = _countryProcessor.SearchCountryByName(CountryName, 1);
                    if (Result != null && Result.Count() > 0)
                    {
                        userModel.Model.CountryId = Result.FirstOrDefault().CountryID;
                    }
                    else
                    {
                        ModelState.AddModelError("Model.CountryNameWithId", "Please select a country from the search results!");
                        return View(userModel);
                    }
                }
                _usersProcessor.UpdateUser(userModel.Model);

                return RedirectToAction("Index");
            }

            var user = _usersProcessor.GetUserById(userModel.Model.Id);

            if (user == null)
                throw new ContentNotFoundException("The User with specified id does not exist.");

            //if (user.BookUserRole != null)
            //{
            //    userModel.SelectItems = GetSectionsSelectItems(_rolesProcessor.GetAllRoles(), user.BookUserRole.Id);
            //}
            //else
            //{
            //    userModel.SelectItems = GetSectionsSelectItems(_rolesProcessor.GetAllRoles());
            //}
            return View(userModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id.HasValue == false)
                throw new ArgumentException("Incorrect id of User", nameof(id));

            _usersProcessor.DeleteUser(id.Value);

            return RedirectToAction("Index");
        }

        private dynamic TrimmingData(dynamic userModel)
        {
            if (!string.IsNullOrEmpty(userModel.Model.UserName))
                userModel.Model.UserName = userModel.Model.UserName.Trim();
            if (!string.IsNullOrEmpty(userModel.Model.UserNameLast))
                userModel.Model.UserNameLast = userModel.Model.UserNameLast.Trim();
            if (!string.IsNullOrEmpty(userModel.Model.EmailAddress))
                userModel.Model.EmailAddress = userModel.Model.EmailAddress.Trim();
            if (!string.IsNullOrEmpty(userModel.Model.WebsiteUrl))
                userModel.Model.WebsiteUrl = userModel.Model.WebsiteUrl.Trim();
            if (!string.IsNullOrEmpty(userModel.Model.AdminNotes))
                userModel.Model.AdminNotes = userModel.Model.AdminNotes.Trim();
            return userModel;
        }

        private IEnumerable<SelectListItem> GetRecordLimitItems(UsersRecordLimit selectItem)
        {
            var items = new List<SelectListItem>();
            var limitRecordsTexts = Enum.GetValues(typeof(UsersRecordLimit)).Cast<UsersRecordLimit>().ToList();
            var limitRecordsValues = Enum.GetValues(typeof(UsersRecordLimit)).Cast<UsersRecordLimit>().Cast<int>().ToList();

            for (var i = 0; i < limitRecordsValues.Count; i++)
            {
                items.Add(new SelectListItem()
                {
                    Text = limitRecordsTexts[i].ToString() == "All" ? UsersRecordLimit.All.ToString() : limitRecordsTexts[i].ToString().Split('_')[1],
                    Value = limitRecordsValues[i].ToString(),
                    Selected = selectItem == (UsersRecordLimit)limitRecordsValues[i]
                });
            }
            return items;
        }
    }
}