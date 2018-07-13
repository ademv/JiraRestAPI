using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models.Organization
{
    public class Links
    {

        public string self { get; set; }
        public string Base { get; set; }
        public string context { get; set; }
        public string next { get; set; }
}
}