using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elixir.ViewModels
{
    public class SubjectResourcesViewModel
    {
        public string Title { get; set; }
        public string WebPageTitle { get; set; }
        public string Description { get; set; }
        public string LinkToPageUrl { get; set; }
        public string LinkToPageText { get; set; }
        public string IntroText { get; set; }

        public List<Resource> RecentRelatedResources { get; set; }
        public List<Resource> CreationResources { get; set; }

        public List<Resource> Applications { get; set; }
        public List<Resource> Audio { get; set; }
        public List<Resource> Books { get; set; }
        public List<Resource> Competitions { get; set; }
        public List<Resource> Education { get; set; }
        public List<Resource> Events { get; set; }
        public List<Resource> Films { get; set; }
        public List<Resource> Information { get; set; }
        public List<Resource> Products { get; set; }
        public List<Resource> Research { get; set; }
        public List<Resource> Videos { get; set; }

        public List<Resource> PeopleResources { get; set; }

        public List<Resource> Academics { get; set; }
        public List<Resource> Advocates { get; set; }
        public List<Resource> Artists { get; set; }
        public List<Resource> Authors { get; set; }
        public List<Resource> CompanyRepresentatives { get; set; }
        public List<Resource> HealthcareProfessionals { get; set; }
        public List<Resource> Journalists { get; set; }
        public List<Resource> Politicians { get; set; }

        public List<Resource> OrganisationResources { get; set; }

        public List<Resource> Academia { get; set; }
        public List<Resource> Companies { get; set; }
        public List<Resource> HealthcareOrganisations { get; set; }
        public List<Resource> Institutes { get; set; }
        public List<Resource> Journals { get; set; }
        public List<Resource> Publisher { get; set; }

        public int CountOfRelatedResources { get; set; }
    }
}