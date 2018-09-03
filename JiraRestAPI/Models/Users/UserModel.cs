using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models.Users
{
    
    public class UserModel
    {
        public string Id { get; set; }
        public string  OrganizationID { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public string accountId { get; set; }
        public string name { get; set; }
        public string emailAddress { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
        public string locale { get; set; }
    }
}