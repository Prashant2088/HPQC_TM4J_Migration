using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ALM_EXTRACT.JiraAPI
{
    public class Project
    {
        private Jira _jira { get; set; }
        public Jira GetJira()
        {
            return _jira;
        }

        public void SetJira(Jira jira)
        {
            _jira = jira;
        }


        public Int32 ID { get; set; }
        public String Key { get; set; }
        public String Name { get; set; }

        private User _lead;

        public User Lead { get; set; }

        [JsonIgnore]
        public User ProjectLead
        {
            get { return _lead ?? (_lead = _jira.Client.GetUser(Lead.Username)); }
        }

        private List<User> _assignableUsers;
        [JsonIgnore]
        public List<User> AssignableUsers
        {
            get
            {
                return _assignableUsers ??
                       (_assignableUsers =
                            _jira.Client.GetAssignableUsers(this.Key));
            }
        }

      


        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj is Project) && this.Key.Equals(((Project)obj).Key);
        }
    }
}