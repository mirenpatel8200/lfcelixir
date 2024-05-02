using Elixir.Contracts.Interfaces;
using Elixir.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class MonitorController : Controller
    {
        private readonly ISettingsProcessor _settingsProcessor;
        public MonitorController(ISettingsProcessor settingsProcessor)
        {
            _settingsProcessor = settingsProcessor;
        }

        // GET: monitor/hellom
        public ActionResult Hellom()
        {
            return View();
        }

        // GET: monitor/dbread
        public ActionResult Dbread()
        {
            var viewModel = new MonitorViewModel();
            try
            {
                viewModel.DbReadOperationResult = _settingsProcessor.GetMonitorPageResponse();
            }
            catch (Exception ex)
            {
                viewModel.DbReadOperationResult = ex.Message;
            }
            return View(viewModel);
        }

        // GET: monitor/dbwrite
        public ActionResult Dbwrite()
        {
            var viewModel = new MonitorViewModel();
            try
            {
                var monitorPageWriteTestSetting = _settingsProcessor.GetSettingsByName("MonitorPageWriteTest");
                monitorPageWriteTestSetting.PairValue = DateTime.Now.ToString();
                _settingsProcessor.UpdateSettigs(monitorPageWriteTestSetting);
                viewModel.DbWriteOperationResult = "Database write OK";
            }
            catch (Exception ex)
            {
                viewModel.DbWriteOperationResult = "Database write FAILED";
            }
            return View(viewModel);
        }
    }
}