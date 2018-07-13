using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models.Organization
{
    public class OranizationsBag
    {

        public int size { get; set; }
        public int start { get; set; }
        public int limit { get; set; }
        public bool isLastPage { get; set; }
        public Links _links { get; set; }
        public IList<Organization> values { get; set; }
    }
}