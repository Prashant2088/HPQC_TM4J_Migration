using RestSharp;
using System;
using System.Collections.Generic;
using RestSharp.Authenticators;
using ALM_EXTRACT.JiraAPI.Tools;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ALM_EXTRACT.JiraAPI
{
    public class JiraClient : IJiraClient
    {
        public enum JiraObjectEnum
        {
            Fields,
            Projects,
            Project,
            ProjectVersions,
            ProjectComponents,
            ProjectRoles,
            ProjectRole,
            ProjectCategories,
            ProjectTypes,
            TestCases,
            AssignableUser,
            User,
            UpdateTestCase,
            TestCaseAttachment,
            Folder,
            GetTestCase,
            DesignStepsAttachment,
            SearchTest,
            DeleteTest
        }

        private RestClient Client { get; set; }

        private const String JiraAPITestManagementURI = "/rest/atm/1.0";
        private const String JiraAPIServiceURI = "/rest/api/latest";

        private Dictionary<JiraObjectEnum, String> _methods = new Dictionary<JiraObjectEnum, String>()
        {
            {JiraObjectEnum.Fields, String.Format("{0}/field/", JiraAPIServiceURI)},
            {JiraObjectEnum.Projects, String.Format("{0}/project/", JiraAPIServiceURI)},
            {JiraObjectEnum.User, String.Format("{0}/user/", JiraAPIServiceURI)},
            {JiraObjectEnum.Project, String.Format("{0}/project/{{projectKey}}/", JiraAPIServiceURI)},
            {JiraObjectEnum.AssignableUser, String.Format("{0}/user/assignable/search/", JiraAPIServiceURI)},
            {JiraObjectEnum.ProjectVersions, String.Format("{0}/project/{{projectKey}}/versions/", JiraAPIServiceURI)},
            {JiraObjectEnum.ProjectComponents, String.Format("{0}/project/{{projectKey}}/components/", JiraAPIServiceURI)},
            {JiraObjectEnum.ProjectRoles, String.Format("{0}/project/{{projectKey}}/role/", JiraAPIServiceURI)},
            {JiraObjectEnum.ProjectRole, String.Format("{0}/project/{{projectKey}}/role/{{id}}", JiraAPIServiceURI)},
            {JiraObjectEnum.ProjectCategories, String.Format("{0}/projectCategory", JiraAPIServiceURI)},
            {JiraObjectEnum.ProjectTypes, String.Format("{0}/project/type", JiraAPIServiceURI)},
            {JiraObjectEnum.TestCases, String.Format("{0}/testcase", JiraAPITestManagementURI)},
            {JiraObjectEnum.TestCaseAttachment, String.Format("{0}/testcase/{{testCaseKey}}/attachments/", JiraAPITestManagementURI)},
            {JiraObjectEnum.UpdateTestCase, String.Format("{0}/testcase/{{testCaseKey}}/", JiraAPITestManagementURI)},
            {JiraObjectEnum.GetTestCase, String.Format("{0}/testcase/{{testCaseKey}}/", JiraAPITestManagementURI)},
            {JiraObjectEnum.Folder, String.Format("{0}/folder/", JiraAPITestManagementURI)},
            {JiraObjectEnum.DesignStepsAttachment, String.Format("{0}/testcase/{{testCaseKey}}/step/{{stepIndex}}/attachments/", JiraAPITestManagementURI)},
            {JiraObjectEnum.SearchTest, String.Format("{0}/testcase/search", JiraAPITestManagementURI)},
            {JiraObjectEnum.DeleteTest, String.Format("{0}/testcase/{{testCaseKey}}/", JiraAPITestManagementURI)},
        };

        public JiraClient(RestClient client)
        {
            Client = client;
        }

        public JiraClient(String url)
        {
            Client = new RestClient(url);
        }

        public JiraClient(String url, String username, String password)
        {
            Client = new RestClient(url)
            {
                Authenticator = new HttpBasicAuthenticator(username, password)
            };
        }

        public string GetBaseUrl()
        {
            return Client.BaseUrl.ToString();
        }


        #region TestCases & Projects Methods
        public List<Field> GetFields()
        {
            return GetList<Field>(JiraObjectEnum.Fields);
        }

        /// <summary>
        /// Get ALl the Jira Projects
        /// </summary>
        /// <returns></returns>
        public List<Project> GetProjects()
        {
            return GetList<Project>(JiraObjectEnum.Projects);
        }

        /// <summary>
        /// Get the Jira User by Username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetUser(String username)
        {
            return GetItem<User>(JiraObjectEnum.User, new Dictionary<string, string>() { { "username", username } });
        }
        public List<User> GetAssignableUsers(String projectKey)
        {
            return GetList<User>(JiraObjectEnum.AssignableUser,
                               parameters: new Dictionary<string, string>() { { "project", projectKey } });
        }

        /// <summary>
        /// Get the Test details by JQL Query for duplicacy test :Jenifer requirement
        /// </summary>
        /// <param name="jql"></param>
        /// <returns></returns>
        public List<string> FindDuplicateJiraTestKeys(String jql)
        {
            return GetDuplicateTestKeys(_methods[JiraObjectEnum.SearchTest], new Dictionary<String, String>() { { "query", jql } });
        }

        /// <summary>
        /// Get the Test details by JQL Query for duplicacy test :Jenifer requirement
        /// </summary>
        /// <param name="jql"></param>
        /// <returns></returns>
        public bool SearchTest(String jql)
        {
            return GetTestcase(_methods[JiraObjectEnum.SearchTest], new Dictionary<String, String>() { { "query", jql } });
        }

        /// <summary>
        /// Fetch the test cases
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        private bool GetTestcase(String url, Dictionary<String, String> parameters = null, Dictionary<String, String> keys = null)
        {
            IRestResponse response = Client.Execute(GetRequest(url, parameters ?? new Dictionary<String, String>(), keys ?? new Dictionary<String, String>()));

            if (response.ErrorException != null)
            {
                if (parameters.Count > 0)
                {
                    foreach (KeyValuePair<String, String> parameter in parameters)
                        Utilities.LogError(string.Format("Exception occured while searching for test in jira. Please delete the duplicate test manually in Jira (if any). Parameters: '{0}' - '{1}'", parameter.Key, parameter.Value));
                }
                else
                    Utilities.LogError("Exception occured while searching for test in jira. Please delete the duplicate test manually in Jira (if any)");

                return false;
            }
            else if (response.ResponseStatus != ResponseStatus.Completed)
            {
                if (parameters.Count > 0)
                {
                    foreach (KeyValuePair<String, String> parameter in parameters)
                        Utilities.LogError(string.Format("Response completed but unable to search test in jira. Please delete the duplicate test manually in Jira (if any). Parameters: '{0}' - '{1}'", parameter.Key, parameter.Value));
                }
                else
                    Utilities.LogError("Response completed but unable to search test in jira. Please delete the duplicate test manually in Jira (if any)");

                return false;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (parameters.Count > 0)
                {
                    foreach (KeyValuePair<String, String> parameter in parameters)
                        Utilities.LogError(string.Format("Failed to search test in jira due to bad request. Please delete the duplicate test manually in Jira (if any). Parameters: '{0}' - '{1}'", parameter.Key, parameter.Value));
                }
                else
                    Utilities.LogError("Failed to search test in jira due to bad request. Please delete the duplicate test manually in Jira (if any)");

                return false;

            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                if (parameters.Count > 0)
                {
                    foreach (KeyValuePair<String, String> parameter in parameters)
                        Utilities.LogError(string.Format("Failed to search test in jira due to Internal Server Error. Please delete the duplicate test manually in Jira (if any). Parameters: '{0}' - '{1}'", parameter.Key, parameter.Value));
                }
                else
                    Utilities.LogError("Failed to search test in jira due to Internal Server Error. Please delete the duplicate test manually in Jira (if any)");

                return false;
            }

            return DeserializeTest(response.Content);
        }

        /// <summary>
        /// Fetch the Duplicate test Case Key
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        private List<string> GetDuplicateTestKeys(String url, Dictionary<String, String> parameters = null, Dictionary<String, String> keys = null)
        {
            IRestResponse response = Client.Execute(GetRequest(url, parameters ?? new Dictionary<String, String>(), keys ?? new Dictionary<String, String>()));

            if (response.ErrorException != null)
            {
                if (parameters.Count > 0)
                {
                    foreach (KeyValuePair<String, String> parameter in parameters)
                        Utilities.LogError(string.Format("Exception occured while fetching Duplicate test records in jira. Please search for the test manually in Jira (if any). Parameters: '{0}' - '{1}'", parameter.Key, parameter.Value));
                }
                else
                    Utilities.LogError("Exception occured while fetching Duplicate test records in jira.Please search for the test manually in Jira (if any).");
            }
            else if (response.ResponseStatus != ResponseStatus.Completed)
            {
                if (parameters.Count > 0)
                {
                    foreach (KeyValuePair<String, String> parameter in parameters)
                        Utilities.LogError(string.Format("Response completed but unable to fetch test in jira. Please search for the test manually in Jira (if any). Parameters: '{0}' - '{1}'", parameter.Key, parameter.Value));
                }
                else
                    Utilities.LogError("Response completed but unable to fetch test in jira. Please search for the test manually in Jira (if any)");
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (parameters.Count > 0)
                {
                    foreach (KeyValuePair<String, String> parameter in parameters)
                        Utilities.LogError(string.Format("Failed to fetch duplicate test in jira due to bad request. Please search for the test manually in Jira (if any). Parameters: '{0}' - '{1}'", parameter.Key, parameter.Value));
                }
                else
                    Utilities.LogError("Failed to fetch duplicate test in jira due to bad request. Please search for the test manually in Jira (if any)");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                if (parameters.Count > 0)
                {
                    foreach (KeyValuePair<String, String> parameter in parameters)
                        Utilities.LogError(string.Format("Failed to fetch Duplicate test in jira due to Internal Server Error. Please search for the test manually in Jira (if any). Parameters: '{0}' - '{1}'", parameter.Key, parameter.Value));
                }
                else
                    Utilities.LogError("Failed to search test in jira due to Internal Server Error. Please delete the duplicate test manually in Jira (if any). Please search for the test manually in Jira (if any)");
            }
            return DeserializeDuplicateTest(response.Content);
        }

        /// <summary>
        /// Create Folder in Jira
        /// </summary>
        /// <param name="newFolder"></param>
        /// <returns></returns>
        public Folder CreateFolder(CreateFolder newFolder)
        {
            var request = GetRequest(JiraObjectEnum.Folder, new Dictionary<string, string>(), new Dictionary<string, string>());
            request.Method = Method.POST;
            request.AddJsonBody(newFolder);
            IRestResponse<Folder> response = this.Client.Execute<Folder>(request);

            if (response.ErrorException != null)
            {
                response.Data.ErrorMessages = response.ErrorException.ToString();
                Utilities.LogError(string.Format("Error occurred in creating Folder in Jira. Folder name is : {0}", newFolder.FolderName));
                throw response.ErrorException;
            }
            else if (response.ResponseStatus != ResponseStatus.Completed)
            {
                response.Data.StatusCode = response.ResponseStatus.ToString();
                response.Data.Message = response.ErrorMessage;
                Utilities.LogError(string.Format("Response completed but unable to create Folder in Jira. Folder name is : {0}", newFolder.FolderName));
                throw response.ErrorException;
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                response.Data.StatusCode = response.StatusCode.ToString();
                response.Data.Message = response.ErrorMessage;
                Utilities.LogError(string.Format("Internal Server Error while creating Folder in Jira. Folder name is : {0}", newFolder.FolderName));
                throw response.ErrorException;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                response.Data.StatusCode = response.StatusCode.ToString();
                response.Data.Message = response.ErrorMessage;

                if (response.ErrorMessage != null)
                    Utilities.LogError(string.Format("{0} : {1}", response.ErrorMessage, newFolder.FolderName));
                else
                    Utilities.LogVerbose(string.Format("Bad request while creating Folder.It seems Folder already exist in Jira. Folder name is : {0}", newFolder.FolderName));

                //throw response.ErrorException;  // DOn't Throw Exception ,We might need to run Import again and folder might exist.
            }
            return response.Data;
        }

        /// <summary>
        /// Create Test Cases in Jira
        /// </summary>
        /// <param name="newTestCase"></param>
        /// <returns></returns>
        public TestCase CreateTestCase(CreateTestCase newTestCase)
        {
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(newTestCase);
            var request = GetRequest(JiraObjectEnum.TestCases, new Dictionary<string, string>(), new Dictionary<string, string>());
            request.Method = Method.POST;
            request.AddJsonBody(newTestCase);

            IRestResponse<TestCase> response = this.Client.Execute<TestCase>(request);

            if (response.ErrorException != null)
            {
                response.Data.ErrorMessages = response.ErrorException.ToString();
                Utilities.LogError(string.Format("Error occuerd while creating the test case in jira. Check for the test case in downloaded worksheet: '{0}' - '{1}'", newTestCase.TestName, newTestCase.Folder));
            }
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                response.Data.StatusCode = response.ResponseStatus.ToString();
                response.Data.Message = response.ErrorMessage;
                Utilities.LogError(string.Format("Response completed but Unable to create the test case in jira. Check for the test case in downloaded worksheet: '{0}' - '{1}'", newTestCase.TestName, newTestCase.Folder));
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                response.Data.StatusCode = response.StatusCode.ToString();
                response.Data.Message = response.ErrorMessage;
                Utilities.LogError(string.Format("Internal server Error occuerd while creating the test case in jira. Check for the test case in downloaded worksheet: '{0}' - '{1}'", newTestCase.TestName, newTestCase.Folder));
            }
            return response.Data;
        }

        /// <summary>
        /// Delete Jira Test by ID
        /// </summary>
        /// <param name="strTestCaseKey"></param>
        /// <returns></returns>
        public bool DeleteJiraTestId(string strTestCaseKey)
        {
            var request = GetRequest(JiraObjectEnum.DeleteTest, new Dictionary<string, string>(), new Dictionary<string, string>() { { "testCaseKey", strTestCaseKey } });
            request.Method = Method.DELETE;
            var response = this.Client.Execute(request);
            //return response.StatusCode == HttpStatusCode.NoContent;
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
            else if (response.ErrorException != null)
            {
                Utilities.LogError(string.Format("Exception occured while deleting test case in jira. Test Key is: '{0}'", strTestCaseKey));
                return false;
            }
            else if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Utilities.LogError(string.Format("Response Completed but unable to delete test case in jira. Test Key is: '{0}'", strTestCaseKey));
                return false;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                Utilities.LogError(string.Format("Failed to delete test case in jira due to bad request. Test Key is: '{0}'", strTestCaseKey));
                return false;
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                Utilities.LogError(string.Format("Internal server error occured while deleting test case in jira. Test Key is: '{0}'", strTestCaseKey));
                return false;
            }

            return false;
        }

        /// <summary>
        /// Update the Jira Test Cases
        /// </summary>
        /// <param name="newTestCase"></param>
        /// <param name="strTestCaseKey"></param>
        /// <returns></returns>
        public bool UpdateTestCase(CreateTestCase newTestCase, string strTestCaseKey)
        {
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(newTestCase);
            var request = GetRequest(JiraObjectEnum.UpdateTestCase, new Dictionary<string, string>(), new Dictionary<string, string>() { { "testCaseKey", strTestCaseKey } });
            request.Method = Method.PUT;
            request.AddJsonBody(newTestCase);
            var response = this.Client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else if (response.ErrorException != null)
            {
                Utilities.LogError(string.Format("Exception occured while updating test case in jira. Test Key is: '{0}'", strTestCaseKey));
                return false;
            }
            else if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Utilities.LogError(string.Format("Response Completed but unable to update the test in jira. Test Key is: '{0}'", strTestCaseKey));
                return false;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                Utilities.LogError(string.Format("Failed to update the test case in jira due to bad request. Test Key is: '{0}'", strTestCaseKey));
                return false;
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                Utilities.LogError(string.Format("Internal server error occured while updating test case in jira. Test Key is: '{0}'", strTestCaseKey));
                return false;
            }

            return false;
        }

        /// <summary>
        /// Updating Design Steps.Its equivalent to Call to Test in Jira
        /// </summary>
        /// <param name="newTestSteps"></param>
        /// <param name="strTestCaseKey"></param>
        /// <returns></returns>
        public bool UpdateDesignSteps(TestScript newTestSteps, string strTestCaseKey)
        {
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(newTestCase);
            var request = GetRequest(JiraObjectEnum.UpdateTestCase, new Dictionary<string, string>(), new Dictionary<string, string>() { { "testCaseKey", strTestCaseKey } });
            request.Method = Method.PUT;
            request.AddJsonBody(newTestSteps);
            var response = this.Client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else if (response.ErrorException != null)
            {
                Utilities.LogError(string.Format("Exception occured while updating Design Steps in jira. Test Key is: '{0}'", strTestCaseKey));
                return false;
            }
            else if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Utilities.LogError(string.Format("Response Completed but unable to update the Design Steps in jira. Test Key is: '{0}'", strTestCaseKey));
                return false;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                Utilities.LogError(string.Format("Failed to update the test case in jira due to bad request. Test Key is: '{0}'", strTestCaseKey));
                return false;
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                Utilities.LogError(string.Format("Internal server error occured while updating Design Steps  in jira. Test Key is: '{0}'", strTestCaseKey));
                return false;
            }

            return false;
        }
                     
        /// <summary>
        /// Uplad Attachements to Jira ID
        /// </summary>
        /// <param name="strTestCaseKey"></param>
        /// <param name="strFilePath"></param>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public bool UploadAttachmentToTest(string strTestCaseKey, string strFilePath, string strFileName)
        {
            if (string.IsNullOrWhiteSpace(strTestCaseKey))
                throw new ArgumentOutOfRangeException("Test Case key is not prsent or created");

            var request = GetRequest(JiraObjectEnum.TestCaseAttachment, new Dictionary<string, string>(), new Dictionary<string, string>() { { "testCaseKey", strTestCaseKey } });
            request.Method = Method.POST;
            request.AddHeader("content-type", "multipart / form - data");
            request.AddFile("file", File.ReadAllBytes(strFilePath), Path.GetFileName(strFileName));
            request.AlwaysMultipartFormData = true;
            request.AddJsonBody(strTestCaseKey);
            IRestResponse response = Client.Execute(request);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return true;
            }
            else if (response.ErrorException != null)
            {
                Utilities.LogError(string.Format("Exception occured while uploading attachemet test in jira. Parameters: '{0}' - '{1}' - '{2}'", strTestCaseKey, strFilePath, strFileName));
                return false;
            }
            else if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Utilities.LogError(string.Format("Response Completed but unable to upload attachemet test in jira. Parameters: '{0}' - '{1}' - '{2}'", strTestCaseKey, strFilePath, strFileName));
                return false;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                Utilities.LogError(string.Format("Failed to upload attachemet test in jira due to bad request. Parameters: '{0}' - '{1}' - '{2}'", strTestCaseKey, strFilePath, strFileName));
                return false;
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                Utilities.LogError(string.Format("Internal server error occured while uploading attachemet test in jira. Parameters: '{0}' - '{1}' - '{2}'", strTestCaseKey, strFilePath, strFileName));
                return false;
            }

            return false;
        }

        /// <summary>
        /// Upload Design Attachments to TM4J
        /// </summary>
        /// <param name="strTestCaseKey"></param>
        /// <param name="strStepIndex"></param>
        /// <param name="strFilePath"></param>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public bool UploadDesignStepsAttachmentToTest(string strTestCaseKey, string strStepIndex, string strFilePath, string strFileName)
        {
            if (string.IsNullOrWhiteSpace(strTestCaseKey))
                throw new ArgumentOutOfRangeException("Test Case key is not prsent or created");

            var request = GetRequest(JiraObjectEnum.DesignStepsAttachment, new Dictionary<string, string>(), new Dictionary<string, string>() { { "testCaseKey", strTestCaseKey }, { "stepIndex", strStepIndex } });
            request.Method = Method.POST;
            request.AddHeader("content-type", "multipart / form - data");
            request.AddFile("file", File.ReadAllBytes(strFilePath), Path.GetFileName(strFileName));
            request.AlwaysMultipartFormData = true;
            request.AddJsonBody(strTestCaseKey);
            IRestResponse response = Client.Execute(request);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return true;
            }
            else if (response.ErrorException != null)
            {
                Utilities.LogError(string.Format("Exception occured while uploading design attachemet test in jira. Parameters: '{0}' - '{1}' - '{2}'", strTestCaseKey, strFilePath, strFileName));
                return false;
            }
            else if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Utilities.LogError(string.Format("Response Completed but unable to upload design attachemet test in jira. Parameters: '{0}' - '{1}' - '{2}'", strTestCaseKey, strFilePath, strFileName));
                return false;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                Utilities.LogError(string.Format("Failed to upload design attachemet test in jira due to bad request. Parameters: '{0}' - '{1}' - '{2}'", strTestCaseKey, strFilePath, strFileName));
                return false;
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                Utilities.LogError(string.Format("Internal server error occured while uploading design attachemet test in jira. Parameters: '{0}' - '{1}' - '{2}'", strTestCaseKey, strFilePath, strFileName));
                return false;
            }

            return false;
        }
        #endregion

        #region Execution Methos
        private T GetItem<T>(JiraObjectEnum objectType, Dictionary<String, String> parameters = null,
         Dictionary<String, String> keys = null) where T : new()
        {
            return Execute<T>(objectType, parameters, keys);
        }

        private List<T> GetList<T>(JiraObjectEnum objectType, Dictionary<String, String> parameters = null, Dictionary<String, String> keys = null) where T : new()
        {
            return Execute<List<T>>(objectType, parameters, keys);
        }

        private T Execute<T>(JiraObjectEnum objectType, Dictionary<String, String> parameters = null, Dictionary<String, String> keys = null) where T : new()
        {
            IRestResponse<T> response = Client.Execute<T>(GetRequest(objectType, parameters ?? new Dictionary<String, String>(), keys ?? new Dictionary<String, String>()));

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException(response.Request.Resource);
            }
            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception(response.ErrorMessage);
            }

            return response.Data;
        }

        public RestRequest GetRequest(JiraObjectEnum objectType, Dictionary<String, String> parameters,
          Dictionary<String, String> keys)
        {
            if (!_methods.ContainsKey(objectType))
                throw new NotImplementedException();

            return GetRequest(_methods[objectType], parameters, keys);
        }

        public RestRequest GetRequest(String url, Dictionary<String, String> parameters, Dictionary<String, String> keys)
        {
            RestRequest request = new RestRequest(url, Method.GET)
            {
                RequestFormat = DataFormat.Json,
                OnBeforeDeserialization = resp => resp.ContentType = "application/json",
                JsonSerializer = new RestSharpJsonNetSerializer()
            };


            foreach (KeyValuePair<String, String> key in keys)
            {
                request.AddParameter(key.Key, key.Value, ParameterType.UrlSegment);
            }

            foreach (KeyValuePair<String, String> parameter in parameters)
            {
                request.AddParameter(parameter.Key, parameter.Value);
            }

            return request;
        }

        #endregion

        #region CommonMethods

        private List<string> DeserializeDuplicateTest(string json)
        {
            List<string> lstDuplicateTestKeys = new List<string>();

            if (!string.IsNullOrEmpty(json))
            {
                JArray jsonObject = JArray.Parse(json);

                if (jsonObject != null)
                {
                    if (jsonObject.Count > 0)
                    {
                        foreach (JObject item in jsonObject)
                        {
                            lstDuplicateTestKeys.Add((string)item["key"]);
                            break;
                        }
                    }
                }
            }

            return lstDuplicateTestKeys;
        }
        private bool DeserializeTest(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                JArray jsonObject = JArray.Parse(json);
                if (jsonObject != null)
                {
                    if (jsonObject.Count > 0)
                        return true;
                }
            }
            return false;
        }

        #endregion
    }
}