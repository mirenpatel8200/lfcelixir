using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.BusinessLogic.Processors
{
    public class AuditLogsProcessor : IAuditLogsProcessor
    {
        private readonly IAuditLogsRepository _auditLogsRepository;
        private readonly ISettingsRepository _settingsRepository;

        public AuditLogsProcessor(IAuditLogsRepository auditLogsRepository, ISettingsRepository settingsRepository)
        {
            _auditLogsRepository = auditLogsRepository;
            _settingsRepository = settingsRepository;
        }

        public void Log(AuditLog model)
        {
            if (!IsIpIgnored(model.IpAddressString))
            {
                var logRecord = new AuditLog()
                {
                    CreatedDT = DateTime.Now,
                    IpAddressString = model.IpAddressString,
                    UserID = model.UserID,
                    EntityTypeID = model.EntityTypeID,
                    EntityID = model.EntityID,
                    ActionTypeID = model.ActionTypeID,
                    NotesLog = model.NotesLog
                };

                _auditLogsRepository.Insert(logRecord);
            }
        }

        public void ArticleLinkTrackLog(int userId, string ipAddress, int articleId, string notesLog)
        {
            var logRecord = new AuditLog()
            {
                CreatedDT = DateTime.Now,
                IpAddressString = ipAddress,
                UserID = userId,
                EntityTypeID = (byte)AuditLogEntityType.Article,
                EntityID = articleId,
                ActionTypeID = (byte)AuditLogActionType.View,
                NotesLog = notesLog
            };
            _auditLogsRepository.Insert(logRecord);
        }

        public void CreateAuditLog(AuditLog auditLog)
        {
            _auditLogsRepository.Insert(auditLog);
        }

        private bool IsIpIgnored(string ipAddress)
        {
            var settingsEntry = _settingsRepository.GetByPairName("LogIgnoreIP");
            if (settingsEntry == null || string.IsNullOrEmpty(settingsEntry.PairValue))
                return false;
            var ips = settingsEntry.PairValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return ips.Any(x => x.Equals(ipAddress, StringComparison.OrdinalIgnoreCase));
        }
        public int NoOfFailedLoginsForUserInSeconds(int seconds, string userEmail, int userId)
        {
            return _auditLogsRepository.NoOfFailedLoginsForUserInSeconds(seconds, userEmail, userId);
        }
        public int NoOfFailedLoginsByHours(int hours = 0)
        {
            return _auditLogsRepository.NoOfFailedLoginsByHours(hours);
        }
        public int NoOfFailedLoginsByDays(int days = 0)
        {
            return _auditLogsRepository.NoOfFailedLoginsByDays(days);
        }

        public IEnumerable<AuditLog> Get100AuditLog(AuditLogSortField sortField, SortDirection sortDirection, int UserId, int Entity, int EntityId)

        {
            var sortFields = new[] { sortField, AuditLogSortField.AuditLogId };
            var sortDirections = new[] { sortDirection, SortDirection.Ascending };

            var allAuditLogs = _auditLogsRepository.GetN(100, sortFields, sortDirections, UserId, Entity, EntityId);

            return allAuditLogs;
        }

        public IEnumerable<AuditLog> GetFilteredAuditLogs(string filter, AuditLogSortField sortField, SortDirection sortDirection, int UserId, int Entity, int EntityId)
        {
            var sortFields = new[] { sortField, AuditLogSortField.AuditLogId };
            var sortDirections = new[] { sortDirection, SortDirection.Ascending };
            IEnumerable<AuditLog> auditLog;
            if (string.IsNullOrEmpty(filter))
            {
                auditLog = Get100AuditLog(sortField, sortDirection, UserId, Entity, EntityId);
            }
            else
            {
                auditLog = _auditLogsRepository.GetFiltered(filter, sortFields, sortDirections);
            }

            return auditLog;
        }
    }
}
