using JiraRestAPI.Models;
using JiraRestAPI.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JiraRestAPI.Services
{
    public class HttpClientService
    {

        public HttpClient httpclient;

        public HttpClientService(string Username,string Password,string BaseUri)
        {
            httpclient = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes(Username+":"+Password);
            httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpclient.Timeout = TimeSpan.FromMinutes(5);
            httpclient.BaseAddress = new Uri(BaseUri);
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public object Get<T>(string action)
        {

            try
            {
                var response =  httpclient.GetAsync(action);

                response.Result.EnsureSuccessStatusCode();
                return response.Result.Content.ReadAsAsync<T>().Result;

            }

            catch (Exception ex)
            {
                return null;
            }


        }
    }
}