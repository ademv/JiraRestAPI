using JiraRestAPI.Models.Users;
using JiraRestAPI.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;

namespace JiraRestAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        JiraService jiraservice = new JiraService();
        DataService dataservice = new DataService();

        [HttpDelete]
        public IHttpActionResult DeleteUser(DeleteUserModel model)
        {
            if (model == null || (model.key == "" && model.username == ""))
            {
                return BadRequest(" Key ose name nuk duhet te jene bosh !");
            }



            if (dataservice.DeleteUser(model.key))
            {

                var listuser = new List<string>();
                listuser.Add(model.key);
                if (!model.AssignIssues)
                {
                    if (jiraservice.DeleteIssuesOfUser(model.key) == HttpStatusCode.OK && jiraservice.DeleteUser(model) == HttpStatusCode.NoContent)
                    {

                        return Ok("Perdoruesi dhe issues u fshine me sukses !");
                    }
                    else
                    {
                        dataservice.UpdateUserSync(listuser, 4, model.AssignIssues, model.UserToAssign,"-1");
                        return InternalServerError(new Exception("Issues nuk u fshine me sukes !"));
                    }
                }
                else
                {
                    if (jiraservice.ChangeReporterOfIssues(model.key, model.UserToAssign) == HttpStatusCode.OK && jiraservice.DeleteUser(model) == HttpStatusCode.NoContent)
                    {
                        return Ok("Perdoruesi u fshi dhe issue kaluan ne userin me key" + model.UserToAssign);
                    }
                    else
                    {
                        dataservice.UpdateUserSync(listuser, 4, model.AssignIssues, model.UserToAssign,"-1");
                        return InternalServerError(new Exception("Perdoruesi nuk u fshi dhe Issues nuk u asenjuan me sukses tek user me key " + model.UserToAssign + " " + " Reference : https://docs.atlassian.com/software/jira/docs/api/REST/7.7.1/?_ga=2.221431000.1781798227.1532332541-753206509.1523518640#api/2/user-removeUser"));
                    }
                }
            }
            else
            {
                return InternalServerError(new Exception("Useri nuk i fshi ne Ims:"));
            }
        }

        [HttpDelete]
        public IHttpActionResult DeleteUsers(List<DeleteUserModel> model)
        {

            if (model==null||model.Count == 0)
            {
                return BadRequest("Lista nuk mund te jete bosh");
            }
            List<DeleteUserModelResponse> l = new List<DeleteUserModelResponse>();
            foreach (var item in model)
            {

               var res= DeleteUser(item);
                bool isdeleted = false;
                if (res is OkResult)
                {
                    isdeleted = true;
                }
              
                var response = new DeleteUserModelResponse
                {
                    key = item.key,
                    result = isdeleted
                };
             
                l.Add(response);
                
            }

            return Ok(l);
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
