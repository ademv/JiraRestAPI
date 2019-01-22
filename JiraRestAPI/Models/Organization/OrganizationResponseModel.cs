using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models.Organization
{
    public class OrganizationResponseModel
    {
        public OrganizationResponseModel()
        {
            Messages = new List<BaseResponseMessage>();
    }

        public string UserKey { get; set; }
        public  string DisplayName { get; set; }
        public bool Status { get; set; }
        public List<BaseResponseMessage> Messages { get; set; }

        public void CalculateStatus()
        {

            var haserror = Messages.Where(y => y.status == false).Any();

            Status = !haserror;
        }
    }
}