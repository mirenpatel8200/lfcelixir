using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Models.Enums
{
    public enum AuditLogActionType
    {
        LoginSuccess = 10,
        LoginFailure = 11,
        Logout = 12,
        LoginRestricted = 13,
        CookieRefresh = 15,
        AuthorisationSuccess = 20,
        AuthorisationFailure = 21,
        Create = 30,
        CreateFailure = 31,
        Delete = 40,
        Update = 70,
        UpdateFailure = 71,
        View = 80
    }
}
