using JiraRestAPI.Models;
using JiraRestAPI.Models.Organization;
using JiraRestAPI.Models.Users;
using JiraRestAPI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

namespace JiraRestAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        JiraService jiraservice = new JiraService();
        DataService dataservice = new DataService();


        public BaseResponseMessage DeleteUser(DeleteUserModel model)
        {


            var listuser = new List<string>();
            listuser.Add(model.key);
            if (!model.AssignIssues)
            {

                var deleteIssues = jiraservice.DeleteIssuesOfUser(model.key);

                if (deleteIssues == HttpStatusCode.OK)
                {
                    var deleteUser = jiraservice.DeleteUser(model);
                    if (deleteUser.status)
                    {
                        dataservice.DeleteUser(model.key);
                    }


                    return deleteUser;
                }
                else
                {

                    return new BaseResponseMessage { message = "Nuk u fshin  ceshtjet e userit :" + model.UserToAssign };
                }


            }



            else
            {
                var changeIssues = jiraservice.ChangeReporterOfIssues(model.key, model.UserToAssign, model.OrganizationToAssign);

                if (changeIssues == HttpStatusCode.OK)
                {
                    var deleteuser = jiraservice.DeleteUser(model);
                    if (deleteuser.status)
                    {
                        dataservice.DeleteUser(model.key);
                    }

                    return deleteuser;
                }
                else
                {

                    return new BaseResponseMessage { message = "Nuk u asnjuan ceshtjet te useri :" + model.UserToAssign };
                }


            }

        }

        [HttpDelete]
        public IHttpActionResult DeleteUsers(List<DeleteUserModel> model)
        {

            if (model == null || model.Count == 0)
            {
                return BadRequest("Lista nuk mund te jete bosh");
            }

            List<OrganizationResponseModel> LISTMESSAGES = new List<OrganizationResponseModel>();
            foreach (var item in model)
            {
                var res = new OrganizationResponseModel { UserKey = item.username,DisplayName=item.DisplayName };
                res.Messages.Add(DeleteUser(item));
                res.CalculateStatus();
                LISTMESSAGES.Add(res);
            }

            return Ok(PrepareFinalResult(LISTMESSAGES));


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

        private TransactionModel PrepareFinalResult(List<OrganizationResponseModel> list)
        {
            var allrecordok = list.Where(x => x.Messages.Where(y => y.status == false).Any() == true).Any();

            return new TransactionModel { Status = !allrecordok, Data = list };
        }

    }
}
