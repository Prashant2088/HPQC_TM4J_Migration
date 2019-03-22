using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALM_EXTRACT.JiraAPI
{
    public interface IJiraClient
    {
        string GetBaseUrl();
        Folder CreateFolder(CreateFolder newFolder);
        TestCase CreateTestCase(CreateTestCase newTestCase);
        bool UpdateTestCase(CreateTestCase newTestCase, string strTestCaseKey);
        List<Field> GetFields();
        List<Project> GetProjects();
        User GetUser(String username);
        List<User> GetAssignableUsers(String projectKey);
        bool UploadAttachmentToTest(string strTestCaseKey, string strFilePath, string strFileName);
        bool UploadDesignStepsAttachmentToTest(string strTestCaseKey, string strStepIndex, string strFilePath, string strFileName);
        bool SearchTest(String jql);
        List<string> FindDuplicateJiraTestKeys(String jql);
        bool DeleteJiraTestId(string strTestCaseKey);
    }
}
