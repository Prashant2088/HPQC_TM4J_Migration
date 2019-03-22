using System;
using System.Collections.Generic;
using System.IO;
using TDAPIOLELib;


namespace ALM_EXTRACT
{
    public class DesignSteps
    {
        /// <summary>
        /// TDAPIOLELib.TDConnection Object for the current ALM Connection
        /// </summary>
        private TDConnection tDConnection;

        /// <summary>
        /// Creates Helper Test Class Object
        /// </summary>
        /// <param name="OALMConnection">Pass TDConnection object to create the Test Object.</param>
        public DesignSteps(TDConnection OALMConnection)
        {
            this.tDConnection = OALMConnection;
        }

        /// <summary>
        /// Get All Design Steps by Design Id
        /// </summary>
        /// <param name="testId"></param>
        /// <returns></returns>
        public Recordset GetDesignStepswithTestID(int testId)
        {
            Recordset designSteps = Utilities.ExecuteQuery("Select * from DESSTEPS where DS_TEST_ID = " + testId, tDConnection);
            designSteps.First();
            return designSteps;
        }

        /// <summary>
        /// Get All  Design Steps
        /// </summary>
        /// <returns></returns>
        public Recordset GetAllDesignSteps()
        {
            Recordset designSteps = Utilities.ExecuteQuery("Select * from DESSTEPS", tDConnection);
            designSteps.First();
            return designSteps;
        }

        /// <summary>
        /// Download Design Steps Attachments (If any) by Design ID 
        /// </summary>
        /// <param name="DS_TEST_ID">Test Id</param>
        /// <param name="AttachmentDownloadPath"></param>
        /// <returns></returns>
        public string DownloadDesignStepsAttachmentsWithTestID(String TS_TEST_ID, String AttachmentDownloadPath)
        {
            TestFactory OTestFactory = tDConnection.TestFactory;
            TDFilter OTDFilter = OTestFactory.Filter;
            List OTestList;
            string strProjectName = tDConnection.ProjectName;
            string strDomainName = tDConnection.DomainName;
            string strAttachmentPath = string.Empty;
            AttachmentFactory OAttachmentFactory;
            ExtendedStorage OExtendedStorage;

            DesignStepFactory ODesignStepFactory;
            Test OTest;

            try
            {
                //Check if the directory exists
                string strfirstLevelPath = string.Format(AttachmentDownloadPath + "\\" + "{0}-{1}-DESIGN_STEPS_ATTACHMENT", strDomainName, strProjectName);
                if (!Directory.Exists(strfirstLevelPath))
                    Directory.CreateDirectory(strfirstLevelPath);

                OTDFilter["TS_TEST_ID"] = TS_TEST_ID;

                OTestList = OTestFactory.NewList(OTDFilter.Text);

                int StepCounter = OTestFactory.NewList(OTDFilter.Text).Count + 1;

                if (OTestList != null && OTestList.Count == 1)
                {
                    OTest = OTestList[1];

                    ODesignStepFactory = OTest.DesignStepFactory;

                    foreach (DesignStep ODesignStep in ODesignStepFactory.NewList(""))
                    {
                        if (ODesignStep.HasAttachment)
                        {
                            strAttachmentPath = string.Format(AttachmentDownloadPath + "\\" + "{0}-{1}-DESIGN_STEPS_ATTACHMENT" + "\\" + "{2}\\", strDomainName, strProjectName, TS_TEST_ID);
                            if (!Directory.Exists(strAttachmentPath))
                                Directory.CreateDirectory(strAttachmentPath);

                            OAttachmentFactory = ODesignStep.Attachments;

                            //Download the Design Step Attachments
                            foreach (Attachment OAttachment in OAttachmentFactory.NewList(""))
                            {
                                OExtendedStorage = OAttachment.AttachmentStorage;
                                OExtendedStorage.ClientPath = strAttachmentPath;
                                OAttachment.Load(true, OAttachment.Name);
                            }
                        }
                    }
                }

                return strAttachmentPath;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                return strAttachmentPath;
            }

        }

        /// <summary>
        /// Download design steps attachments
        /// </summary>
        /// <param name="AttachmentDownloadPath"></param>
        /// <returns></returns>
        public Boolean DownloadDesignStepsAttachments(String AttachmentDownloadPath)
        {
            TestFactory OTestFactory = tDConnection.TestFactory;
            TDFilter OTDFilter = OTestFactory.Filter;
            List OTestList;

            AttachmentFactory OAttachmentFactory;
            ExtendedStorage OExtendedStorage;
            DesignStepFactory ODesignStepFactory;
            Test OTest;

            Utilities.LogVerbose("Downloading design attachments for the Tests : " + AttachmentDownloadPath);
            try
            {
                //Check if the directory exists
                if (!Directory.Exists(AttachmentDownloadPath))
                    return false;

                OTDFilter["DS_ATTACHMENT"] = "Y";

                OTestList = OTestFactory.NewList(OTDFilter.Text);

                if (OTestList != null && OTestList.Count == 1)
                {
                    OTest = OTestList[1];

                    ODesignStepFactory = OTest.DesignStepFactory;

                    foreach (DesignStep ODesignStep in ODesignStepFactory.NewList(""))
                    {
                        if (ODesignStep.HasAttachment)
                        {
                            OAttachmentFactory = ODesignStep.Attachments;

                            //Download the Design Step Attachments
                            foreach (Attachment OAttachment in OAttachmentFactory.NewList(""))
                            {
                                OExtendedStorage = OAttachment.AttachmentStorage;
                                OExtendedStorage.ClientPath = AttachmentDownloadPath;
                                OAttachment.Load(true, AttachmentDownloadPath + "\\" + OAttachment.Name);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                return false;
            }

        }

    }
}