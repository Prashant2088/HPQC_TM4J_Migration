﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALM_EXTRACT.JiraAPI
{
    public class TestCase
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

        public string Key { get; set; }

        public string ErrorMessages { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }
}