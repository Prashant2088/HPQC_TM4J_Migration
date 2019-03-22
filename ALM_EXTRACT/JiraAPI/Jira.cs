using System;
using System.Collections.Generic;

namespace ALM_EXTRACT.JiraAPI
{
    public class Jira : IDisposable
    {
        private IJiraClient _client;
        internal IJiraClient Client { get { return _client; } }

        public List<Field> Fields { get; private set; }

        public void Connect(IJiraClient client)
        {
            _client = client;
            Fields = _client.GetFields();
        }

        public void Connect(String url)
        {
            Connect(new JiraClient(url));
        }

        public void Connect(String url, String username, String password)
        {
            Connect(new JiraClient(url, username, password));
        }

        public Folder CreateFolder(CreateFolder newFolder)
        {
            Folder tstResponse = new Folder();
            try
            {
                tstResponse = _client.CreateFolder(newFolder);
                return tstResponse;
            }
            catch (Exception ex)
            {
                tstResponse.ErrorMessages = ex.Message;
                Utilities.LogException(ex);
                return tstResponse;
            }
        }
        public TestCase CreateTestCase(CreateTestCase newTestCase)
        {
            TestCase tstResponse = new TestCase();
            try
            {
                tstResponse = _client.CreateTestCase(newTestCase);
                return tstResponse;
            }
            catch (Exception ex)
            {
                tstResponse.ErrorMessages = ex.Message;
                Utilities.LogException(ex);
                return tstResponse;
            }
        }

        public bool UpdateTestCase(CreateTestCase newTestCase, string strTestCaseKey)
        {
            return _client.UpdateTestCase(newTestCase, strTestCaseKey);
        }

        public bool UploadAttachmentToTest(string strTestCaseKey, string strFilePath, string strFileName)
        {
            try
            {
                return _client.UploadAttachmentToTest(strTestCaseKey, strFilePath, strFileName); ;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                return false;
            }
        }

        public bool UploadDesignStepsAttachmentToTest(string strTestCaseKey, string strStepIndex, string strFilePath, string strFileName)
        {
            try
            {
                return _client.UploadDesignStepsAttachmentToTest(strTestCaseKey, strStepIndex, strFilePath, strFileName); ;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                return false;
            }
        }

        public List<Project> GetProjects()
        {
            List<Project> projects = _client.GetProjects();
            projects.ForEach(project => project.SetJira(this));
            return projects;
        }

        public bool SearchTest(String jql)
        {
            return _client.SearchTest(jql);
        }
        public List<string> FindDuplicateJiraTestKeys(String jql)
        {
            return _client.FindDuplicateJiraTestKeys(jql);
        }

        public bool DeleteJiraTestId(string strTestCaseKey)
        {
            return _client.DeleteJiraTestId(strTestCaseKey);
        }

        public User GetUser(String username)
        {
            return _client.GetUser(username);
        }

        public void Dispose()
        {

        }

        public void Dispose(bool v)
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
