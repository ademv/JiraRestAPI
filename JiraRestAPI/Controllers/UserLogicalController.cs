using JiraRestAPI.Models.IMS;
using JiraRestAPI.Models.Users;
using JiraRestAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace JiraRestAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserLogicalController : ApiController
    {
        DataService data = new DataService();



        /// <summary>
        /// Kthen te gjithe perdoruesit ne jira
        /// </summary>
        /// <returns>json text</returns>
        /// 
        [HttpPost]
        public IHttpActionResult GetAllUsers(Filter model)
        {

            try
            {
                List<UserModel> user = data.GetLocalUsers(model);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }



        }

        /// <summary>
        /// Kthen te gjithe perdoruesit te cilet nuk jane ne ndonje organizate dhe nuk jane pjese  logical
        /// </summary>
        /// <returns></returns>
        
            [HttpPost]
        public IHttpActionResult GetUnConnectedUsers(Filter model)
        {

            try
            {
                List<UserModel> user = data.GetUnConnectedLocalUsers(model);

                return Ok(user);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]

        public IHttpActionResult GetTotalUsers()
        {
            var nr = data.GetTotalALLUsers();
            if (nr == null)
            {
                return InternalServerError();
            }
            return Ok(nr);
        }
        [HttpGet]
        public IHttpActionResult GetTotalUnConnectedUsers()
        {
            var nr = data.GetTotalUnConnectedUsers();
            if (nr == null)
            {
                return InternalServerError();
            }
            return Ok(nr);
        }
        [HttpPost]
        public IHttpActionResult GetConnectedUsers(Filter filter)
        {
            var nr = data.GetConnectedUsers(filter);
            if (nr == null)
            {
                return InternalServerError();
            }
            return Ok(nr);
        }
        public IHttpActionResult GetTotalConnectedUsers()
        {
            var nr = data.GetTotalConnectedUsers();
            if (nr == null)
            {
                return InternalServerError();
            }
            return Ok(nr);
        }

    }
}
