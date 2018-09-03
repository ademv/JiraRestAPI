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
    public class UserController : ApiController
    {
        JiraService jiraservice = new JiraService();

        [HttpDelete]
        public IHttpActionResult DeleteUser(UserModel model)
        {
            if (model == null || (model.key == "" && model.name == ""))
            {
                return BadRequest(" Key ose name nuk duhet te jene bosh !");
            }

            var response = jiraservice.DeleteUser(model);
            if (response == HttpStatusCode.NoContent)
            {
                return Ok("Perdoruesi u fshi me sukses !");
            }

            return InternalServerError((new Exception("Statusi nga Jira:" + response.ToString() + "  Reference : https://docs.atlassian.com/software/jira/docs/api/REST/7.7.1/?_ga=2.221431000.1781798227.1532332541-753206509.1523518640#api/2/user-removeUser")));
        }



        /// <summary>
        /// Kthen te gjithe perdoruesit ne jira
        /// </summary>
        /// <returns>json text</returns>
        public IHttpActionResult GetAllUsers()
        {
            var response = jiraservice.GetAllUsers();

            if (response == null)
            {
                return InternalServerError();
            }
            return Ok(response);
        }

        /// <summary>
        /// Kthen te gjithe perdoruesit te cilet nuk jane ne ndonje organizate dhe nuk jane pjese  logical
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetUnConnectedUsers()
        {
            var list = jiraservice.GetUnconectedUser();

            return Ok(list);
        }
        
       
    }
}
