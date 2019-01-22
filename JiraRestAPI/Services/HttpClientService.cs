using JiraRestAPI.Models;
using JiraRestAPI.Models.Organization;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace JiraRestAPI.Services
{
    public class HttpClientService
    {

        public HttpClient httpclient;

        public HttpClientService(string Username, string Password, string BaseUri)
        {
            httpclient = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes(Username + ":" + Password);
            httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpclient.Timeout = TimeSpan.FromMinutes(5);
            httpclient.BaseAddress = new Uri(BaseUri);
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public object Get<T>(string action)
        {

            try
            {
                var response = httpclient.GetAsync(action);

                response.Result.EnsureSuccessStatusCode();
                return response.Result.Content.ReadAsAsync<T>().Result;

            }

            catch (Exception)
            {
                return null;
            }


        }


        public BaseResponseMessage Post(object o, string action)
        {
            try
            {
                var response = httpclient.PostAsJsonAsync(action, o).Result;

                if (response.IsSuccessStatusCode)
                {
                    return new BaseResponseMessage
                    {
                        status = true,
                        message = "Sukses !"
                    };

                }
                else
                {
                    var err = response.Content.ReadAsStringAsync().Result;

                    return new BaseResponseMessage
                    {
                        status = false,
                        message = err
                    };
                }

            }

            catch (Exception e)
            {
                return new BaseResponseMessage
                {
                    status = false,
                    message = e.Message
                };
            }
        }

        public Organization PostOrganization(object o, string action)
        {
            try
            {
                var response = httpclient.PostAsJsonAsync(action, o).Result;

                if (response.IsSuccessStatusCode)
                {
                    var objectresponse = response.Content.ReadAsAsync<Organization>().Result;
                    return objectresponse;
                }
                else
                {

                    return null;
                }


            }

            catch (Exception)
            {
                return null;
            }


        }

        public BaseResponseMessage Delete(object o, string action)
        {
            try
            {
                var response = httpclient.SendAsync(
                new HttpRequestMessage(HttpMethod.Delete, action)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json")
                })
                .Result;


                if (response.IsSuccessStatusCode)
                {
                    return new BaseResponseMessage
                    {
                        status = true,
                        message = "Sukses !"
                    };

                }
                else
                {
                    var err = response.Content.ReadAsStringAsync().Result;

                    return new BaseResponseMessage
                    {
                        status = false,
                        message = err
                    };
                }


            }

            catch (Exception e)
            {
                return new BaseResponseMessage
                {
                    status = false,
                    message = e.Message
                };
            }
        }

        public BaseResponseMessage  Put(object o, string action)
        {
            try
            {
                var response = httpclient.SendAsync(
                new HttpRequestMessage(HttpMethod.Put, action)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json")
                })
                .Result;

                if (response.IsSuccessStatusCode)
                {
                    return new BaseResponseMessage
                    {
                        status = true,
                        message = "Sukses !"
                    };

                }
                else
                {
                    var err = response.Content.ReadAsStringAsync().Result;

                    return new BaseResponseMessage
                    {
                        status = false,
                        message = err
                    };
                }
             


            }

            catch (Exception e)
            {
                return new BaseResponseMessage
                {
                    status = false,
                    message = e.Message
                };
            }
        }
    }
}