using JiraRestAPI.Models;
using JiraRestAPI.Models.Organization;
using JiraRestAPI.Models.Users;
using JiraRestAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public IHttpActionResult AddUserToOrganization(List<UserToOrganizationModel> model)
        {

            if (model == null || model.Count == 0)
            {
                BadRequest();
            }

            bool IMSInsert = dataservise.UpdateUserOrganization(model.Select(x => x.username).ToList(), model.Select(x => x.OrganizationId).First());

            if (IMSInsert)
            {



                var response = jiraservice.AddUserToOrganization(model);


                return Ok(PrepareFinalResult(response));
            }
            else
            {
                return InternalServerError(new Exception("Perdoruesi nuk u shtua ne organizate,  Ims level!"));

            }
        }


        public IHttpActionResult ChangeUserOrganization(List<UserToOrganizationModel> model)
        {
            if (model == null)
            {
                BadRequest();
            }



            bool IMSInsert = dataservise.UpdateUserOrganization(model.Select(x => x.username).ToList(), model.Select(x => x.OrganizationId).First());

            if (IMSInsert)
            {
                var response = jiraservice.ChangeUserOrganization(model);
                return Ok(PrepareFinalResult(response));
            }
            else
            {

                return InternalServerError(new Exception("Perdoruesi/t nuk u shtua/n ne organizate,  Ims level !"));
            }
        }
        /// <summary>
        /// Heq perdoruesin nga Organizata
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult RemoveUsersFromOrganization(List<UserToOrganizationModel> model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var isdeleted = dataservise.UpdateUserOrganization(model.Select(x => x.username).ToList(), "-1");
            if (isdeleted)
            {
                var response = jiraservice.RemoveUsersFromOrganization(model);               
                return Ok(PrepareFinalResult(response));
            }
            else
            {

                return InternalServerError(new Exception("Perdoruesi/t nuk u hoq/en nga orginazata,IMS level"));
            }
        }




        /// <summary>
        /// Krijon nje organizate te re
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CreateOrganization(Organization model)
        {
            if (model == null || model.name == null || model.name == "")
            {
                return BadRequest();
            }

            var response = jiraservice.CreateOrganization(model);

            if (response == HttpStatusCode.NoContent)
            {


                return Ok("Organizata u krijua me sukses !");
            }

            return InternalServerError(new Exception("Statusi nga Jira:" + response.ToString() + "  Reference :https://developer.atlassian.com/cloud/jira/service-desk/rest/#api-organization-post"));
        }

        [HttpDelete]
        public IHttpActionResult DeleteOrganization(Organization model)
        {

            if (model == null || model.id == "0")
            {
                return BadRequest();
            }

            var response = jiraservice.DeleteOrganization(model);


            if (response.status)
            {

                var logicalDelete = dataservise.DeleteOrganization(model.id);
            }
            return Ok(response.message);

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


        private TransactionModel PrepareFinalResult(List<OrganizationResponseModel> list)
        {
            var allrecordok = list.Where(x => x.Messages.Where(y => y.status == false).Any() == true).Any();

            return new TransactionModel { Status = !allrecordok, Data = list };
        }
       
    }
}
