using JiraRestAPI.Models;
using JiraRestAPI.Models.Comment;
using JiraRestAPI.Models.IMS;
using JiraRestAPI.Models.Issue;
using JiraRestAPI.Models.Organization;
using JiraRestAPI.Models.Users;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;

namespace JiraRestAPI.Services
{
    public class JiraService
    {
        private HttpClientService httpservice;

        private DataService dataserviceinside = new DataService();

        public JiraService()
        {

            var config = ConfigurationManager.AppSettings;

            httpservice = new HttpClientService(config["username"], config["password"], config["url"]);
        }


        //organization controller
        public object GetAllUsers()
        {
            var response = httpservice.Get<List<UserModel>>("api/latest/user/search?startAt=0&maxResults=5000&username=%");


            return response;
        }

        public List<UserModel> GetUnconectedUser()
        {
            //get all user


            List<UserModel> alluser = null;

            Thread th = new Thread(() =>
              {
                  alluser = (List<UserModel>)GetAllUsers();
              });



            //get users in organizations
            List<UserModel> ousers = null;
            Thread th2 = new Thread(() =>
            {
                ousers = GetUsersAllOrganizations();
            });

            th2.Start();
            th.Start();
            th.Join();
            th2.Join();
            var result = alluser.Where(x => !ousers.Any(y => y.key == x.key) && !x.emailAddress.Contains("atlassian.com") && !x.emailAddress.Contains("@example.com")).ToList();

            return result;
        }



        public List<Organization> GetOrganizations()
        {

            OranizationsBag pageorganization;
            List<Organization> allorganization = new List<Organization>();

            var nextlink = "servicedeskapi/organization/";
            do
            {
                pageorganization = (OranizationsBag)httpservice.Get<OranizationsBag>(nextlink);

                if (pageorganization._links.next != null)
                    nextlink = pageorganization._links.next.Substring(httpservice.httpclient.BaseAddress.OriginalString.Count());

                allorganization.AddRange(pageorganization.values);

            }
            while (!pageorganization.isLastPage);

            return allorganization;

        }

        public Organization GetOrganization(string Id)
        {
            return (Organization)httpservice.Get<Organization>("servicedeskapi/organization/" + Id);
        }

        public List<UserModel> GetUsersOfOrgaization(Organization o)
        {


            UserBag pageusers;
            List<UserModel> allusers = new List<UserModel>();

            var nextlink = "servicedeskapi/organization/" + o.id + "/user";
            do
            {
                pageusers = (UserBag)httpservice.Get<UserBag>(nextlink);

                if (pageusers._links.next != null)
                    nextlink = pageusers._links.next.Substring(httpservice.httpclient.BaseAddress.OriginalString.Count());

                allusers.AddRange(pageusers.values);

            }
            while (!pageusers.isLastPage);

            return allusers;


        }


        public List<UserModel> GetUsersAllOrganizations()
        {
            var allusers = new List<UserModel>();
            foreach (var item in GetOrganizations())
            {
                allusers.AddRange(GetUsersOfOrgaization(item));
            }
            return allusers;

        }

        public List<UserModel> GetUsersInOrganization(string Id)
        {
            return GetUsersOfOrgaization(new Organization { id = Id });
        }

        public List<OrganizationWithUsers> GetOrganizationsWithUsers()
        {
            List<OrganizationWithUsers> orgusers = new List<OrganizationWithUsers>();
            foreach (var item in GetOrganizations())
            {
                var ou = new OrganizationWithUsers();
                ou.organization = item;
                ou.users = GetUsersOfOrgaization(item);
                orgusers.Add(ou);
            }

            return orgusers;
        }

        public IssuesSearch GetIssuesByJQL(string jql)
        {
            var obj = httpservice.Get<IssuesSearch>("api/2/search?jql=" + jql.Replace("&","%26").Replace("+", "%2B").ToString()) as IssuesSearch;

            return obj;
        }

        public void AddComentsIntoIssues(IssuesSearch model, string comment, int customField_10075)
        {

            foreach (var item in model.issues)
            {       //add comment and update datetime of issue
                AddCommentToIssue(item.key, new CommentModel { body = comment });
                IssueLastUpdateField fields = new IssueLastUpdateField();
                fields.fields.customfield_10075 = customField_10075;
                var response = httpservice.Put(fields, "api/2/issue/" + item.key);
            }
        }

        public List<OrganizationResponseModel> AddUserToOrganization(List<UserToOrganizationModel> model)
        {
            List<OrganizationResponseModel> LISTMESSAGES = new List<OrganizationResponseModel>();
          
            var adduser = httpservice.Post(new { usernames = model.Select(x => x.username).ToList() }, "servicedeskapi/organization/" + model.Select(x => x.OrganizationId).First() + "/user");
            //per cdo username bejme update organization id te issue nese vendosja ne organizae eshte me sukses
            if (adduser.status)
            {

                foreach (var user in model)
                {
                    var issueRes = new OrganizationResponseModel { UserKey = user.username,DisplayName=user.DisplayName };
                    issueRes.Messages.Add(new BaseResponseMessage { status = true, message = "Useri u shtua me sukses ne organizate(if have lines below not all issues are assiged correctly into organization) !" });
                    //issue of Users update organizationId
                    if (user.IssueUpdate)
                    {
                        var issues = GetIssuesOfUsers(user.username);

                        foreach (var item in issues.issues)
                        {
                            var req = new IssueOrganizationFiled();
                            req.fields.customfield_10004.Add(Convert.ToInt32(user.OrganizationId));
                            var IssueUpdate = httpservice.Put(req, "api/2/issue/" + item.key);
                            if (!IssueUpdate.status)
                            {
                                issueRes.Messages.Add(IssueUpdate);
                                //rollback 
                                dataserviceinside.UpdateUserOrganization(new List<string>() { user.username }, "-1");
                            }


                        }
                    }

                    issueRes.CalculateStatus();
                    LISTMESSAGES.Add(issueRes);

                    //add comments in company
                    var commentdata = DataService.GetCompanyComments().Where(x => x.OrganizationId == user.OrganizationId.ToString()).FirstOrDefault();

                    if (commentdata != null)
                    {
                        var result = GetIssuesByJQL(commentdata.JQL);
                        AddComentsIntoIssues(result, commentdata.Comment, commentdata.customField_10075);
                    }


                }

                return LISTMESSAGES;


            }
            else
            {

                foreach (var item in model)
                {

                
                    LISTMESSAGES.Add(new OrganizationResponseModel
                    {
                        UserKey = item.username,
                        Messages = new List<Models.BaseResponseMessage> {
                        new Models.BaseResponseMessage { message = adduser.message } }
                    });
                }
                return LISTMESSAGES;
            }


        }
        public List<OrganizationResponseModel> ChangeUserOrganization(List<UserToOrganizationModel> model)
        {
            List<OrganizationResponseModel> LISTMESSAGES = new List<OrganizationResponseModel>();

            var   adduser = httpservice.Post(new { usernames = model.Select(x => x.username).ToList() }, "servicedeskapi/organization/" + model.Select(x => x.OrganizationId).First().ToString() + "/user");
          var   deleteuser = httpservice.Delete(new { usernames = model.Select(x => x.username).ToList() }, "servicedeskapi/organization/" + model.Select(x => x.CurrentOgranizationId).First() + "/user");



            if (adduser.status&&deleteuser.status)
            {
                foreach (var user in model)
                {
                    var issueRes = new OrganizationResponseModel { UserKey = user.username,DisplayName=user.DisplayName };
                    issueRes.Messages.Add(new BaseResponseMessage { status = true, message = "User(i)/at ndrysh(oi)/uan  me sukses ne organizate (if have lines below not all issues are assiged correctly into organization) !" });
                    if (user.IssueUpdate)
                    {
                        var issues = GetIssuesOfUsers(user.username);
                 
                        foreach (var item in issues.issues)
                        {
                            var req = new IssueOrganizationFiled();
                            req.fields.customfield_10004.Add(Convert.ToInt32(user.OrganizationId));
                            var response = httpservice.Put(req, "api/2/issue/" + item.key);
                            if (!response.status)
                            {
                                issueRes.Messages.Add(response);
                                //rollback 
                                dataserviceinside.UpdateUserOrganization(new List<string>() { user.username }, user.CurrentOgranizationId);
                            }

                        }

                        issueRes.CalculateStatus();
                        LISTMESSAGES.Add(issueRes);
                        var commentdata = DataService.GetCompanyComments().Where(x => x.OrganizationId == user.OrganizationId.ToString()).FirstOrDefault();

                        if (commentdata != null)
                        {
                            var result = GetIssuesByJQL(commentdata.JQL);
                            AddComentsIntoIssues(result, commentdata.Comment, commentdata.customField_10075);
                        }


                    }

                }


                return LISTMESSAGES;
            }
            else
            {
                foreach (var item in model)
                {
                    LISTMESSAGES.Add(new OrganizationResponseModel
                    {
                        UserKey = item.username,
                        Messages = new List<Models.BaseResponseMessage> {
                        new Models.BaseResponseMessage { message = adduser.message+":"+deleteuser.message } }
                    });
                }
                return LISTMESSAGES;
            }

        }



        public IssuesSearch GetIssuesOfUsers(string userid)
        {
            return httpservice.Get<IssuesSearch>("api/2/search?jql=project = 'SVD' and reporter=" + userid) as IssuesSearch;
        }

        public List<OrganizationResponseModel> RemoveUsersFromOrganization(List<UserToOrganizationModel> model)
        {
            List<OrganizationResponseModel> list = new List<OrganizationResponseModel>();
            
            var groupedusernames = model.GroupBy(x => x.OrganizationId).Select(x => new Tuple<string, List<string>>(x.Key, x.Select(y => y.username).ToList()));

            foreach (var item in groupedusernames)
            {
              var deleteusers=  httpservice.Delete(new { usernames = item.Item2 }, "servicedeskapi/organization/" + item.Item2 + "/user");

                foreach (var u in item.Item2)
                {
                    list.Add(new OrganizationResponseModel { UserKey = u, Messages = new List<BaseResponseMessage> { new BaseResponseMessage { status = true, message = "Sukses" } } });
                }

            }

            return list;
        }


        public BaseResponseMessage AddCommentToIssue(string issueKey, CommentModel comment)
        {

            return httpservice.Post(comment, "api/2/issue/" + issueKey + "/comment");

        }

        public HttpStatusCode CreateOrganization(Organization OrganizationName)
        {
            var res1 = httpservice.PostOrganization(new { name = OrganizationName.name }, "servicedeskapi/organization");

            if (res1 == null)
            {
                return HttpStatusCode.InternalServerError;
            }

            var res2 = httpservice.Post(new { organizationId = res1.id }, "servicedeskapi/servicedesk/SVD/organization");

            if (res2.status)
            {
                dataserviceinside.UpdateJiraOrganizationId(OrganizationName.id.ToString(), res1.name, res1.id);
                return HttpStatusCode.NoContent;
            }
            return HttpStatusCode.InternalServerError;
        }

        public BaseResponseMessage DeleteOrganization(Organization model)
        {
            return httpservice.Delete(new object(), "servicedeskapi/organization/" + model.id);
        }


        //user controller

        public BaseResponseMessage DeleteUser(DeleteUserModel model)
        {
            return httpservice.Delete(new object(), "api/2/user?query=key=" + model.key + "&username=" + model.username);
        }

        public HttpStatusCode DeleteIssuesOfUser(string key)
        {
            var issuesBag = GetIssuesOfUsers(key);
            if (issuesBag == null)
            {
                return HttpStatusCode.InternalServerError;
            }
            //numri qe mban sa rekorde jane fshire ne rregull, nese nuk fshihen te gjitha rekordet nuk quhet fshirje e sukseshme
            int nr = 0;
            foreach (var item in issuesBag.issues)
            {
                var response = httpservice.Delete(new object(), "api/2/issue/" + item.key + "?query=deleteSubtasks=true");
                if (response.status)
                {
                    nr++;
                }
            }

            if (nr == issuesBag.issues.Count)
            {
                return HttpStatusCode.OK;
            }
            else
            {
                return HttpStatusCode.InternalServerError;
            }


        }


        public HttpStatusCode ChangeReporterOfIssues(string currentuser, string nextuser,string nextorganizationId)
        {
            var issuesBag = GetIssuesOfUsers(currentuser);
            if (issuesBag == null)
            {
                return HttpStatusCode.InternalServerError;
            }
            //numri qe mban sa rekorde jane update-uar ne rregull, nese nuk update-ohen te gjitha rekordet nuk quhet update i sukseshem
            int nr = 0;

            //change reporter and organizationId for issues
            ReporterBodyRequest reportedupdate = new ReporterBodyRequest(nextuser);
            reportedupdate.fields.customfield_10004.Add(Convert.ToInt32(nextorganizationId));
            foreach (var item in issuesBag.issues)
            {
                var response = httpservice.Put(reportedupdate, "api/2/issue/" + item.key);

                if (response.status)
                {
                    nr++;
                }
            }

            if (nr == issuesBag.issues.Count)
            {
                return HttpStatusCode.OK;
            }
            else
            {
                return HttpStatusCode.InternalServerError;
            }

        }

    }

}