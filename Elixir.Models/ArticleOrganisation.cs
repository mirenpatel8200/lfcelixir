using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Models
{
    class ArticleOrganisation : BaseEntity
    {
        public ArticleOrganisation()
        {
            
        }

        public ArticleOrganisation(int articleId, int organisationId)
        {
            ArticleId = articleId;
            OrganisationId = organisationId;
        }

        public virtual int ArticleId { get; set; }
        public virtual int OrganisationId { get; set; }
    }
}
