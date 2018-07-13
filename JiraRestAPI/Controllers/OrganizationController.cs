using JiraRestAPI.Models;
using JiraRestAPI.Models.Users;
using JiraRestAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JiraRestAPI.Controllers
{
    public class OrganizationController : ApiController
    {
        JiraService jiraservice = new JiraService();

        public IHttpActionResult GetAllUsers()
        {
            var response = jiraservice.GetAllUsers();

            if (response == null)
            {
                return InternalServerError();
            }
            return Ok(response);
        }

        public IHttpActionResult GetTest()
        {

            var list = jiraservice.GetUnconectedUser();
            var listname = list.Select(x => new {x.emailAddress }).ToList();
            return Ok(listname);
        }



    }
}
