using System;
using TDAPIOLELib;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ALM_EXTRACT.JiraAPI;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace ALM_EXTRACT
{
    public partial class Utility : System.Web.UI.Page
    {
        #region Global Declaration
        private DesignSteps dsgnSteps;
        private TestPlan tstPlan;
        private TestAttachments tstAttachments;
        public static TDConnection otaConnection;
        public Jira jiraClient;
        string strProjectName;
        string strDomainName;

        private const string strWrksheetFileName = "HPQC_Test_Plan_Report";
        private const string strFailedImportFileName = "Failed_Import_Jira_Test_Report";
        private const string strDuplicateImportFileName = "Duplicate_Import_Jira_Test_Report";
        private const string strSuccessfulImportFileName = "Successful_Import_Jira_Test_Report";
        private const string strTestIdClmn = "Test ID";
        private const string strJiraKeyClmn = "Test Key";
        private const string strTestNameClmn = "Test Name";
        private const string strErrorClmn = "Error";

        #endregion

        #region Asp page button opeartions
        /// <summary>
        /// Page Load Initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Clear text and visibility
                ClearErrorLabel();

                if (Session["myTDConnection"] != null)
                {
                    otaConnection = (TDConnection)Session["myTDConnection"];
                    dsgnSteps = new DesignSteps(otaConnection);
                    tstPlan = new TestPlan(otaConnection);
                    tstAttachments = new TestAttachments(otaConnection);
                    strProjectName = otaConnection.ProjectName;
                    strDomainName = otaConnection.DomainName;
                }
                else
                    Response.Redirect("~/View/Forbidden.aspx?", false);
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
            }
        }

        /// <summary>
        /// Get HPQC Worksheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnworksheet_Click(object sender, EventArgs e)
        {
            if (IsValidInput() || IsValidLogin() || IsValidAttachment())
                return;

            ClearErrorLabel();
            try
            {
                if (otaConnection != null)
                {
                    DataTable dstALMTable = CreateDataTable();
                    if (dstALMTable.Rows.Count > 0)
                        Utilities.ExportDatatoExcel(dstALMTable, txtAttachmentPath.Value, strProjectName, strDomainName, strWrksheetFileName);
                    else
                    {
                        Utilities.LogError("Unable to fetch the HPQC Test data . Please try again after some time.");
                        lblTextError.Visible = true;
                        lblTextError.Value = "Unable to fetch the HPQC Test data. Please try again after some time";
                    }
                }
                else
                {
                    lblTextError.Visible = true;
                    lblTextError.Value = "Unable to connect with HPQC while fetching Test Cases";
                    Utilities.LogError("Unable to connect with HPQC while fetching Test Cases");
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
            }

        }

        /// <summary>
        /// Get all custom Field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGetCustomField_Click(object sender, EventArgs e)
        {
            if (IsValidInput() || IsValidLogin() || IsValidAttachment())
                return;

            ClearErrorLabel();

            try
            {
                if (otaConnection != null)
                {
                    string lstCstmfldName = GetCustomFields();
                    if (!string.IsNullOrEmpty(lstCstmfldName))
                    {
                        lblTextError.Visible = true;
                        lblTextError.Value = lstCstmfldName;
                    }
                    else
                    {
                        lblTextError.Visible = true;
                        lblTextError.Value = string.Format("There is no Custom field present in this Project-{0}", strProjectName);
                        Utilities.LogVerbose(string.Format("There is no Custom field  present in this Project-{0}", strProjectName));
                    }
                }
                else
                {
                    lblTextError.Visible = true;
                    lblTextError.Value = "There is no active connection availiable with HPQC";
                    Utilities.LogError("There is no active connection availiable with HPQC");
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
            }
        }

        /// <summary>
        /// Logout from the HPALM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            ClearErrorLabel();

            try
            {
                if (otaConnection != null)
                {
                    new ALM_Core().LogoutALM(otaConnection);

                    //Disposing Jira object.
                    if (jiraClient != null)
                        jiraClient.Dispose(true);

                    //Just to double check
                    if (!otaConnection.ProjectConnected)
                        if (!otaConnection.LoggedIn)
                            Response.Redirect("~/Login.aspx?", false);
                }
                else
                {
                    lblTextError.Visible = true;
                    lblTextError.Value = "There is no active connection availiable with HPQC & Jira. Please wiat .. Redirecting to login page.";
                    Utilities.LogError("There is no active connection availiable with HPQC & Jira. Please wiat .. Redirecting to login page.");
                    Thread.Sleep(10000);
                    Response.Redirect("~/Login.aspx?", false);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
            }
        }

        /// <summary>
        /// Download Test Attachments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDwnldAttchmnts_Click(object sender, EventArgs e)
        {
            if (IsValidInput() || IsValidLogin() || IsValidAttachment())
                return;

            ClearErrorLabel();

            try
            {
                if (otaConnection != null)
                {
                    //string strpath = @"C:\Users\1019884\Desktop\HP ALM";
                    string strDwnloadpath = string.Format(txtAttachmentPath.Value.Trim() + "\\{0}", strProjectName);

                    if ((Directory.Exists(strDwnloadpath)) == false)
                        Directory.CreateDirectory(strDwnloadpath);

                    Recordset testset;
                    //Recordset testUnattachedSet;

                    // Fetch all datasets
                    //testUnattachedSet = tstPlan.GetAllUnattachedTestsAndDesignSteps();
                    testset = tstPlan.GetAllTestsAndDesignSteps();

                    for (int i = 1; i <= testset.RecordCount; i++)
                    {
                        if (testset["TS_ATTACHMENT"] != null)
                        {
                            if (testset["TS_ATTACHMENT"] == "Y")
                                if (testset["TS_TEST_ID"] != null)
                                    tstAttachments.DownloadTestAttachmentsWithTestID(testset["TS_TEST_ID"], strDwnloadpath);
                        }
                        testset.Next();
                    }
                    //for (int i = 1; i <= testUnattachedSet.RecordCount; i++)
                    //{
                    //    if (testset["TS_ATTACHMENT"] != null)
                    //    {
                    //        if (testset["TS_ATTACHMENT"] == "Y")
                    //            if (testset["TS_TEST_ID"] != null)
                    //                tstAttachments.DownloadTestAttachmentsWithTestID(testset["TS_TEST_ID"], strDwnloadpath);
                    //    }
                    //    testset.Next();
                    //}
                }
                else
                {
                    lblTextError.Visible = true;
                    lblTextError.Value = "There is no active connection availiable with HPQC";
                    Utilities.LogError("There is no active connection availiable with HPQC");
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
            }
        }

        /// <summary>
        /// Download Design Test Attachments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDwnldDsgnsAttchmnts_Click(object sender, EventArgs e)
        {
            if (IsValidInput() || IsValidLogin() || IsValidAttachment())
                return;

            ClearErrorLabel();

            try
            {
                if (otaConnection != null)
                {
                    Recordset testset;
                    //Recordset testUnattachedSet;

                    string strDwnloadpath = string.Format(txtAttachmentPath.Value.Trim() + "\\{0}", strProjectName);

                    if ((Directory.Exists(strDwnloadpath)) == false)
                        Directory.CreateDirectory(strDwnloadpath);

                    // Fetch all datasets
                    //testUnattachedSet = tstPlan.GetAllUnattachedTestsAndDesignSteps();
                    testset = tstPlan.GetAllTestsAndDesignSteps();

                    for (int i = 1; i <= testset.RecordCount; i++)
                    {
                        if (testset["DS_ATTACHMENT"] != null)
                        {
                            if (testset["DS_ATTACHMENT"] == "Y")
                                if (testset["TS_TEST_ID"] != null)
                                    dsgnSteps.DownloadDesignStepsAttachmentsWithTestID(testset["TS_TEST_ID"], strDwnloadpath);
                        }
                        testset.Next();
                    }
                    //for (int i = 1; i <= testUnattachedSet.RecordCount; i++)
                    //{
                    //    if (testset["DS_ATTACHMENT"] != null)
                    //    {
                    //        if (testset["DS_ATTACHMENT"] == "Y")
                    //            if (testset["TS_TEST_ID"] != null)
                    //                dsgnSteps.DownloadDesignStepsAttachmentsWithTestID(testset["TS_TEST_ID"], strDwnloadpath);
                    //    }
                    //    testset.Next();
                    //}
                }
                else
                {
                    lblTextError.Visible = true;
                    lblTextError.Value = "There is no active connection availiable with HPQC";
                    Utilities.LogError("There is no active connection availiable with HPQC");
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
            }
        }

        /// <summary>
        /// Get the Total Distinct Test Case Count
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnHPQCTestCount_Click(object sender, EventArgs e)
        {
            if (IsValidInput() || IsValidLogin() || IsValidAttachment())
                return;

            ClearErrorLabel();

            try
            {
                if (otaConnection != null && tstPlan != null)
                {
                    int lstHPQCTstCount = tstPlan.CountAllTests();
                    lblTextError.Visible = true;
                    lblTextError.Value = string.Format("The total distinct test Case in HPQC is-{0}", lstHPQCTstCount);
                    Utilities.LogVerbose(string.Format("The total distinct test Case present in HPQC is-{0}", lstHPQCTstCount));
                }
                else
                {
                    lblTextError.Visible = true;
                    lblTextError.Value = "There is no active connection availiable with HPQC";
                    Utilities.LogError("There is no active connection availiable with HPQC");
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
            }
        }

        /// <summary>
        /// Get The Duplicate Test Names in Jira- Which might created earlier in Jira
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFind_TM4J_Dplicate_Test_Click(object sender, EventArgs e)
        {
            if (IsValidInput() || IsValidLogin() || IsValidAttachment())
                return;

            ClearErrorLabel();
            List<string> lstDuplicateTestCaseName = new List<string>();

            try
            {
                string strProjectName = txtProject.Value;
                string strJiraRestURI = txtJiraUrl.Value;
                string strJiraUN = txtUserName.Value;
                string strJiraPwd = txtPWD.Value;
                string strProjKey = string.Empty;
                jiraClient = new Jira();
                DataTable dstALMTable = new DataTable();

                //Retrieve the HPALM DataTable
                dstALMTable = CreateDataTable(true);

                if (dstALMTable != null)
                {
                    if (dstALMTable.Rows.Count > 0)
                    {
                        //Connect to the Jira
                        jiraClient.Connect(strJiraRestURI, strJiraUN, strJiraPwd);

                        if (jiraClient != null)
                        {
                            //Get the Project Key for creating Jira testcases
                            strProjKey = jiraClient.GetProjects().FirstOrDefault(prjName => prjName.Name.ToUpper().Trim() == strProjectName.ToUpper().Trim()).Key;

                            if (!string.IsNullOrEmpty(strProjKey))
                            {
                                foreach (string tstId in Utilities.IsDistinctTestID(dstALMTable))
                                {
                                    string strJQLCommand = string.Empty;
                                    string strTestName = string.Empty;
                                    string strFolderName = string.Empty;
                                    UnclearErrorLabel();

                                    foreach (DataRow rowInfo in Utilities.GetDataRowsbyTestID(dstALMTable, tstId))
                                    {
                                        strTestName = rowInfo["Test Name"].ToString();
                                        strFolderName = rowInfo["Subject"].ToString();
                                        if (!string.IsNullOrEmpty(strTestName) && !string.IsNullOrEmpty(strFolderName))
                                            break;
                                    }

                                    if (!string.IsNullOrEmpty(strTestName) && !string.IsNullOrEmpty(strFolderName))
                                    {
                                        // Using JQL command check for the duplicate records in Jira Test management
                                        strJQLCommand = String.Format("projectKey = \"{0}\" AND name = \"{1}\" AND folder = \"{2}\"", strProjKey, strTestName, strFolderName);

                                        // Get the List of Duplicate Ids to delete
                                        if (jiraClient.SearchTest(strJQLCommand))
                                        {
                                            if (!lstDuplicateTestCaseName.Contains(strTestName))
                                                lstDuplicateTestCaseName.Add(strTestName);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Utilities.LogError("Unable to find the project with this name");
                                lblTextError.Visible = true;
                                lblTextError.Value = "Unable to find the project with this name";
                            }
                        }
                        else
                        {
                            Utilities.LogError("Unable to Connect with Jira. Retry again.");
                            lblTextError.Visible = true;
                            lblTextError.Value = "Unable to Connect with Jira. Retry again.";
                        }
                    }
                    else
                    {
                        Utilities.LogError("Unable to fetch the HPQC Test data . Please try again after some time.");
                        lblTextError.Visible = true;
                        lblTextError.Value = "Unable to fetch the HPQC Test data. Please try again after some time";
                    }
                }
                else
                {
                    Utilities.LogError("Unable to fetch the HPQC Test data . Please try again after some time.");
                    lblTextError.Visible = true;
                    lblTextError.Value = "Unable to fetch the HPQC Test data. Please try again after some time";
                }


            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
            }
            finally
            {
                // Inform Admin about Duplicate Test Case which is present in Jira 
                if (lstDuplicateTestCaseName != null)
                {
                    if (lstDuplicateTestCaseName.Count == 0)
                    {
                        Utilities.LogVerbose(string.Format("There is No Dulpicate Test found in Jira for the Project :'{0}'", strProjectName));
                        lblTextError.Visible = true;
                        lblTextError.Value = (string.Format("There is No Dulpicate Test found in Jira for the Project :'{0}'", strProjectName));
                    }
                    else if (lstDuplicateTestCaseName.Count > 0)
                    {
                        Utilities.LogVerbose(string.Format("Duplicate Test present for the project -'{0}' .The Jira test Names are-'{1}'", strProjectName, string.Join(",", lstDuplicateTestCaseName)));
                        lblTextError.Visible = true;
                        lblTextError.Value = string.Format("Duplicate Test present for the project -'{0}' .You will recieve the Duplicate test Names in your provided email Id", strProjectName);
                        Utilities.SendReport(txtEmail.Value, string.Join(",", lstDuplicateTestCaseName), strProjectName, Convert.ToString(lstDuplicateTestCaseName.Count), true);
                    }
                }
            }
        }

        /// <summary>
        /// Delete Duplicate Test present in Jira- Which might created earlier in Jira
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_TM4J_Dplicate_Test_Click(object sender, EventArgs e)
        {
            if (IsValidInput() || IsValidLogin() || IsValidAttachment())
                return;

            ClearErrorLabel();

            try
            {
                string strProjectName = txtProject.Value;
                string strJiraRestURI = txtJiraUrl.Value;
                string strJiraUN = txtUserName.Value;
                string strJiraPwd = txtPWD.Value;
                string strProjKey = string.Empty;
                jiraClient = new Jira();
                DataTable dstALMTable = new DataTable();
                List<string> lstDuplicateJiraTestId = new List<string>();

                //Retrieve the HPALM DataTable
                dstALMTable = CreateDataTable(true);

                if (dstALMTable != null)
                {
                    if (dstALMTable.Rows.Count > 0)
                    {
                        //Connect to the Jira
                        jiraClient.Connect(strJiraRestURI, strJiraUN, strJiraPwd);

                        if (jiraClient != null)
                        {
                            //Get the Project Key for creating Jira testcases
                            strProjKey = jiraClient.GetProjects().FirstOrDefault(prjName => prjName.Name.ToUpper().Trim() == strProjectName.ToUpper().Trim()).Key;

                            if (!string.IsNullOrEmpty(strProjKey))
                            {
                                foreach (string tstId in Utilities.IsDistinctTestID(dstALMTable))
                                {
                                    string strJQLCommand = string.Empty;
                                    string strTestName = string.Empty;
                                    string strFolderName = string.Empty;
                                    UnclearErrorLabel();

                                    foreach (DataRow rowInfo in Utilities.GetDataRowsbyTestID(dstALMTable, tstId))
                                    {
                                        strTestName = rowInfo["Test Name"].ToString();
                                        strFolderName = rowInfo["Subject"].ToString();
                                        if (!string.IsNullOrEmpty(strTestName) && !string.IsNullOrEmpty(strFolderName))
                                            break;
                                    }

                                    if (!string.IsNullOrEmpty(strTestName) && !string.IsNullOrEmpty(strFolderName))
                                    {
                                        // Using JQL command check for the duplicate records in Jira Test management
                                        strJQLCommand = String.Format("projectKey = \"{0}\" AND name = \"{1}\" AND folder = \"{2}\"", strProjKey, strTestName, strFolderName);

                                        // Get the List of Duplicate Ids to delete
                                        foreach (string testId in jiraClient.FindDuplicateJiraTestKeys(strJQLCommand))
                                        {
                                            if (!lstDuplicateJiraTestId.Contains(testId))
                                                lstDuplicateJiraTestId.Add(testId);
                                        }
                                    }
                                }

                                // Delete all the Duplicate Test Case one by one in Jira 
                                if (lstDuplicateJiraTestId != null)
                                {
                                    if (lstDuplicateJiraTestId.Count == 0)
                                    {
                                        Utilities.LogVerbose(string.Format("There is No Dulpicate Test found in Jira for the Project :'{0}'", strProjectName));
                                        lblTextError.Visible = true;
                                        lblTextError.Value = (string.Format("There is No Dulpicate Test found in Jira for the Project :'{0}'", strProjectName));
                                    }
                                    else if (lstDuplicateJiraTestId.Count > 0)
                                    {
                                        //Delete the Jira test Id one by one
                                        Parallel.ForEach(lstDuplicateJiraTestId, strTestCaseKey =>
                                        {
                                            jiraClient.DeleteJiraTestId(strTestCaseKey);
                                        });

                                        lblTextError.Visible = true;
                                        lblTextError.Value = string.Format("Duplicate Test deleted for the project -'{0}' .You will recieve the Deleted test Id's in your provided email Id", strProjectName);
                                        Utilities.LogVerbose(string.Format("Duplicate Test deleted for the project -'{0}' .The deleted Jira test Id's are-'{1}'", strProjectName, string.Join(",", lstDuplicateJiraTestId)));
                                        Utilities.SendReport(txtEmail.Value, string.Join(",", lstDuplicateJiraTestId), strProjectName, Convert.ToString(lstDuplicateJiraTestId.Count), true);
                                    }
                                }
                            }
                            else
                            {
                                Utilities.LogError("Unable to find the project with this name");
                                lblTextError.Visible = true;
                                lblTextError.Value = "Unable to find the project with this name";
                            }
                        }
                        else
                        {
                            Utilities.LogError("Unable to Connect with Jira. Retry again.");
                            lblTextError.Visible = true;
                            lblTextError.Value = "Unable to Connect with Jira. Retry again.";
                        }
                    }
                    else
                    {
                        Utilities.LogError("Unable to fetch the HPQC Test data . Please try again after some time.");
                        lblTextError.Visible = true;
                        lblTextError.Value = "Unable to fetch the HPQC Test data. Please try again after some time";
                    }
                }
                else
                {
                    Utilities.LogError("Unable to fetch the HPQC Test data . Please try again after some time.");
                    lblTextError.Visible = true;
                    lblTextError.Value = "Unable to fetch the HPQC Test data. Please try again after some time";
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
            }
        }

        /// <summary>
        /// Process Error Records (If any)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Process_Error_Test_Import_Tm4J_Click(object sender, EventArgs e)
        {
            if (IsValidInput() || IsValidLogin() || IsValidAttachment())
                return;

            ClearErrorLabel();
            List<string> lstTestCaseKeys = null;

            try
            {
                if (otaConnection != null)
                {
                    List<string> lstTestIds = Utilities.ReadExcelFile(txtAttachmentPath.Value.Trim(), strFailedImportFileName, strTestIdClmn);

                    if (lstTestIds.Count > 0)
                        lstTestCaseKeys = DoImport(txtJiraUrl.Value, txtUserName.Value, txtPWD.Value, txtProject.Value, lstTestIds, true);
                    else
                    {
                        Utilities.LogError(string.Format("Looks Like All the Test Cases have already Imported.None of the Test Cases Import failed for this Project '{0}'", txtProject.Value));
                        Utilities.SendErrorEmail(txtEmail.Value, "Looks Like All the Test Cases have already Imported.None of the Test Cases Import failed for this Project'{0}'", txtProject.Value);
                        lblTextError.Visible = true;
                        lblTextError.Value = string.Format("Looks Like All the Test Cases have already Imported.None of the Test Cases Import failed for this Project '{0}'", txtProject.Value);
                    }
                }
                else
                {
                    lblTextError.Visible = true;
                    lblTextError.Value = "There is no active connection availiable with HPQC";
                    Utilities.LogError("There is no active connection availiable with HPQC");
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                Utilities.SendErrorEmail(txtEmail.Value, ex.Message.ToString(), txtProject.Value);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
            }
            finally
            {
                ProcessFinalEmailOperation(lstTestCaseKeys);
            }
        }

        /// <summary>
        /// Importing HP ALM Data to TM4J
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImport_Tm4J_Click(object sender, EventArgs e)
        {
            if (IsValidInput() || IsValidLogin() || IsValidAttachment())
                return;

            ClearErrorLabel();
            List<string> lstTestCaseKeys = null;

            try
            {
                if (otaConnection != null)
                    lstTestCaseKeys = DoImport(txtJiraUrl.Value, txtUserName.Value, txtPWD.Value, txtProject.Value);
                else
                {
                    lblTextError.Visible = true;
                    lblTextError.Value = "There is no active connection availiable with HPQC";
                    Utilities.LogError("There is no active connection availiable with HPQC");
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                Utilities.SendErrorEmail(txtEmail.Value, ex.Message.ToString(), txtProject.Value);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
            }
            finally
            {
                ProcessFinalEmailOperation(lstTestCaseKeys);
            }
        }

        #endregion

        #region Common Methods

        /// <summary>
        /// Do the Jira Import
        /// </summary>
        /// <param name="strJiraRestURI"></param>
        /// <param name="strJiraUN"></param>
        /// <param name="strJiraPwd"></param>
        /// <param name="strProjectName"></param>
        /// <returns></returns>
        private List<string> DoImport(string strJiraRestURI, string strJiraUN, string strJiraPwd, string strProjectName, List<string> lstTestIds = null, bool isProcessDuplicateErrorDataTable = false)
        {
            List<string> lstTestCaseKeys = null;
            Dictionary<string, string> dcftDuplicateTestCase = null;
            Dictionary<string, string> dcftFailedTestId = null;
            lblTextError.Value = string.Empty;
            lblTextError.Visible = false;

            try
            {
                string strProjKey = string.Empty;
                jiraClient = new Jira();
                DataTable dstALMTable = new DataTable();
                CreateTestCase createJiraTest;
                TestCase jiraTestCase;
                lstTestCaseKeys = new List<string>();
                dcftDuplicateTestCase = new Dictionary<string, string>();
                dcftFailedTestId = new Dictionary<string, string>();

                //Retrieve the HPALM DataTable
                if (isProcessDuplicateErrorDataTable)
                    dstALMTable = ProcessDuplicate_ErrorDataTable(lstTestIds, true);
                else
                    dstALMTable = CreateDataTable(true);

                if (dstALMTable != null)
                {
                    if (dstALMTable.Rows.Count > 0)
                    {
                        //Connect to the Jira
                        jiraClient.Connect(strJiraRestURI, strJiraUN, strJiraPwd);

                        if (jiraClient != null)
                        {
                            //Get the Project Key for creating Jira testcases
                            strProjKey = jiraClient.GetProjects().FirstOrDefault(prjName => prjName.Name.ToUpper().Trim() == strProjectName.ToUpper().Trim()).Key;

                            if (!string.IsNullOrEmpty(strProjKey))
                            {
                                jiraTestCase = new TestCase();

                                //Create the distinct folder in Jira as per HPALM
                                var jiraFolderLists = dstALMTable.AsEnumerable().Select(row => new { Name = row.Field<string>("Subject") }).Distinct();
                                if (jiraFolderLists != null)
                                {
                                    Parallel.ForEach(jiraFolderLists, folderName =>
                                   {
                                       CreateFolder createJiraFolder = new CreateFolder()
                                       {
                                           ProjectKey = strProjKey,
                                           FolderName = folderName.Name,
                                           FieldType = "TEST_CASE" // Use this Typr for creating TestCase only
                                       };
                                       jiraClient.CreateFolder(createJiraFolder);
                                   });

                                    //Fetch the rows based on each Distinct TestId along with field mappings 
                                    //Parallel.ForEach(Utilities.IsDistinctTestID(dstALMTable), tstId =>
                                    //{
                                    foreach (string tstId in Utilities.IsDistinctTestID(dstALMTable))
                                    {
                                        if (!string.IsNullOrEmpty(tstId))
                                        {
                                            bool IsAttachment = false;
                                            bool IsDsgnAttachment = false;
                                            string strAttachmentPath = string.Empty;
                                            string strDsgnAttachmentPath = string.Empty;
                                            string strJQLCommand = string.Empty;
                                            UnclearErrorLabel();

                                            createJiraTest = new CreateTestCase();
                                            customFields jiraCustomFields = new customFields();
                                            List<Steps> jiraTestSteps = new List<Steps>();

                                            createJiraTest.TestScript = new TestScript { DesignStepType = "STEP_BY_STEP" }; // Set "step by step" . No custom field as of in Design Steps.
                                            createJiraTest.ProjectKey = strProjKey; // Project Key ( mandatory  field in Jira)

                                            //fetch the rows based on Distinct Test Id
                                            foreach (DataRow rowInfo in Utilities.GetDataRowsbyTestID(dstALMTable, tstId))
                                            {
                                                Steps jiraTestStep = new Steps();
                                                for (int colCount = 0; colCount < dstALMTable.Columns.Count; colCount++)
                                                {
                                                    string strColName = dstALMTable.Columns[colCount].ToString();

                                                    if (!string.IsNullOrEmpty(strColName) && !string.IsNullOrEmpty(rowInfo[strColName].ToString()))
                                                    {
                                                        if (isProcessDuplicateErrorDataTable)
                                                            createJiraTest = MappingFields(strColName, rowInfo[strColName].ToString(), createJiraTest, jiraCustomFields, jiraTestStep, true);
                                                        else
                                                            createJiraTest = MappingFields(strColName, rowInfo[strColName].ToString(), createJiraTest, jiraCustomFields, jiraTestStep);
                                                    }
                                                }

                                                //Check for attachments
                                                if (rowInfo["Attachments"] != null)
                                                {
                                                    if (rowInfo["Attachments"].ToString() == "Y")
                                                    {
                                                        if (string.IsNullOrEmpty(strAttachmentPath))
                                                        {
                                                            strAttachmentPath = rowInfo["Attachments Path"].ToString();
                                                            IsAttachment = true;
                                                        }
                                                    }
                                                }

                                                //Check for Design attachments
                                                if (rowInfo["Design Attachments"] != null)
                                                {
                                                    if (rowInfo["Design Attachments"].ToString() == "Y")
                                                    {
                                                        jiraTestStep.IsDesignAttachment = true;
                                                        if (string.IsNullOrEmpty(strDsgnAttachmentPath))
                                                        {
                                                            strDsgnAttachmentPath = rowInfo["Design Attachments Path"].ToString();
                                                            IsDsgnAttachment = true;
                                                        }
                                                    }
                                                }
                                                //Adding Design Steps. Restrict Empty entries of HPQC
                                                if (jiraTestStep.IsDesignAttachment || !string.IsNullOrEmpty(jiraTestStep.TestData) || !string.IsNullOrEmpty(jiraTestStep.Description) || !string.IsNullOrEmpty(jiraTestStep.ExpectedResult))
                                                    jiraTestSteps.Add(jiraTestStep);

                                                createJiraTest.TestScript.Steps = jiraTestSteps.ToArray();
                                            }

                                            // Using JQL command check for the duplicate records in Jira Test management
                                            strJQLCommand = String.Format("projectKey = \"{0}\" AND name = \"{1}\" AND folder = \"{2}\"", strProjKey, createJiraTest.TestName, createJiraTest.Folder);

                                            if (!jiraClient.SearchTest(strJQLCommand))
                                            {
                                                //Create the Jira Test case
                                                jiraTestCase = jiraClient.CreateTestCase(createJiraTest);

                                                //Upload the attachment (if any) after Test creation.
                                                if (!string.IsNullOrEmpty(jiraTestCase.Key) && string.IsNullOrEmpty(jiraTestCase.ErrorMessages))
                                                {
                                                    // Returning the successful imported Keys
                                                    lstTestCaseKeys.Add(jiraTestCase.Key);

                                                    //Upload Test Attachments
                                                    if (IsAttachment)
                                                    {
                                                        bool isAttachmentUploadSuccess = UploadAttachmentToTest(jiraTestCase.Key.Trim(), strAttachmentPath);
                                                        if (!isAttachmentUploadSuccess)
                                                        {
                                                            Utilities.LogError(string.Format("Unable to upload the Attachment file for the jira test case Id- {0}", jiraTestCase.Key.Trim()));
                                                            lblTextError.Visible = true;
                                                            lblTextError.Value = string.Format("Unable to upload the Attachment file for the jira test case Id- {0}", jiraTestCase.Key.Trim()); ;
                                                        }
                                                    }
                                                    //Upload Design Attachments
                                                    if (IsDsgnAttachment)
                                                    {
                                                        bool isDsgnAttachmentUploadSuccess = UploadDesignStepsAttachmentToTest(jiraTestCase.Key.Trim(), createJiraTest.TestScript.Steps, strDsgnAttachmentPath);
                                                        if (!isDsgnAttachmentUploadSuccess)
                                                        {
                                                            Utilities.LogError(string.Format("Unable to upload the Design Attachment file for the jira test case Id- {0}", jiraTestCase.Key.Trim()));
                                                            lblTextError.Visible = true;
                                                            lblTextError.Value = string.Format("Unable to upload the Design Attachment file for the jira test case Id- {0}", jiraTestCase.Key.Trim());
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (!dcftFailedTestId.ContainsKey(Convert.ToString(createJiraTest.CustomFields.TestId)))
                                                        dcftFailedTestId.Add(Convert.ToString(createJiraTest.CustomFields.TestId), jiraTestCase.ErrorMessages);
                                                }
                                            }
                                            else
                                            {
                                                if (!dcftDuplicateTestCase.ContainsKey(Convert.ToString(createJiraTest.CustomFields.TestId)))
                                                    dcftDuplicateTestCase.Add(Convert.ToString(createJiraTest.CustomFields.TestId), createJiraTest.TestName);
                                            }
                                            //});
                                        }
                                    }
                                }
                                else
                                {
                                    Utilities.LogError("Something Went wrong while fetching the Folder list from HPQC under - Subject");
                                    lblTextError.Visible = true;
                                    lblTextError.Value = "Something Went wrong while fetching the Folder list under - Subject";
                                }
                            }
                            else
                            {
                                Utilities.LogError("Unable to find the project with this name");
                                lblTextError.Visible = true;
                                lblTextError.Value = "Unable to find the project with this name";
                            }
                        }
                        else
                        {
                            Utilities.LogError("Unable to Connect with Jira. Retry again.");
                            lblTextError.Visible = true;
                            lblTextError.Value = "Unable to Connect with Jira. Retry again.";
                        }
                    }
                    else
                    {
                        Utilities.LogError("Unable to fetch the HPQC Test data . Please try again after some time.");
                        lblTextError.Visible = true;
                        lblTextError.Value = "Unable to fetch the HPQC Test data. Please try again after some time";
                        return null;
                    }
                }
                else
                {
                    Utilities.LogError("Unable to fetch the HPQC Test data. Please try again after some time.");
                    lblTextError.Visible = true;
                    lblTextError.Value = "Unable to fetch the HPQC Test data. Please try again after some time.";
                }

                return lstTestCaseKeys;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
                //return lstTestCaseKeys;
                throw ex;
            }
            finally
            {
                ProcessFinalErrorReport(dcftFailedTestId);
                ProcessFinalDuplicateReport(dcftDuplicateTestCase);
                ProcessFinalEmailOperation(lstTestCaseKeys, true);
            }
        }

        /// <summary>
        /// Only common Case Email Operations
        /// </summary>
        /// <param name="lstTestCaseKeys"></param>
        /// <param name="IsImport"></param>
        private void ProcessFinalEmailOperation(List<string> lstTestCaseKeys, bool IsImport = false)
        {
            if (lstTestCaseKeys != null)
            {
                if (lstTestCaseKeys.Count > 0)
                {
                    lblTextError.Visible = true;
                    lblTextError.Value = string.Format(@"{0}-Test Case Imported successfully for the Project -{1}", Convert.ToString(lstTestCaseKeys.Count), txtProject.Value);
                    Utilities.LogVerbose(string.Format(@"{0}-Test Case Imported successfully for the Project -{1}", Convert.ToString(lstTestCaseKeys.Count), txtProject.Value));
                    Utilities.ExportDatatoExcel(Utilities.CreateDataTable_FromList(lstTestCaseKeys, strJiraKeyClmn), txtAttachmentPath.Value, strProjectName, strDomainName, strSuccessfulImportFileName);
                    Utilities.SendReport(txtEmail.Value, string.Join(",", lstTestCaseKeys), txtProject.Value, Convert.ToString(lstTestCaseKeys.Count));
                }
                else if (lstTestCaseKeys.Count == 0)
                {
                    if (!IsImport)
                    {
                        Utilities.LogError(string.Format("Duplicate Import-Test Data already exist in Jira for project- {0}.No test case created in jira", txtProject.Value));
                        Utilities.SendErrorEmail(txtEmail.Value, "Duplicate Import-Test Data already exist in Jira. No changes in TM4J", txtProject.Value);
                        lblTextError.Visible = true;
                        lblTextError.Value = string.Format("Duplicate Import-Test Data already exist in Jira for project- {0}.No test case created in jira", txtProject.Value);
                    }
                }
            }
            else
            {
                Utilities.LogError(string.Format("Importing Test Case in Jira Failed for project- {0}.Unable to create test case created in jira", txtProject.Value));
                Utilities.SendErrorEmail(txtEmail.Value, "Importing Test Case in Jira Failed", txtProject.Value);
                lblTextError.Visible = true;
                lblTextError.Value = string.Format("Importing Test Case in Jira Failed for project- {0}.Unable to create test case created in jira", txtProject.Value);
            }
        }

        /// <summary>
        /// Only Error Case Email Operations
        /// </summary>
        /// <param name="dcftFailedTestId"></param>
        private void ProcessFinalErrorReport(Dictionary<string, string> dcftFailedTestId)
        {
            // Inform Admin about Error while creating Test case in Jira 
            if (dcftFailedTestId != null)
            {
                if (dcftFailedTestId.Count > 0)
                {
                    //TO Do : Load the Report in the Directory
                    StringBuilder sb = new StringBuilder();
                    sb.Append(string.Format("Unable to create the  below Test cases for the Project '{0}'.", strProjectName));
                    sb.Append(Environment.NewLine);

                    Parallel.ForEach(dcftFailedTestId, item =>
                    {
                        sb.Append(string.Format("Test Id : '{0}' With Error is '{1}'", item.Key, item.Value));
                        sb.Append(Environment.NewLine);
                    });

                    Utilities.LogError(sb.ToString());
                    Utilities.ExportDatatoExcel(Utilities.CreateDataTable(dcftFailedTestId, strTestIdClmn, strErrorClmn), txtAttachmentPath.Value, strProjectName, strDomainName, strFailedImportFileName);
                    Utilities.SendErrorEmail(txtEmail.Value, string.Join(" , ", dcftFailedTestId.Keys), strProjectName, Convert.ToString(dcftFailedTestId.Count), true);
                }
            }
        }

        /// <summary>
        /// Only Duplicate Case Email Operations
        /// </summary>
        /// <param name="dcftDuplicateTestCase"></param>
        private void ProcessFinalDuplicateReport(Dictionary<string, string> dcftDuplicateTestCase)
        {
            // Inform Admin about Duplicate Test Case present in Jira 
            if (dcftDuplicateTestCase != null)
            {
                if (dcftDuplicateTestCase.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(string.Format("Duplicate tests found for the Project '{0}'.", strProjectName));
                    sb.Append(Environment.NewLine);

                    Parallel.ForEach(dcftDuplicateTestCase, item =>
                    {
                        sb.Append(string.Format("Duplicate :Test Id : '{0}' and Test Name is : '{1}'", item.Key, item.Value));
                        sb.Append(Environment.NewLine);
                    });

                    Utilities.LogVerbose(sb.ToString());
                    Utilities.ExportDatatoExcel(Utilities.CreateDataTable(dcftDuplicateTestCase, strTestIdClmn, strTestNameClmn), txtAttachmentPath.Value, strProjectName, strDomainName, strDuplicateImportFileName);
                    Utilities.SendReport(txtEmail.Value, string.Join(" , ", dcftDuplicateTestCase.Values), strProjectName, Convert.ToString(dcftDuplicateTestCase.Count), true);
                }
            }
        }

        /// <summary>
        /// Mapping HPQC & Jira Field
        /// </summary>
        /// <param name="strColumnName"> Coulm Name</param>
        /// <param name="strRowValue">Column Value</param>
        /// <param name="jiraTestDetails"></param>
        /// <param name="jiraCustomFields"></param>
        /// <param name="jiraTestStep"></param>
        /// <param name="isProcessDuplicateErrorDataTable">Only If we are processing Error  report</param>
        /// <returns></returns>
        private CreateTestCase MappingFields(string strColumnName, string strRowValue, CreateTestCase jiraTestDetails, customFields jiraCustomFields, Steps jiraTestStep, bool isProcessDuplicateErrorDataTable = false)
        {
            switch (strColumnName.ToUpper().Trim())
            {
                #region Default Fields
                case "COMPONENET":
                case "MODULE":
                    jiraTestDetails.Component = strRowValue ?? string.Empty;
                    break;
                case "OBJECTIVE":
                case "PURPOSE":
                    jiraTestDetails.Objective = strRowValue ?? string.Empty;
                    break;
                case "PRECONDITION":
                case "PREREQUISITE":
                    jiraTestDetails.PreCondition = strRowValue ?? string.Empty;
                    break;
                case "LABELS":
                    string[] Labeltokens = null;
                    List<string> jiraLabels = new List<string>();

                    if (strRowValue.Contains(","))
                        Labeltokens = strRowValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    else if (strRowValue.Contains(";"))
                        Labeltokens = strRowValue.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    else
                    {
                        if (!string.IsNullOrEmpty(strRowValue))
                        {
                            jiraLabels.Add(strRowValue);
                            jiraTestDetails.Labels = jiraLabels.ToArray();
                            break;
                        }
                        else
                        {
                            jiraTestDetails.Labels = null;
                            break;
                        }
                    }

                    if (Labeltokens != null)
                    {
                        if (Labeltokens.Length > 0)
                        {
                            foreach (string label in Labeltokens)
                                jiraLabels.Add(label);

                            jiraTestDetails.Labels = jiraLabels.ToArray();
                            break;
                        }
                        else
                        {
                            jiraTestDetails.Labels = null;
                            break;
                        }
                    }
                    else
                        jiraTestDetails.Labels = null;

                    break;

                #endregion

                #region Test- Jira System Fields
                case "TEST NAME":
                    jiraTestDetails.TestName = strRowValue ?? string.Empty;
                    break;
                case "DESIGNER":
                    if (!String.IsNullOrEmpty(strRowValue))
                    {
                        User IsValidUser = jiraClient.GetUser(strRowValue);
                        if (!string.IsNullOrEmpty(IsValidUser.Username) && !string.IsNullOrEmpty(IsValidUser.Name))
                        {
                            jiraTestDetails.Owner = strRowValue;
                            break;
                        }
                        else
                        {
                            // This probability is very less but still possible so inform admin through error logs that we are setting up null
                            if (isProcessDuplicateErrorDataTable)
                            {
                                //jiraTestDetails.Owner = null; 
                                //Utilities.LogVerbose(string.Format("Unable to find the Designer/Owner '{0}' in jira. Setting up Designer/Owner Value as Null to avoid failure in creating Test cases", strRowValue));

                                Utilities.LogError(string.Format("Unable to find the Designer/Owner '{0}' in jira while processing Error records. Please update valid user manually in jira or directly in HPQC", strRowValue));
                                break;
                            }
                            else
                            {
                                //jiraTestDetails.Owner = null; 
                                Utilities.LogError(string.Format("Unable to find the Designer/Owner '{0}' in jira. Please update valid user manually in jira or directly in HPQC", strRowValue));
                                break;
                            }
                        }
                    }
                    else
                    {
                        Utilities.LogVerbose(string.Format("User/Owner field is empty. Setting up Designer/Owner Value as Null to avoid failure in creating Test cases", strRowValue));
                        jiraTestDetails.Owner = null;
                    }
                    break;

                case "STATUS":
                    jiraTestDetails.Status = strRowValue ?? string.Empty;
                    break;

                case "SUBJECT":
                    jiraTestDetails.Folder = strRowValue ?? string.Empty;
                    break;

                #endregion

                #region Test- Jira Custom Fields

                case "CHANGE STATUS":
                    jiraCustomFields.ChangeStatus = strRowValue ?? string.Empty;
                    break;

                case "CREATION DATE":
                    jiraCustomFields.CreationDate = Utilities.ParseDate(strRowValue ?? string.Empty);
                    break;

                case "TEST NAME DESCRIPTION":
                    jiraCustomFields.TestNameDescription = strRowValue ?? string.Empty;
                    break;

                case "COMMENTS":
                    jiraCustomFields.Comments = strRowValue ?? string.Empty;
                    break;

                case "ESTIMATED DEVTIME":
                    jiraCustomFields.EstimatedDevTime = strRowValue ?? string.Empty;
                    break;

                case "EXECUTION STATUS":
                    jiraCustomFields.ExecutionStatus = strRowValue ?? string.Empty;
                    break;

                case "PATH":
                    jiraCustomFields.Path = strRowValue ?? string.Empty;
                    break;

                case "PROTOCOL TYPE":
                    jiraCustomFields.ProtocolType = strRowValue ?? string.Empty;
                    break;

                case "TESTING MODE":
                    jiraCustomFields.TestingMode = strRowValue ?? string.Empty;
                    break;

                case "TEMPLATE":
                    jiraCustomFields.Template = strRowValue ?? string.Empty;
                    break;

                case "TEST ID":
                    //int convertedval = 0;
                    //Int32.TryParse(strRowValue, out convertedval);
                    jiraCustomFields.TestId = strRowValue ?? string.Empty;
                    break;

                case "TREE PATH":
                    jiraCustomFields.TreePath = strRowValue ?? string.Empty;
                    break;

                case "TYPE":
                    jiraCustomFields.Type = strRowValue ?? string.Empty;
                    break;

                case "WORKING MODE":
                    jiraCustomFields.WorkingMode = strRowValue ?? string.Empty;
                    break;

                case "EXTENDED TEST DATA":
                    jiraCustomFields.ExtendedTestData = strRowValue ?? string.Empty;
                    break;

                case "PERFORMANCE CENTER'S DATA XML":
                    jiraCustomFields.PerformanceCentersDataXML = strRowValue ?? string.Empty;
                    break;

                case "TOTAL DURATION OF TEST":
                    jiraCustomFields.TotalDurationOfTest = strRowValue ?? string.Empty;
                    break;

                case "TEST INTERNAL ERRORS:":
                    jiraCustomFields.TestInternalErrors = strRowValue ?? string.Empty;
                    break;

                case "SLA DEFINED":
                    jiraCustomFields.SLADefined = strRowValue ?? string.Empty;
                    break;

                case "TOTAL AMOUNT OF VUSERS":
                    jiraCustomFields.TotalVusers = strRowValue ?? string.Empty;
                    break;

                case "TEST VALIDITY":
                    jiraCustomFields.TestValidity = strRowValue ?? string.Empty;
                    break;

                case "CATEGORY":
                    jiraCustomFields.Category = strRowValue ?? string.Empty;
                    break;

                case "FEATURE":
                    jiraCustomFields.Feature = strRowValue ?? string.Empty;
                    break;

                case "JIRA ID":
                    jiraCustomFields.JiraId = strRowValue ?? string.Empty;
                    break;

                case "ORIGIN":
                    jiraCustomFields.Origin = strRowValue ?? string.Empty;
                    break;

                case "STORY":
                    jiraCustomFields.Story = strRowValue ?? string.Empty;
                    break;

                case "IDENTIFIED IN RELEASE":
                    jiraCustomFields.IdentifiedInRelease = strRowValue ?? string.Empty;
                    break;

                case "RELEASE":
                    jiraCustomFields.Release = strRowValue ?? string.Empty;
                    break;

                case "REQ NUM":
                    jiraCustomFields.ReqNum = strRowValue ?? string.Empty;
                    break;

                case "DS_STEP_NAME":
                    jiraCustomFields.DS_STEP_NAME = strRowValue ?? string.Empty;
                    break;

                case "REVIEWER":
                    jiraCustomFields.Reviewer = strRowValue ?? string.Empty;
                    break;
                case "SPRINT":
                    jiraCustomFields.Sprint = strRowValue ?? string.Empty;
                    break;
                case "AUTOMATED":
                    jiraCustomFields.Automated = strRowValue ?? string.Empty;
                    break;
                case "JDA SCENARIO ID":
                    jiraCustomFields.JDAScenarioID = strRowValue ?? string.Empty;
                    break;
                case "JDA FITNESSE STATUS":
                    jiraCustomFields.JDAFitnesseStatus = strRowValue ?? string.Empty;
                    break;
                case "JDA IF-VERSION":
                    jiraCustomFields.JDA_IF_Version = strRowValue ?? string.Empty;
                    break;
                case "DEFECT ID":
                    jiraCustomFields.DefectId = strRowValue ?? string.Empty;
                    break;
                case "AUTOMATION TYPE":
                    jiraCustomFields.Automation_Type = strRowValue ?? string.Empty;
                    break;
                case "RELEASE VERSION":
                    jiraCustomFields.ReleaseVersion = strRowValue ?? string.Empty;
                    break;
                case "PRIORITY(LEGACY)":
                    jiraCustomFields.Priority_Legacy = strRowValue ?? string.Empty;
                    break;
                case "SOURCE_PROJECT":
                    jiraCustomFields.Source_Project = strRowValue ?? string.Empty;
                    break;
                case "VERSION":
                    jiraCustomFields.Version = strRowValue ?? string.Empty;
                    break;
                case "VERSION TO":
                    jiraCustomFields.VersionTo = strRowValue ?? string.Empty;
                    break;
                case "AUTOMATD TESTCASE BACKEND":
                    jiraCustomFields.Automatd_Testcase_Backend = strRowValue ?? string.Empty;
                    break;
                case "AUTOMATED TESTCASE UI":
                    jiraCustomFields.Automated_Testcase_UI = strRowValue ?? string.Empty;
                    break;
                case "TEST CASE TYPE":
                    jiraCustomFields.TestCase_Type = strRowValue ?? string.Empty;
                    break;
                case "SOURCE_VERSION":
                    jiraCustomFields.Source_Version = strRowValue ?? string.Empty;
                    break;
                case "CUSTOMER USE CASE / REQUIREMENT NAME":
                    jiraCustomFields.Customer_Use_Case = strRowValue ?? string.Empty;
                    break;

                case "LEGACYID":
                    jiraCustomFields.LegacyId = strRowValue ?? string.Empty;
                    break;

                case "AUTOMATION":
                    jiraCustomFields.Automation = strRowValue ?? string.Empty;
                    break;

                case "JIRA":
                    jiraCustomFields.Jira = strRowValue ?? string.Empty;
                    break;

                case "MODEL":
                    jiraCustomFields.Model = strRowValue ?? string.Empty;
                    break;

                case "DB TYPE":
                    jiraCustomFields.DB_Type = strRowValue ?? string.Empty;
                    break;

                case "SERVER":
                    jiraCustomFields.Server = strRowValue ?? string.Empty;
                    break;

                case "AUTOMATION ID":
                    jiraCustomFields.AutomationID = strRowValue ?? string.Empty;
                    break;

                case "SETUP":
                    jiraCustomFields.Setup = strRowValue ?? string.Empty;
                    break;

                case "VAULT ID":
                    jiraCustomFields.VaultID = strRowValue ?? string.Empty;
                    break;

                case "TEST PRIORITY":
                    jiraCustomFields.TestPriority = strRowValue ?? string.Empty;
                    break;

                case "CLIENT TYPE":
                    jiraCustomFields.ClientType = strRowValue ?? string.Empty;
                    break;

                case "GROUP NAME":
                    jiraCustomFields.GroupName = strRowValue ?? string.Empty;
                    break;

                case "SET UP":
                    jiraCustomFields.Set_UP = strRowValue ?? string.Empty;
                    break;

                case "TEST CLIENT":
                    jiraCustomFields.TestClient = strRowValue ?? string.Empty;
                    break;

                case "IMPLEMENTED VERSION":
                    jiraCustomFields.ImplementedVersion = strRowValue ?? string.Empty;
                    break;

                case "DEPRECATED  VERSION":
                    jiraCustomFields.DeprecatedVersion = strRowValue ?? string.Empty;
                    break;

                case "TESTCASE QUALITY":
                    jiraCustomFields.TestCaseQuality = strRowValue ?? string.Empty;
                    break;

                case "TESTCASE QUALITY COMMENT":
                    jiraCustomFields.TestCaseQualityComment = strRowValue ?? string.Empty;
                    break;

                case "PRICING PAW REGRESSION":
                    jiraCustomFields.PricingPawRegression = strRowValue ?? string.Empty;
                    break;

                case "PRE-REQUISITES":
                    jiraCustomFields.Prerequisites = strRowValue ?? string.Empty;
                    break;

                case "WEIGHTAGE":
                    jiraCustomFields.Weightage = strRowValue ?? string.Empty;
                    break;

                case "PROJECT":
                    jiraCustomFields.Project = strRowValue ?? string.Empty;
                    break;

                case "SEVERITY":
                    jiraCustomFields.Severity = strRowValue ?? string.Empty;
                    break;

                case "PREFERENCE":
                    jiraCustomFields.Preference = strRowValue ?? string.Empty;
                    break;

                case "IS HMM":
                    jiraCustomFields.Is_HMM = strRowValue ?? string.Empty;
                    break;

                case "TM RELEASE":
                    jiraCustomFields.TM_Release = strRowValue ?? string.Empty;
                    break;

                case "SPEC/REF #":
                    jiraCustomFields.Spec_Ref = strRowValue ?? string.Empty;
                    break;

                case "TESTAUTOMATEDBY":
                    if (!String.IsNullOrEmpty(strRowValue))
                    {
                        User IsValidUser = jiraClient.GetUser(strRowValue);
                        if (!string.IsNullOrEmpty(IsValidUser.Username) && !string.IsNullOrEmpty(IsValidUser.Name))
                        {
                            jiraCustomFields.TestAutomatedBy = strRowValue;
                            break;
                        }
                        else
                        {
                            // This probability is very less but still possible so inform admin through error logs that we are setting up null
                            if (isProcessDuplicateErrorDataTable)
                            {
                                //jiraTestDetails.Owner = null; 

                                Utilities.LogError(string.Format("Unable to find the Test Automated by User '{0}' in jira while processing Error records. Please update valid user manually in jira or directly in HPQC", strRowValue));
                                break;
                            }
                            else
                            {
                                //jiraTestDetails.Owner = null; 
                                Utilities.LogError(string.Format("Unable to find the Test Automated by User '{0}' in jira. Please update valid user manually in jira or directly in HPQC", strRowValue));
                                break;
                            }
                        }
                    }
                    else
                    {
                        Utilities.LogVerbose(string.Format("Test Automated by User field is empty. Setting up Test Automated by User Value as Null to avoid failure in creating Test cases", strRowValue));
                        jiraCustomFields.TestAutomatedBy = null;
                    }
                    break;

                case "ISSUE#":
                    jiraCustomFields.Issue = strRowValue ?? string.Empty;
                    break;

                case "PRIORITY":
                    jiraTestDetails.Priority = string.Empty; // Go with default always.(Low,High,Normal)
                    jiraCustomFields.Priority = strRowValue ?? string.Empty;
                    break;
                #endregion

                #region Design Step Jira System Field
                case "STEP NAME":
                    jiraTestStep.Description = strRowValue ?? string.Empty;
                    break;
                case "DESCRIPTION":
                case "PROCEDURE":
                    jiraTestStep.TestData = strRowValue ?? string.Empty;
                    break;
                case "EXPECTED RESULT":
                case "EXPECTED":
                    jiraTestStep.ExpectedResult = strRowValue ?? string.Empty;
                    break;
                case "DESIGN ID":
                    jiraTestStep.StepId = strRowValue ?? string.Empty;
                    break;
                #endregion

                default:
                    break;
            }

            jiraTestDetails.CustomFields = jiraCustomFields;
            return jiraTestDetails;
        }

        /// <summary>
        /// Downlaod Attachments and get the Path.
        /// </summary>
        /// <param name="strHPALMTestId"></param>
        /// <returns></returns>
        private string DwnldAttchmntsAndPathByTestId(string strHPALMTestId)
        {
            try
            {
                string strDwnloadpath = string.Format(txtAttachmentPath.Value.Trim() + "\\{0}", strProjectName);

                if ((Directory.Exists(strDwnloadpath)) == false)
                    Directory.CreateDirectory(strDwnloadpath);

                return tstAttachments.DownloadTestAttachmentsWithTestID(strHPALMTestId, strDwnloadpath); ;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Download the Design Attachments by Test Id
        /// </summary>
        /// <param name="strHPALMTestId"></param>
        /// <returns></returns>
        private string DwnldDesignAttchmntsAndPathByTestId(string strHPALMTestId)
        {
            try
            {
                string strDwnloadpath = string.Format(txtAttachmentPath.Value.Trim() + "\\{0}", strProjectName);

                if ((Directory.Exists(strDwnloadpath)) == false)
                    Directory.CreateDirectory(strDwnloadpath);

                return dsgnSteps.DownloadDesignStepsAttachmentsWithTestID(strHPALMTestId, strDwnloadpath);
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Uploading Attachments to the Test
        /// </summary>
        /// <param name="strTestCaseKey"></param>
        /// <param name="strAttachmentPath"></param>
        /// <returns></returns>
        private bool UploadAttachmentToTest(string strTestCaseKey, string strAttachmentPath)
        {
            bool isUplaodSucess = false;

            if (jiraClient != null)
            {
                if (!string.IsNullOrEmpty(strAttachmentPath))
                {
                    foreach (string strFilePath in Directory.GetFiles(strAttachmentPath, "*", SearchOption.AllDirectories).ToList()) //iterate the file list
                    {
                        if (!string.IsNullOrEmpty(strFilePath))
                        {
                            // Always the file name is in the format of (TEST_TestID_Filename)
                            string[] tokens = Path.GetFileName(new FileInfo(strFilePath).FullName).ToUpper().Replace("TEST_", "").Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                            if (tokens.Length >= 2)
                            {
                                string strFileName = string.Join("_", tokens.Skip(1));
                                isUplaodSucess = jiraClient.UploadAttachmentToTest(strTestCaseKey, strFilePath, strFileName);
                            }
                        }
                    }
                }
                else
                    Utilities.LogError("Attachment path is empty.Unable to upload the Design Attachements in Jira");
            }
            else
                Utilities.LogError("Jira Client is not connected while uploading the attachments");

            return isUplaodSucess;
        }

        /// <summary>
        /// Uploading Design Steps Attachments
        /// </summary>
        /// <param name="strTestCaseKey"></param>
        /// <param name="designSteps"></param>
        /// <param name="strAttachmentPath"></param>
        /// <returns></returns>
        private bool UploadDesignStepsAttachmentToTest(string strTestCaseKey, Steps[] designSteps, string strAttachmentPath)
        {
            bool isUplaodSucess = false;

            if (jiraClient != null && designSteps != null)
            {
                if (!string.IsNullOrEmpty(strAttachmentPath))
                {
                    //iterate through the file directory
                    foreach (string strFilePath in Directory.GetFiles(strAttachmentPath, "*", SearchOption.AllDirectories).ToList())
                    {
                        if (!string.IsNullOrEmpty(strFilePath))
                        {
                            // Always the file name is in the format of (DESSTEPS_DesignStepId_Filename)
                            string[] tokens = Path.GetFileName(new FileInfo(strFilePath).FullName).ToUpper().Replace("DESSTEPS_", "").Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                            if (tokens.Length >= 2)
                            {
                                string strDesignId = tokens[0].ToString().Trim();
                                string strFileName = string.Join("_", tokens.Skip(1));
                                // Fetch the step index for uploading attachments in design step
                                int strStepIndex = Array.FindIndex(designSteps, item => item.StepId == strDesignId);
                                if (strStepIndex != -1)
                                    isUplaodSucess = jiraClient.UploadDesignStepsAttachmentToTest(strTestCaseKey, Convert.ToString(strStepIndex), strFilePath, strFileName);
                            }
                        }
                    }
                }
                else
                    Utilities.LogError("Design Attachment path is empty. Unable to upload the Design Attachements in Jira");
            }
            else
                Utilities.LogError("Jira Client is not connected or design steps id not present while uploading the Design attachments");

            return isUplaodSucess;
        }

        /// <summary>
        /// Get the Custom Fields
        /// </summary>
        /// <returns></returns>
        private string GetCustomFields()
        {
            List<string> lstCstmfldName = new List<string>();
            try
            {
                Dictionary<string, string> dctSFList = new Dictionary<string, string>();
                Recordset testPrjectFieldsForTest = tstPlan.GetSystemFieldDetailsForTest();
                Recordset testPrjectFieldsForDesignSteps = tstPlan.GetSystemFieldDetailsForDesignSteps();

                for (int Counter = 1; Counter <= testPrjectFieldsForTest.RecordCount; Counter++)
                {
                    if (!dctSFList.ContainsKey(testPrjectFieldsForTest["SF_COLUMN_NAME"]))
                        dctSFList.Add(Convert.ToString(testPrjectFieldsForTest["SF_COLUMN_NAME"]), Convert.ToString(testPrjectFieldsForTest["SF_USER_LABEL"]));

                    testPrjectFieldsForTest.Next();
                }
                for (int Counter = 1; Counter <= testPrjectFieldsForDesignSteps.RecordCount; Counter++)
                {
                    if (!dctSFList.ContainsKey(testPrjectFieldsForDesignSteps["SF_COLUMN_NAME"]))
                        dctSFList.Add(Convert.ToString(testPrjectFieldsForDesignSteps["SF_COLUMN_NAME"]), Convert.ToString(testPrjectFieldsForDesignSteps["SF_USER_LABEL"]));

                    testPrjectFieldsForDesignSteps.Next();
                }
                foreach (KeyValuePair<string, string> tstKey in dctSFList)
                {
                    if (tstKey.Key.Contains("TS_USER_") || tstKey.Key.Contains("DS_USER_"))
                        lstCstmfldName.Add(tstKey.Value);
                }

                return string.Join(",", lstCstmfldName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Create DataTable
        /// </summary>
        /// <param name="IsjirsExport">if the call is from Jira</param>
        /// <returns></returns>
        private DataTable CreateDataTable(bool IsjiraExport = false)
        {
            DataTable dstALMTable = new DataTable();
            try
            {
                Dictionary<string, string> dctSFList = new Dictionary<string, string>();
                Recordset testset;
                Recordset testPrjectFieldsForTest;
                Recordset testPrjectFieldsForDesignSteps;
                //Recordset testUnattachedSet;

                // Fetch all datasets
                //testUnattachedSet = tstPlan.GetAllUnattachedTestsAndDesignSteps();
                testset = tstPlan.GetAllTestsAndDesignSteps();
                testPrjectFieldsForTest = tstPlan.GetSystemFieldDetailsForTest();
                testPrjectFieldsForDesignSteps = tstPlan.GetSystemFieldDetailsForDesignSteps();

                //Bind the system file into a dictionary
                for (int Counter = 1; Counter <= testPrjectFieldsForTest.RecordCount; Counter++)
                {
                    if (!dctSFList.ContainsKey(testPrjectFieldsForTest["SF_COLUMN_NAME"]))
                    {
                        // TS_Desciption label is same as user label DS_Description. So changing the label
                        if (testPrjectFieldsForTest["SF_COLUMN_NAME"] == "TS_DESCRIPTION")
                        {
                            dctSFList.Add(Convert.ToString(testPrjectFieldsForTest["SF_COLUMN_NAME"]), "Test Name Description");
                        }
                        else
                            dctSFList.Add(Convert.ToString(testPrjectFieldsForTest["SF_COLUMN_NAME"]), Convert.ToString(testPrjectFieldsForTest["SF_USER_LABEL"]));
                    }

                    testPrjectFieldsForTest.Next();
                }

                for (int Counter = 1; Counter <= testPrjectFieldsForDesignSteps.RecordCount; Counter++)
                {
                    if (!dctSFList.ContainsKey(testPrjectFieldsForDesignSteps["SF_COLUMN_NAME"]))
                        dctSFList.Add(Convert.ToString(testPrjectFieldsForDesignSteps["SF_COLUMN_NAME"]), Convert.ToString(testPrjectFieldsForDesignSteps["SF_USER_LABEL"]));

                    testPrjectFieldsForDesignSteps.Next();
                }

                //Adding Necessary token which is not picked up by Active System Field
                dctSFList.Add("TS_ATTACHMENT", "Attachments");
                dctSFList.Add("DS_ATTACHMENT", "Design Attachments");
                dctSFList.Add("TS_ATTACHMENT_PATH", "Attachments Path");
                dctSFList.Add("DS_ATTACHMENT_PATH", "Design Attachments Path");
                dctSFList.Add("DS_ID", "Design Id");

                if (!IsjiraExport)
                {
                    dstALMTable = ExportData(dctSFList, testset, dstALMTable);
                    //dstALMTable = ExportData(dctSFList, testUnattachedSet, dstALMTable); // Able to fetch Unattached data from main query hence Commented
                }
                else
                {
                    dstALMTable = ExportJiraData(dctSFList, testset, dstALMTable);
                    //dstALMTable = ExportJiraData(dctSFList, testUnattachedSet, dstALMTable); // Able to fetch Unattached data from main query hence Commented
                }
                dstALMTable = Utilities.RemoveUnusedColumns(dstALMTable);
                return dstALMTable;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
                throw ex;
            }
        }
        private DataTable ProcessDuplicate_ErrorDataTable(List<string> strTestIds, bool IsjiraExport = false)
        {
            DataTable dstALMTable = new DataTable();
            try
            {
                Dictionary<string, string> dctSFList = new Dictionary<string, string>();
                Recordset testset;
                Recordset testPrjectFieldsForTest;
                Recordset testPrjectFieldsForDesignSteps;

                // Fetch all datasets
                testset = tstPlan.GetTestByIds(string.Join(",", strTestIds));
                testPrjectFieldsForTest = tstPlan.GetSystemFieldDetailsForTest();
                testPrjectFieldsForDesignSteps = tstPlan.GetSystemFieldDetailsForDesignSteps();

                //Bind the system file into a dictionary
                for (int Counter = 1; Counter <= testPrjectFieldsForTest.RecordCount; Counter++)
                {
                    if (!dctSFList.ContainsKey(testPrjectFieldsForTest["SF_COLUMN_NAME"]))
                    {
                        // TS_Desciption label is same as user label DS_Description. So changing the label
                        if (testPrjectFieldsForTest["SF_COLUMN_NAME"] == "TS_DESCRIPTION")
                        {
                            dctSFList.Add(Convert.ToString(testPrjectFieldsForTest["SF_COLUMN_NAME"]), "Test Name Description");
                        }
                        else
                            dctSFList.Add(Convert.ToString(testPrjectFieldsForTest["SF_COLUMN_NAME"]), Convert.ToString(testPrjectFieldsForTest["SF_USER_LABEL"]));
                    }

                    testPrjectFieldsForTest.Next();
                }

                for (int Counter = 1; Counter <= testPrjectFieldsForDesignSteps.RecordCount; Counter++)
                {
                    if (!dctSFList.ContainsKey(testPrjectFieldsForDesignSteps["SF_COLUMN_NAME"]))
                        dctSFList.Add(Convert.ToString(testPrjectFieldsForDesignSteps["SF_COLUMN_NAME"]), Convert.ToString(testPrjectFieldsForDesignSteps["SF_USER_LABEL"]));

                    testPrjectFieldsForDesignSteps.Next();
                }

                //Adding Necessary token which is not picked up by Active System Field
                dctSFList.Add("TS_ATTACHMENT", "Attachments");
                dctSFList.Add("DS_ATTACHMENT", "Design Attachments");
                dctSFList.Add("TS_ATTACHMENT_PATH", "Attachments Path");
                dctSFList.Add("DS_ATTACHMENT_PATH", "Design Attachments Path");
                dctSFList.Add("DS_ID", "Design Id");

                if (!IsjiraExport)
                    dstALMTable = ExportData(dctSFList, testset, dstALMTable);
                else
                    dstALMTable = ExportJiraData(dctSFList, testset, dstALMTable);

                dstALMTable = Utilities.RemoveUnusedColumns(dstALMTable);
                return dstALMTable;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = ex.Message.ToString();
                throw ex;
            }
        }
        /// <summary>
        /// Exprt Data through Iterartion
        /// </summary>
        /// <param name="dctSFList"></param>
        /// <param name="testset"></param>
        /// <param name="dstALMTable"></param>
        private DataTable ExportData(Dictionary<string, string> dctSFList, Recordset testset, DataTable dstALMTable)
        {
            try
            {
                if (dstALMTable != null)
                {
                    //Adding Columns
                    foreach (var columnValue in dctSFList)
                        if (!dstALMTable.Columns.Contains(Utilities.CleanInvalidXmlChars(columnValue.Value)))
                            dstALMTable.Columns.Add(Utilities.CleanInvalidXmlChars(columnValue.Value));

                    for (int i = 1; i <= testset.RecordCount; i++)
                    {
                        DataRow row = dstALMTable.NewRow();
                        bool ifRowexists = Utilities.IsDuplicateExist(dstALMTable, testset["TS_TEST_ID"].ToString(), Utilities.CleanInvalidXmlChars(testset["TS_NAME"].ToString()));

                        foreach (var item in dctSFList)
                        {
                            if (!string.IsNullOrEmpty(item.Key))
                            {
                                /*Sometime we have couple of design steps and while adding it creteas dupliacte item with other columns. To remove this 
                                 enable this below code and comment the above.*/
                                if (!ifRowexists)
                                {
                                    if (item.Key.Contains("TS_ATTACHMENT_PATH"))
                                    {
                                        if (testset["TS_ATTACHMENT"] != null)
                                        {
                                            if (testset["TS_ATTACHMENT"] == "Y")
                                                row[item.Value] = DwnldAttchmntsAndPathByTestId(testset["TS_TEST_ID"].ToString());
                                        }
                                    }
                                    else if (item.Key.Contains("DS_ATTACHMENT_PATH"))
                                    {
                                        if (testset["DS_ATTACHMENT"] != null)
                                        {
                                            if (testset["DS_ATTACHMENT"] == "Y")
                                                row[item.Value] = DwnldDesignAttchmntsAndPathByTestId(testset["TS_TEST_ID"].ToString());
                                        }
                                    }
                                    else if (testset[item.Key] == null)
                                        row[item.Value] = DBNull.Value;
                                    else if (item.Key.Contains("TS_SUBJECT"))
                                        row[item.Value] = GetFolderPath(Convert.ToInt32(Utilities.CleanInvalidXmlChars(testset["AL_ITEM_ID"])));
                                    else if (item.Key.Contains("TS_CREATION_DATE") || item.Key.Contains("TS_VC_CHECKIN_DATE") || item.Key.Contains("TS_VC_DATE") || item.Key.Contains("TS_VTS"))
                                        row[item.Value] = Convert.ToDateTime(Utilities.CleanInvalidXmlChars(testset[item.Key]));
                                    else
                                        row[item.Value] = Utilities.CleanInvalidXmlChars(testset[item.Key]);
                                }
                                else
                                {
                                    if (testset[item.Key] == null)
                                        row[item.Value] = DBNull.Value;
                                    else if (item.Key.Contains("DS_STEP_NAME") || item.Key.Contains("DS_DESCRIPTION") || item.Key.Contains("DS_EXPECTED"))
                                        row[item.Value] = Utilities.CleanInvalidXmlChars(testset[item.Key]);
                                    else if (item.Key.Contains("DS_ATTACHMENT_PATH"))
                                    {
                                        if (testset["DS_ATTACHMENT"] != null)
                                        {
                                            if (testset["DS_ATTACHMENT"] == "Y")
                                                row[item.Value] = DwnldDesignAttchmntsAndPathByTestId(testset["TS_TEST_ID"].ToString());
                                        }
                                    }
                                    else if (item.Key.Contains("DS_ID"))
                                        row[item.Value] = Utilities.CleanInvalidXmlChars(testset[item.Key]);
                                }
                            }
                        }
                        dstALMTable.Rows.Add(row);
                        testset.Next();
                    }
                }
                else
                    Utilities.LogError("No data found in datatable for HPQC export");

                return dstALMTable;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Export Data for Jira
        /// </summary>
        /// <param name="dctSFList"></param>
        /// <param name="testset"></param>
        /// <param name="dstALMTable"></param>
        /// <returns></returns>
        private DataTable ExportJiraData(Dictionary<string, string> dctSFList, Recordset testset, DataTable dstALMTable)
        {
            try
            {
                if (dstALMTable != null)
                {
                    //Adding Columns
                    foreach (var columnValue in dctSFList)
                        if (!dstALMTable.Columns.Contains(Utilities.CleanInvalidXmlChars(columnValue.Value)))
                            dstALMTable.Columns.Add(Utilities.CleanInvalidXmlChars(columnValue.Value));

                    for (int i = 1; i <= testset.RecordCount; i++)
                    {
                        DataRow row = dstALMTable.NewRow();
                        foreach (var item in dctSFList)
                        {
                            if (!string.IsNullOrEmpty(item.Key))
                            {
                                if (item.Key.Contains("TS_ATTACHMENT_PATH"))
                                {
                                    if (testset["TS_ATTACHMENT"] != null)
                                    {
                                        if (testset["TS_ATTACHMENT"] == "Y")
                                            row[item.Value] = DwnldAttchmntsAndPathByTestId(testset["TS_TEST_ID"].ToString());
                                    }
                                }
                                else if (item.Key.Contains("DS_ATTACHMENT_PATH"))
                                {
                                    if (testset["DS_ATTACHMENT"] != null)
                                    {
                                        if (testset["DS_ATTACHMENT"] == "Y")
                                            row[item.Value] = DwnldDesignAttchmntsAndPathByTestId(testset["TS_TEST_ID"].ToString());
                                    }
                                }
                                else if (testset[item.Key] == null)
                                    row[item.Value] = DBNull.Value;
                                else if (item.Key.Contains("TS_SUBJECT"))
                                    row[item.Value] = GetFolderPath(Convert.ToInt32(Utilities.CleanInvalidXmlChars(testset["AL_ITEM_ID"])));
                                else if (item.Key.Contains("TS_CREATION_DATE") || item.Key.Contains("TS_VC_CHECKIN_DATE") || item.Key.Contains("TS_VC_DATE") || item.Key.Contains("TS_VTS"))
                                    row[item.Value] = Convert.ToDateTime(Utilities.CleanInvalidXmlChars(testset[item.Key]));
                                else if (item.Key.Contains("DS_STEP_NAME") || item.Key.Contains("DS_DESCRIPTION") || item.Key.Contains("DS_EXPECTED"))
                                    row[item.Value] = Utilities.CleanInvalidXmlChars(testset[item.Key]);
                                else if (item.Key.Contains("DS_ID"))
                                    row[item.Value] = Utilities.CleanInvalidXmlChars(testset[item.Key]);
                                else
                                    row[item.Value] = Utilities.CleanInvalidXmlChars(testset[item.Key]);
                            }
                        }
                        dstALMTable.Rows.Add(row);
                        testset.Next();
                    }
                }
                else
                    Utilities.LogError("No data found in datatable for Jira export");

                return dstALMTable;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Geth the folder Path
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetFolderPath(int allListItemId)
        {
            if (allListItemId > 0)
            {
                TreeManager trmng = otaConnection.TreeManager;
                //Replacing the string path to forward slash as per Jira folder
                return "/" + trmng.get_NodePath(allListItemId).Replace("\\", "/");
            }
            else
                return "/Subject/Unattached";
        }

        /// <summary>
        /// Verify Valid Mandatory fields
        /// </summary>
        /// <returns></returns>
        private bool IsValidInput()
        {
            try
            {
                ClearErrorLabel();

                if (string.IsNullOrEmpty(txtJiraUrl.Value) || string.IsNullOrEmpty(txtUserName.Value) ||
                    string.IsNullOrEmpty(txtPWD.Value) || string.IsNullOrEmpty(txtProject.Value) ||
                    string.IsNullOrEmpty(txtEmail.Value) ||
                    string.IsNullOrEmpty(txtAttachmentPath.Value))

                {

                    lblTextError.Visible = true;
                    lblTextError.Value = "Please enter all the mandatory text field";
                    return true;
                }
                else if (!string.IsNullOrEmpty(txtEmail.Value))
                {
                    bool isValid = false;
                    string address = txtEmail.Value;
                    if (txtEmail.Value.Contains(","))
                    {
                        string[] userEmails = address.Trim().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string emailaddress in userEmails)
                        {
                            if (Utilities.IsValidEmailFormat(emailaddress))
                                isValid = emailaddress.ToUpper().Trim().Contains("JDA");
                        }
                    }
                    else if (txtEmail.Value.Contains(";"))
                    {
                        string[] userEmails = address.Trim().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string emailaddress in userEmails)
                        {
                            if (Utilities.IsValidEmailFormat(emailaddress))
                                isValid = emailaddress.ToUpper().Trim().Contains("JDA");
                        }
                    }
                    else
                        isValid = address.ToUpper().Trim().Contains("JDA");

                    if (!isValid)
                    {
                        lblTextError.Visible = true;
                        lblTextError.Value = "Only JDA Email Id's Accepted. Please Enter Valid Jda Email Id";
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = "Exception occurred while veriying Email and all mandatory fields. Retry again with Valid entries.";
                return true;
            }
        }

        /// <summary>
        /// Verify valid Jira Login
        /// </summary>
        /// <returns></returns>
        private bool IsValidLogin()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtUserName.Value) && !string.IsNullOrEmpty(txtPWD.Value) && !string.IsNullOrEmpty(txtJiraUrl.Value)) // Validating JDA User Jira credentials
                {
                    jiraClient = new Jira();
                    jiraClient.Connect(txtJiraUrl.Value.Trim(), txtUserName.Value.Trim(), txtPWD.Value.Trim());

                    if (jiraClient == null)
                    {
                        lblTextError.Visible = true;
                        lblTextError.Value = "Jira Server Not availiable OR Login to Jira Failed .Retry with valid credentials.";
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = "Jira Server Not availiable OR Login to Jira Failed .Retry with valid credentials.";
                return true;
            }
        }

        /// <summary>
        /// Verify Valid attachment path
        /// </summary>
        /// <returns></returns>
        private bool IsValidAttachment()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtAttachmentPath.Value))
                {
                    string strDirectory = txtAttachmentPath.Value;
                    if (!Directory.Exists(strDirectory))
                    {
                        lblTextError.Visible = true;
                        lblTextError.Value = "Ivalid path. Attachment directory path does not exist. Retry with valid attachment directory path.";
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                lblTextError.Visible = true;
                lblTextError.Value = "Please Provide the correct path. Attachment directory path does not exist.";
                return true;
            }
        }
        private void ClearErrorLabel()
        {
            lblTextError.Value = string.Empty;
            lblTextError.Visible = false;
        }
        private void UnclearErrorLabel()
        {
            lblTextError.Visible = true;
            lblTextError.Value = string.Empty;
        }

        #endregion
    }
}