using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models.Issue
{
    public class IssueOrganizationFiled
    {

        public IssueOrganizationFiled()
        {
            fields = new Fields();
            fields.customfield_10004 = new List<int>();
        }

        public Fields fields { get; set; }
    }

    public class Fields
    {
        public IList<int> customfield_10004 { get; set; }
    }

    public class IssueLastUpdateField
    {
        public CustomDateField fields;

        public IssueLastUpdateField()
        {
            fields = new CustomDateField();
        }
    }

    public class CustomDateField
    {

        public CustomDateField()
        {
            customfield_10074 = DateTime.Now.ToString("yyyy-MM-dd");
        }
        public string customfield_10074 { get; set; }
        public int customfield_10075 { get; set; }
    }



    public class ReporterBodyRequest
    {
        public ReportedField fields { get; set; }
        public ReporterBodyRequest(string name)
        {
            fields = new ReportedField(name);
        }
    }
    public class ReportedField
    {
        public ReportedField(string name)
        {
            reporter = new Reporter(name);
            customfield_10004 = new List<int>();
        }
        public Reporter reporter { get; set; }
        public IList<int> customfield_10004 { get; set; }
    }
    public class Reporter
    {
        public Reporter(string name)
        {
            this.name = name;
           
        }
        public string name { get; set; }
         public IList<int> customfield_10004 { get; set; }
    }

}