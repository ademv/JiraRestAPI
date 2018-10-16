using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace JiraRestAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "JiraVersionUser",
                routeTemplate: "api/jira/users/{action}/{id}",
                defaults: new { id = RouteParameter.Optional,controller="User" }
            );
            config.Routes.MapHttpRoute(
               name: "JiraVersionOrganization",
               routeTemplate: "api/jira/organizations/{action}/{id}",
               defaults: new { id = RouteParameter.Optional, controller = "Organization" }
           );
            config.Routes.MapHttpRoute(
             name: "JiraVersionIssues",
             routeTemplate: "api/jira/issue/{action}/{id}",
             defaults: new { id = RouteParameter.Optional, controller = "Issue" }
         );
            config.Routes.MapHttpRoute(
             name: "LogicalVersionUser",
             routeTemplate: "api/logical/users/{action}/{id}",
             defaults: new { id = RouteParameter.Optional, controller = "UserLogical" }
         );


            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
