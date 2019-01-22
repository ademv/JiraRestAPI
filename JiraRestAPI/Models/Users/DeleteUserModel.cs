using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models.Users
{
    public class DeleteUserModel
    {

        public string username { get; set; }
        public string key { get; set; }
        public bool AssignIssues { get; set; }
        public string UserToAssign { get; set; }
        public string OrganizationToAssign { get; set; }

        public string DisplayName { get; set; }
    }
}