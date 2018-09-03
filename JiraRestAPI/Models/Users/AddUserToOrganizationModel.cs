using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models.Users
{
    public class UserToOrganizationModel
    {
        public string OrganizationId { get; set; }
        public IList<string> usernames { get; set; }
    }
}