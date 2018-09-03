using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Models.Comment
{
    public class CommentModel
    {

        public string body { get; set; }
        public IList<Property> properties { get; set; }

        public CommentModel()
        {
            properties = new List<Property>();
            Property p = new Property { value = new Value { @internal = "True" },key= "sd.public.comment" };
            
            properties.Add(p);
        }
    }
}