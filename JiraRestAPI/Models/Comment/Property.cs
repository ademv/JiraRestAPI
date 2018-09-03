using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models.Comment
{
    public class Property
    {
        public string key { get; set; }
        public Value value { get; set; }
    }
}