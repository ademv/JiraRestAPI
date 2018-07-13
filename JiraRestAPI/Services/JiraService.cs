using JiraRestAPI.Models;
using JiraRestAPI.Models.Organization;
using JiraRestAPI.Models.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace JiraRestAPI.Services
{
    public class JiraService
    {
        private HttpClientService httpservice;

        public JiraService()
        {
            httpservice = new HttpClientService("j.hoxha@logic.al", "kTJCzj3HX9035fjfVXgY3AA5", "https://logicalalbania.atlassian.net/rest/");
        }


       public object GetAllUsers()
        {
           var response = httpservice.Get<List<UserModel>>("api/latest/user/search?startAt=0&maxResults=5000&username=%");

            
            return response;
        }

        public List<UserModel> GetUnconectedUser()
        {
            //get all user
            var alluser = (List<UserModel>)GetAllUsers();
            //get users in organizations
            var ousers = GetUsersAllOrganizations();

            var result = alluser.Where(x => !ousers.Any(y => y.key == x.key)&&!x.emailAddress.Contains("@logic.al")).ToList();

            return result;
        }


        private List<Organization> GetOrganizations()
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
        private List<UserModel> GetUsersOfOrgaization(Organization o)
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

     
    }

}