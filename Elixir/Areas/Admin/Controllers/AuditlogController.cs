using Elixir.Areas.Admin.Models;
using Elixir.Areas.Admin.ViewModels;
using Elixir.BusinessLogic.Processors;
using Elixir.Contracts.Interfaces;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elixir.Controllers;
using Elixir.Attributes;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator)]
    public class AuditlogController :   BaseController
    {
        private readonly IAuditLogsProcessor _auditLogsProcessor;
        public AuditlogController(IAuditLogsProcessor auditLogsProcessor)
        {
            _auditLogsProcessor = auditLogsProcessor;
        }
        public ActionResult Index(AuditLogSortField sortBy = AuditLogSortField.AuditLogId,
            SortDirection direction = SortDirection.Descending,
            string filter = "",
            int UserId = -1,
            int Entity = 0,
            int EntityId = -1
            )
        {
            if (!Enum.IsDefined(typeof(AuditLogSortField), sortBy))
                throw new ContentNotFoundException("Incorrect sorting parameter.");
            if (!Enum.IsDefined(typeof(SortDirection), direction))
                throw new ContentNotFoundException("Incorrect sort direction parameter.");
            var auditLog = _auditLogsProcessor.GetFilteredAuditLogs(filter, sortBy, direction,UserId,Entity,EntityId).Select(x => new AuditLogModel(x));
            return SortableListView<AuditLogSortField, AuditLogViewModel, AuditLogModel>(auditLog, sortBy, direction);
        }
    }
}