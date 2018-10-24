using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models.IMS
{
    public class CompanyIssueComment
    {
        public string JQL { get; set; }
        public string Comment { get; set; }
        public string Company { get; set; }
        public string OrganizationId { get; set; }
        public int customField_10075 { get; set; }
    }
}