using JiraRestAPI.Models.IMS;
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
    public class IssueController : ApiController
    {
        JiraService jiraservice = new JiraService();


        /// <summary>
        /// Shton nje komment tek nje issue te caktuar
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

            [HttpPost]
        public IHttpActionResult AddCommentToIssue(CommentIms model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var response = jiraservice.AddCommentToIssue(model.issue, new Models.Comment.CommentModel { body = model.body });

            if (response == HttpStatusCode.Created)
            {
                return Ok("Komenti u shtua me sukes");
            }


            return InternalServerError(new Exception("Statusi nga Jira:" + response.ToString() + "  Reference : https://developer.atlassian.com/cloud/jira/platform/rest/#api-api-2-issue-issueIdOrKey-comment-post"));
        }

        [HttpPost]
        public IHttpActionResult UpdateCompanyIssues(CompanyIssueComment model)
        {
            if (model == null)
            {
                return BadRequest();
            }
         
            var result = jiraservice.GetIssuesByJQL(model.JQL);

            if (result == null)
            {
                return InternalServerError(new Exception("Gabim ne marrjen e issue"));
            }

            jiraservice.AddComentsIntoIssues(result, model.Comment);

            return Ok("Issues u perditsuan  me sukes");

        }
    }
}
