using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace ALM_EXTRACT.JiraAPI
{
    public class SearchTest
    {
        public SearchTestFields Fields { get; set; }

        public SearchTest(JObject fields)
        {
            this.Fields = new SearchTestFields(fields);
        }
    }

    public class SearchTestFields
    {
        /*Fetching only required feilds . You can fetch more fields EX: in postman : http://hostnameofJira/rest/atm/1.0/testcase/search?query=projectKey = 
        "UCA" AND name = "4.1 Run with no arguments" AND folder = "/Subject/Reporting/Post Installation Batch Scripts/ManageDataSources" */

        public string ProjectKey { get; set; }
        public string Status { get; set; }
        public string TestName { get; set; }
        public string Folder { get; set; }

        public SearchTestFields() { }
        public SearchTestFields(JObject fieldsObj)
        {
            Dictionary<String, Object> fields = fieldsObj.ToObject<Dictionary<String, Object>>();

            ProjectKey = "";
            if (fields.ContainsKey("key") && fields["key"] != null)
            {
                ProjectKey = (String)fields["key"];
            }

            Status = "";
            if (fields.ContainsKey("status") && fields["status"] != null)
            {
                Status = (String)fields["status"];
            }

            TestName = "";
            if (fields.ContainsKey("name") && fields["name"] != null)
            {
                TestName = (String)fields["name"];
            }

            Folder = "";
            if (fields.ContainsKey("folder") && fields["folder"] != null)
            {
                Folder = (String)fields["folder"];
            }
        }
    }
}