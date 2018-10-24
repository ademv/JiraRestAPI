using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models.Issue
{
    public class IssueModel
    {
        

    }

    public class IssueField
    {
        public string expand { get; set; }
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public IList<Issue> issues { get; set; }
    }


}