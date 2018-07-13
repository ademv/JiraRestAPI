using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models
{
    public class BaseResponseMessage
    {

        public bool status { get; set; }
        public  string message { get; set; }
    }
}