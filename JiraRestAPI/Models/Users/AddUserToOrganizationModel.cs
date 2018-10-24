using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models.Users
{
    public class UserToOrganizationModel
    {
        public string CurrentOgranizationId { get; set; }
        public string OrganizationId { get; set; }
        public  string username { get; set; }
        public  bool IssueUpdate { get; set; }
        public string UserToAssign { get; set; }
    }
}