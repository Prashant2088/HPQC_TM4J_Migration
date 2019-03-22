using System;
using System.IO;
using TDAPIOLELib;

namespace ALM_EXTRACT
{
    public class TestAttachments
    {
        /// <summary>
        /// TDAPIOLELib.TDConnection Object for the current ALM Connection
        /// </summary>
        private TDConnection tDConnection;

        /// <summary>
        /// Creates Helper Test Class Object
        /// </summary>
        /// <param name="OALMConnection">Pass TDConnection object to create the Test Object.</param>
        public TestAttachments(TDConnection OALMConnection)
        {
            this.tDConnection = OALMConnection;
        }

        /// <summary>
        /// Download the Test Attachments
        /// </summary>
        /// <param name="AttachmentDownloadPath"></param>
        /// <returns></returns>
        public Boolean DownloadTestAttachments(String AttachmentDownloadPath)
        {
            TestFactory OTestFactory = tDConnection.TestFactory;
            TDFilter OTDFilter = OTestFactory.Filter;
            List OTestList;
            string strProjectName = tDConnection.ProjectName;
            string strDomainName = tDConnection.DomainName;
            AttachmentFactory OAttachmentFactory;
            ExtendedStorage OExtendedStorage;

            try
            {
                //Check if the directory exists
                string strfirstLevelPath = string.Format(AttachmentDownloadPath + "\\" + "{0}-{1}", strDomainName, strProjectName);
                if (!Directory.Exists(strfirstLevelPath))
                    Directory.CreateDirectory(strfirstLevelPath);

                OTDFilter["TS_ATTACHMENT"] = "Y";

                OTestList = OTestFactory.NewList(OTDFilter.Text);

                foreach (Test OTest in OTestList)
                {
                    if (OTest.HasAttachment)
                    {
                        string strAttachmentPath = string.Format(AttachmentDownloadPath + "\\" + "{0}-{1}" + "\\" + "{2}-{3}", strDomainName, strProjectName, OTest.ID.ToString(), OTest.Name.ToString());

                        if (!Directory.Exists(strAttachmentPath))
                            Directory.CreateDirectory(strAttachmentPath);

                        OAttachmentFactory = OTest.Attachments;

                        //Download the test attachments
                        foreach (Attachment OAttachment in OAttachmentFactory.NewList(""))
                        {
                            OExtendedStorage = OAttachment.AttachmentStorage;
                            OExtendedStorage.ClientPath = strAttachmentPath;
                            OAttachment.Load(true, OAttachment.Name);
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

        /// <summary>
        /// Down All Test Attachments from Test Id
        /// </summary>
        /// <param name="TS_TESTID"></param>
        /// <param name="AttachmentDownloadPath"></param>
        /// <returns></returns>
        public string DownloadTestAttachmentsWithTestID(String TS_TESTID, String AttachmentDownloadPath)
        {
            TestFactory OTestFactory = tDConnection.TestFactory;
            TDFilter OTDFilter = OTestFactory.Filter;
            List OTestList;
            AttachmentFactory OAttachmentFactory;
            ExtendedStorage OExtendedStorage;

            string strProjectName = tDConnection.ProjectName;
            string strDomainName = tDConnection.DomainName;
            string strAttachmentPath = string.Empty;

            try
            {
                //Check if the directory exists
                string strfirstLevelPath = string.Format(AttachmentDownloadPath + "\\" + "{0}-{1}", strDomainName, strProjectName);
                if (!Directory.Exists(strfirstLevelPath))
                    Directory.CreateDirectory(strfirstLevelPath);

                OTDFilter["TS_TEST_ID"] = TS_TESTID;

                OTestList = OTestFactory.NewList(OTDFilter.Text);

                foreach (Test OTest in OTestList)
                {
                    if (OTest.HasAttachment)
                    {
                        strAttachmentPath = string.Format(AttachmentDownloadPath + "\\" + "{0}-{1}" + "\\" + "{2}-{3}", strDomainName, strProjectName, OTest.ID.ToString(), OTest.Name.ToString());

                        if (!Directory.Exists(strAttachmentPath))
                            Directory.CreateDirectory(strAttachmentPath);

                        OAttachmentFactory = OTest.Attachments;

                        //Download the test attachments
                        foreach (Attachment OAttachment in OAttachmentFactory.NewList(""))
                        {
                            OExtendedStorage = OAttachment.AttachmentStorage;
                            OExtendedStorage.ClientPath = strAttachmentPath;
                            OAttachment.Load(true, OAttachment.Name);
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
    }
}