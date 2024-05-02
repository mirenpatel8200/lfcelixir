using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Models.Enums
{
    public enum AuditLogEntityType
    {
        Article= 10,
        ArticleResource=11,
        AuditLog=20,
        BlogPost=30,
        BlogPostResource=31,
        BlogPostTag=32,
        BookChapter=40,
        BookSection=41,
        BookPage=42,
        BookManualPage=44,
        GoLink=80,
        GoLinkLog=81,
        Payment=110,
        ReportDashboard = 121,
        Resource=130,
        ResourceResource=131,
        SearchLog=150,
        Settings=160,
        SocialPost=190,
        Topic=210,
        User =220,//User in pdf
        UserRole=221,
        WebPage=230,
        WebPageTopic=231,
        WebPageType=232

    }
}
