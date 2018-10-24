using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace JiraRestAPI.Models.Users
{
    public class DeleteUserModelResponse
    {

        public string key { get; set; }
        public bool result{get;set;}
    }
}