using Elixir.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elixir.Models.Json;
using Elixir.Utils.View;

namespace Elixir.Areas.Admin.Controllers
{
    public class CountryController : Controller
    {
        private readonly ICountryProcessor _countryProcessor;
        public CountryController(ICountryProcessor countryProcessor)
        {
            _countryProcessor = countryProcessor;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult FetchAutocompleteForCountryName(string term)
        {
            var list = _countryProcessor.SearchCountry(term, 10)
                .Select(x => new AutocompleteJson
                (                   
                    x.CountryName,                    
                    x.CountryName
                    ));
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        
    }
}