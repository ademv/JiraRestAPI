using JiraRestAPI.Models;
using JiraRestAPI.Models.IMS;
using JiraRestAPI.Models.Organization;
using JiraRestAPI.Models.Users;
using JiraRestAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace JiraRestAPI.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OrganizationController : ApiController
    {
        JiraService jiraservice = new JiraService();
        DataService dataservise = new DataService();

        /// <summary>
        /// Shton perdoruesin ne nje organizate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult AddUserToOrganization(UserToOrganizationModel model)
        {

            if (model == null)
            {
                BadRequest();
            }
            var response = jiraservice.AddUserToOrganization(model);
            if (response == HttpStatusCode.NoContent)
            {
                var allupdated= dataservise.UpdateUserOrganization(model.usernames, model.OrganizationId);
                string mesg2;
                mesg2 = " Perdoruesit nuk u shtuan lokalisht!";
                if (allupdated)
                {
                    mesg2 = "Perdoruesit  u shtuan lokalisht!";
                }
                return Ok("Perdoruesi u shtua me sukses ne jira !"+" "+mesg2);
            }

            return InternalServerError(new Exception("Statusi nga Jira:" + response.ToString() + "  Reference : https://developer.atlassian.com/cloud/jira/service-desk/rest/#api-organization-organizationId-user-post"));
        }

        /// <summary>
        /// Heq perdoruesin nga Organizata
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult RemoveUsersFromOrganization(UserToOrganizationModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var response = jiraservice.RemoveUsersFromOrganization(model);

            if (response == HttpStatusCode.NoContent)
            {
                return Ok("Perdoruesi/t u hoqen nga organizata me sukses !");
            }

            return InternalServerError(new Exception("Statusi nga Jira:" + response.ToString() + "  Reference : https://developer.atlassian.com/cloud/jira/service-desk/rest/#api-organization-organizationId-user-delete"));
        }

       


        /// <summary>
        /// Krijon nje organizate te re
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CreateOrganization(Organization model)
        {
            if (model==null||model.name == null || model.name == "")
            {
                return BadRequest();
            }

            var response = jiraservice.CreateOrganization(model.name);

            if (response == HttpStatusCode.NoContent)
            {
                return Ok("Organizata u krijua me sukses !");
            }

            return InternalServerError(new Exception("Statusi nga Jira:" + response.ToString() + "  Reference :https://developer.atlassian.com/cloud/jira/service-desk/rest/#api-organization-post"));
        }

        [HttpDelete]
        public  IHttpActionResult DeleteOrganization(Organization model)
        {

            if (model == null)
            {
                return BadRequest();
            }

            var response = jiraservice.DeleteOrganization(model);

            if (response == HttpStatusCode.NoContent)
            {
                return Ok("Organizata u fshi me sukses");
            }

            return InternalServerError(new Exception("Statusi nga Jira:" + response.ToString() + "  Reference : https://developer.atlassian.com/cloud/jira/service-desk/rest/#api-organization-organizationId-delete"));

        }

        [HttpGet]
        public IHttpActionResult GetOrganizations()
        {
            var response = jiraservice.GetOrganizations();


            if (response == null)
            {
                return InternalServerError();
            }

            return Ok(response);
        }

        [HttpGet]
        public IHttpActionResult GetUsersInAllOrganizations()
        {
            var response = jiraservice.GetUsersAllOrganizations();
            if (response == null)
            {
                return InternalServerError();
            }


            return Ok(response);
        }

        [HttpGet]
        public IHttpActionResult GetUsersInOrganization(string Id)
        {
            var result = jiraservice.GetUsersInOrganization(Id);

            if (result == null)
            {
                return InternalServerError();
            }
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetOrganizationsWithUsers()
        {

            var response = jiraservice.GetOrganizationsWithUsers();
            if (response == null)
            {
                return InternalServerError();
            }

            return Ok(response);
        }

        public IHttpActionResult GetOrganization(string Id)
        {
            Organization response = jiraservice.GetOrganization(Id);

            if (response == null)
            {
                return InternalServerError(new Exception("Error ne server !"));
            }

            return Ok(response);
        }

    }
}
