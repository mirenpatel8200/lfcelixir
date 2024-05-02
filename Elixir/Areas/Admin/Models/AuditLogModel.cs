using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Elixir.Models;
using Elixir.Models.Validation;
using Elixir.Utils.Reflection;
using Elixir.Utils.View;

namespace Elixir.Areas.Admin.Models
{
    public sealed class AuditLogModel : AuditLog
    {
        public AuditLogModel()
        {
        }

        public AuditLogModel(AuditLog model)
        {
            ReflectionUtils.ClonePublicProperties(model, this);
        }

        
    }
}