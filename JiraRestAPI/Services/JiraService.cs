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


            List<UserModel> alluser=null;

                Thread th = new Thread(() =>
                  {
                      alluser= (List<UserModel>)GetAllUsers();
                  });



            //get users in organizations
            List<UserModel> ousers = null;
            Thread th2 = new Thread(() =>
            {
               ousers= GetUsersAllOrganizations();
            });

            th2.Start();
            th.Start();
            th.Join();
            th2.Join();
            var result = alluser.Where(x => !ousers.Any(y => y.key == x.key)&&!x.emailAddress.Contains("@logic.al")).ToList();

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

                if(pageorganization._links.next!=null)
                nextlink = pageorganization._links.next.Substring(httpservice.httpclient.BaseAddress.OriginalString.Count());

                allorganization.AddRange(pageorganization.values);

            }
            while (!pageorganization.isLastPage);

            return allorganization;

        }

        public Organization GetOrganization(string Id)
        {
            return (Organization)httpservice.Get<Organization>("servicedeskapi/organization/"+Id);
        }

        public List<UserModel> GetUsersOfOrgaization(Organization o)
        {


            UserBag pageusers;
            List<UserModel> allusers = new List<UserModel>();

            var nextlink = "servicedeskapi/organization/"+o.id+"/user";
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
            var obj = httpservice.Get<IssuesSearch>("api/2/search?jql=" + jql) as IssuesSearch;

            return obj;
        }

        public void AddComentsIntoIssues(IssuesSearch model,string comment)
        {
            
            foreach (var item in model.issues)
            {
                AddCommentToIssue(item.key, new CommentModel { body = comment });
            }
        }

        public HttpStatusCode AddUserToOrganization(UserToOrganizationModel model)
        {
           return httpservice.Post(new { usernames=model.usernames }, "servicedeskapi/organization/"+model.OrganizationId+"/user");
           
        }

        public HttpStatusCode RemoveUsersFromOrganization(UserToOrganizationModel model)
        {
            return httpservice.Delete(new { usernames = model.usernames }, "servicedeskapi/organization/"+model.OrganizationId+"/user");
        }

        
        public HttpStatusCode AddCommentToIssue(string issueKey,CommentModel comment)
        {

            return httpservice.Post(comment, "api/2/issue/" + issueKey + "/comment");

        }

        public HttpStatusCode CreateOrganization(string OrganizationName)
        {
            var res1= httpservice.Post(new { name = OrganizationName }, "servicedeskapi/organization");
            var organization = GetOrganizations();
            string maxid = organization.Max(x => Convert.ToInt32(x.id)).ToString();
            var res2 = httpservice.Post(new { organizationId = maxid }, "servicedeskapi/servicedesk/SVD/organization");

            if (res1 == HttpStatusCode.Created && res2 == HttpStatusCode.NoContent)
            {
                return HttpStatusCode.NoContent;
            }
            return HttpStatusCode.InternalServerError;
        }

        public HttpStatusCode DeleteOrganization(Organization model)
        {
            return httpservice.Delete(new object(), "servicedeskapi/organization/"+model.id);
        }


        //user controller

        public HttpStatusCode DeleteUser(UserModel model)
        {


            return httpservice.Delete(new object(), "rest/api/2/user?query=username=" + model.name + "&key=" + model.key);
        }

    }

}