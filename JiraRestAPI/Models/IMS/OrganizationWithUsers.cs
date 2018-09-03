using JiraRestAPI.Models;
using JiraRestAPI.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models.IMS
{
    public class OrganizationWithUsers
    {

        public Organization.Organization organization;

        public List<UserModel> users;
    }
}