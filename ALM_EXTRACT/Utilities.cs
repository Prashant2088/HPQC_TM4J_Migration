using System;
using TDAPIOLELib;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.Configuration;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;
using System.Globalization;

namespace ALM_EXTRACT
{
    static class Utilities
    {
        private static Regex _regex = new Regex(
            @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            RegexOptions.CultureInvariant | RegexOptions.Singleline);

        /// <summary>
        /// Executes query on ALM Database
        /// </summary>
        /// <param name="QueryToExecute">Query to execute</param>
        /// <param name="tDConnection">TDAPIOLELib.TDConnection Object with active ALM Connection</param>
        /// <returns>TDAPIOLELib.Recordset Object</returns>
        public static Recordset ExecuteQuery(String QueryToExecute, TDConnection tDConnection)
        {
            try
            {
                if (!(QueryToExecute.Trim().ToUpper().StartsWith("SELECT")))
                    throw (new Exception("Only Select Query can be executed using this funtion"));

                Command OCommand = (Command)tDConnection.Command;
                OCommand.CommandText = QueryToExecute;
                return (Recordset)OCommand.Execute();
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message.ToString()));
            }
        }

        /// <summary>
        /// Remove All Unnessary Null Columns
        /// </summary>
        /// <param name="dtable"></param>
        public static DataTable RemoveUnusedColumns(DataTable dtable)
        {
            foreach (var column in dtable.Columns.Cast<DataColumn>().ToArray())
            {
                if (dtable.AsEnumerable().All(dr => dr.IsNull(column)))
                    dtable.Columns.Remove(column);
            }
            dtable.AcceptChanges();
            return dtable;
        }

        /// <summary>
        /// Get value from record set after iteration for integer
        /// </summary>
        /// <param name="rset"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static List<int> GetRecordetIterationsint(Recordset rset, string columnName)
        {
            List<int> lstDetails = new List<int>();
            rset.First();
            for (int Counter = 1; Counter <= rset.RecordCount; Counter++)
            {
                lstDetails.Add(Convert.ToInt32(rset[columnName]));
                rset.Next();
            }
            return lstDetails;
        }

        /// <summary>
        /// Get value from record set after iteration for String
        /// </summary>
        /// <param name="rset"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static List<string> GetRecordetIterationsString(Recordset rset, string columnName)
        {
            List<string> lstDetails = new List<string>();
            rset.First();
            for (int Counter = 1; Counter <= rset.RecordCount; Counter++)
            {
                lstDetails.Add(Convert.ToString(rset[columnName]));
                rset.Next();
            }
            return lstDetails;
        }

        /// <summary>
        /// Log all exceptions in the Utility
        /// </summary>
        /// <param name="ex">Exception message</param>
        public static void LogException(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("********************" + " Exception Log Start- " + DateTime.Now + "*********************");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Exception Type : " + ex.GetType().Name);
            sb.Append(Environment.NewLine);
            sb.Append("Error Message : " + ex.Message);
            sb.Append(Environment.NewLine);
            sb.Append("Error Source : " + ex.Source);
            sb.Append(Environment.NewLine);
            if (ex.StackTrace != null)
            {
                sb.Append("Error Trace : " + ex.StackTrace);
            }
            Exception innerEx = ex.InnerException;

            while (innerEx != null)
            {
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append("Exception Type : " + innerEx.GetType().Name);
                sb.Append(Environment.NewLine);
                sb.Append("Error Message : " + innerEx.Message);
                sb.Append(Environment.NewLine);
                sb.Append("Error Source : " + innerEx.Source);
                sb.Append(Environment.NewLine);
                if (ex.StackTrace != null)
                {
                    sb.Append("Error Trace : " + innerEx.StackTrace);
                }
                innerEx = innerEx.InnerException;
            }
            sb.Append("********************" + " Exception Log End- " + DateTime.Now + "*********************");

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\ExceptionLog";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = string.Format(folderPath + "\\" + "ExceptionLog-{0}.txt", DateTime.Today.ToString("dd-MM-yy"));

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine(sb.ToString());
                writer.Flush();
                writer.Close();
            }
        }

        /// <summary>
        /// Log all Error in the Utility
        /// </summary>
        /// <param name="msgLog">Error Message</param>
        public static void LogError(string msgLog)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("********************" + " Error Log start - " + DateTime.Now + "*********************");
            sb.Append(Environment.NewLine);
            sb.Append(msgLog);
            sb.Append(Environment.NewLine);
            sb.Append("********************" + " Error Log End- " + DateTime.Now + "*********************");

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\ErrorLog";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = string.Format(folderPath + "\\" + "ErrorLog-{0}.txt", DateTime.Today.ToString("dd-MM-yy"));

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine(sb.ToString());
                writer.Flush();
                writer.Close();
            }
        }

        /// <summary>
        /// Verbose Logs
        /// </summary>
        /// <param name="msgLog">Verbose messages</param>
        public static void LogVerbose(string msgLog)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("********************" + " Verbose Log start - " + DateTime.Now + "*********************");
            sb.Append(Environment.NewLine);
            sb.Append(msgLog);
            sb.Append(Environment.NewLine);
            sb.Append("********************" + " Verbose Log End- " + DateTime.Now + "*********************");

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\VerboseLog";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = string.Format(folderPath + "\\" + "VerboseLog-{0}.txt", DateTime.Today.ToString("dd-MM-yy"));

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine(sb.ToString());
                writer.Flush();
                writer.Close();
            }
        }

        public static string PrintToExcel<T>(IEnumerable<T> o, string messageType)
        {
            ExcelPackage p = new ExcelPackage();
            ExcelWorksheet ws = p.Workbook.Worksheets.Add("Imported TestKey");

            ws.Cells["C1"].Value = string.Format(@"{0}-Imported Test Key List", messageType);

            ws.Cells["C1"].Style.Font.Size = 14;
            ws.Cells["C1"].Style.Font.Bold = true;

            ws.Cells["A2"].Value = "Test Case Id";
            ws.Cells["A2"].Style.Font.Size = 14;
            ws.Cells["A2"].Style.Font.Bold = true;

            using (var cells = ws.Cells["A3"])
            {
                cells.LoadFromCollection(o, false);
            }

            for (int i = 1; i <= typeof(T).GetProperties().Length; i++)
                ws.Column(i).AutoFit();

            string fileName = string.Format(@"{0}{1}_{2}_ImportedTestKeyList.xls", AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyyMMddHHmmssffff"), messageType);

            System.IO.File.WriteAllBytes(fileName, p.GetAsByteArray());
            return fileName;
        }
        public static void SendReport(string address, string reportName, string messageType, string lstCount, bool isDuplicate = false)
        {
            string subject;
            string body;

            if (!isDuplicate)
            {
                subject = string.Format(@"{0}-Test Case Imported successfully for the Project -{1}", lstCount, messageType);
                body = string.Format("Hi,<br/> Please find below the Successful Imported Test Id created in Jira.<br/> {0}<br/> <br/>Thank you.", reportName);
            }
            else
            {
                subject = string.Format(@"{0}-Duplicate Test Case found for the Project -{1}", lstCount, messageType);
                body = string.Format("Hi,<br/> Please find below the Duplicate Test Case name found in Jira.<br/> {0}<br/> <br/>Thank you.", reportName);
            }

            string SenderAddress = WebConfigurationManager.AppSettings["SenderAddress"];

            if (address.Contains(","))
            {
                string[] userEmails = address.Trim().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string emailaddress in userEmails)
                    SendEmail(SenderAddress, emailaddress.Trim(), subject, body, string.Empty, "application/json");
            }
            else if (address.Contains(";"))
            {
                string[] userEmails = address.Trim().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string emailaddress in userEmails)
                    SendEmail(SenderAddress, emailaddress.Trim(), subject, body, string.Empty, "application/json");
            }
            else
                SendEmail(SenderAddress, address, subject, body, string.Empty, "application/json");
            //SendEmail(SenderAddress, address, subject, body, reportName, "application/vnd.ms-excel");
        }

        public static void SendErrorEmail(string address, string exceptionMessage, string messageType, string lstCount = "", bool IsError = false)
        {
            string subject;
            string body;
            string senderAddress = WebConfigurationManager.AppSettings["SenderAddress"];


            if (!IsError)
            {
                subject = string.Format(@"Importing Test Case Failed for the Project - {0}", messageType);
                body = String.Format("Hi,<br/> Error happened while importing test cases in Jira. Please do the Import again. Exception details are below <br/>{0}<br/>Thank you.", exceptionMessage);
            }
            else
            {
                subject = string.Format(@"{0}-Test Case failed to create in Jira due to Error for the Project -{1}", lstCount, messageType);
                body = string.Format("Hi,<br/> Please find below the Test Id whaich are failed to create in Jira.<br/> '{0}'<br/> <br/><br/> <br/>Please find the failed Test Report in the directory which you were given during Import. Thank you.", exceptionMessage);
            }

            if (address.Contains(","))
            {
                string[] userEmails = address.Trim().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string emailaddress in userEmails)
                    SendEmail(senderAddress, emailaddress.Trim(), subject, body, null, "");
            }
            else if (address.Contains(";"))
            {
                string[] userEmails = address.Trim().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string emailaddress in userEmails)
                    SendEmail(senderAddress, emailaddress.Trim(), subject, body, null, "");
            }
            else
                SendEmail(senderAddress, address, subject, body, null, "");

        }

        /// <summary>
        /// Send Emails to Utility Users
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attchmentFileName"></param>
        /// <param name="mediaType"></param>
        private static void SendEmail(string from, string to, string subject, string body, string attchmentFileName, string mediaType)
        {
            using (MailMessage message = new MailMessage())
            {
                string SenderAddress = WebConfigurationManager.AppSettings["SenderAddress"];

                message.Subject = subject;
                message.Body = body;
                message.To.Add(new MailAddress(to));
                message.From = new MailAddress(from);
                message.IsBodyHtml = true;

                if (!String.IsNullOrEmpty(attchmentFileName))
                    message.Attachments.Add(new System.Net.Mail.Attachment(attchmentFileName, mediaType));

                using (SmtpClient client = GetMailClient())
                {
                    client.Send(message);
                }
            }
        }

        private static SmtpClient GetMailClient()
        {
            string SmtpServer = WebConfigurationManager.AppSettings["SmtpServer"];
            int Port = int.Parse(WebConfigurationManager.AppSettings["Port"]);
            string User = WebConfigurationManager.AppSettings["User"];
            string Password = WebConfigurationManager.AppSettings["Password"];
            bool EnableSSL = Boolean.Parse(WebConfigurationManager.AppSettings["EnableSSL"]);

            var client = new SmtpClient(SmtpServer);
            client.EnableSsl = EnableSSL;

            if (!string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Password))
                client.Credentials = new NetworkCredential(User, Password);
            return client;
        }

        /// <summary>
        /// Return all the Distinct TestIds
        /// </summary>
        /// <param name="dstALMTable"></param>
        /// <returns></returns>
        public static List<string> IsDistinctTestID(DataTable dstALMTable)
        {
            return dstALMTable.AsEnumerable().Select(rowItem => rowItem.Field<string>("Test ID")).Distinct().ToList();
        }

        /// <summary>
        /// Get All dataRows by TestId
        /// </summary>
        /// <param name="dstALMTable"></param>
        /// <param name="strTestId"></param>
        /// <returns></returns>
        public static List<DataRow> GetDataRowsbyTestID(DataTable dstALMTable, string strTestId)
        {
            return dstALMTable.AsEnumerable().Where(x => x.Field<string>("Test ID") == strTestId).ToList();
        }

        /// <summary>
        /// If Duplicate row exist . We can have multiple design steps.
        /// </summary>
        /// <param name="dstALMTable">Data Table</param>
        /// <param name="strTestId">Test Id</param>
        /// <param name="strTestName">Test Name</param>
        /// <returns></returns>
        public static bool IsDuplicateExist(DataTable dstALMTable, string strTestId, string strTestName)
        {
            return dstALMTable.AsEnumerable().Any(rowItem => rowItem.Field<string>("Test ID") == strTestId && rowItem.Field<string>("Test Name") == strTestName);
        }

        public static bool IsValidEmailFormat(string emailInput)
        {
            bool isValidemail = false;
            if (emailInput.Contains(","))
            {
                string[] userEmails = emailInput.Trim().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string emailaddress in userEmails)
                    isValidemail = _regex.IsMatch(emailaddress.Trim());
            }
            else if (emailInput.Contains(";"))
            {
                string[] userEmails = emailInput.Trim().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string emailaddress in userEmails)
                    isValidemail = _regex.IsMatch(emailaddress.Trim());
            }
            else
                isValidemail = _regex.IsMatch(emailInput.Trim());

            return isValidemail;
        }

        /// <summary>
        /// Export the Data Table to excel Format
        /// </summary>
        /// <param name="dstALMTable">Data Table</param>
        public static void ExportDatatoExcel(DataTable dstALMTable, string txtAttachmentPath, string strProjectName, string strDomainName, string strFileName)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(dstALMTable);

                string strDwnloadpath = string.Format(txtAttachmentPath.Trim() + "\\{0}", strProjectName);
                if (!Directory.Exists(strDwnloadpath))
                    Directory.CreateDirectory(strDwnloadpath);

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(ds);
                    wb.ColumnWidth = 75;
                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Alignment.WrapText = true;
                    wb.Style.Font.Bold = true;
                    //Save the file in the directory
                    wb.SaveAs(string.Format(strDwnloadpath + "\\{0}_{1}_{2}.xlsx", strDomainName, strProjectName, strFileName));

                    //Response.Clear();
                    //Response.Buffer = true;
                    //Response.Charset = "";
                    //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", string.Format("attachment;filename={0}_{1}_Test_Plan_Recordset.xlsx", strDomainName, strProjectName));
                    //using (MemoryStream MyMemoryStream = new MemoryStream())
                    //{
                    //    wb.SaveAs(MyMemoryStream);
                    //    MyMemoryStream.WriteTo(Response.OutputStream);
                    //    Response.Flush();
                    //    Response.End();
                    //}
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        public static DataTable CreateDataTable(Dictionary<string, string> dctlstItem, string clmName1, string clmName2)
        {
            DataTable tempDataTable = new DataTable();

            tempDataTable.Columns.Add(clmName1, typeof(string));
            tempDataTable.Columns.Add(clmName2, typeof(string));
            foreach (var item in dctlstItem)
            {
                if (!IsDuplicateExist(tempDataTable, item.Key, item.Value, clmName1, clmName2))
                    tempDataTable.Rows.Add(item.Key, item.Value);
            }

            return tempDataTable;
        }

        /// <summary>
        /// Creating Data Table from the List
        /// </summary>
        /// <param name="lstItem"></param>
        /// <param name="clmName1"></param>
        /// <returns></returns>
        public static DataTable CreateDataTable_FromList(List<string> lstItem, string clmName1)
        {
            DataTable tempDataTable = new DataTable();
            if (lstItem != null)
            {
                if (lstItem.Count > 0)
                {
                    tempDataTable.Columns.Add(clmName1, typeof(string));

                    for (int lstcnt = 0; lstcnt < lstItem.Count; lstcnt++)
                    {
                        if (lstItem[lstcnt] != null)
                            tempDataTable.Rows.Add(lstItem[lstcnt].ToString().Trim());
                    }
                }
            }
            return tempDataTable;
        }

        /// <summary>
        /// Duplicacy check in the Data table based on the Column
        /// </summary>
        /// <param name="dstALMTable"></param>
        /// <param name="strTestId"></param>
        /// <param name="strTestName"></param>
        /// <param name="clmName1"></param>
        /// <param name="clmName2"></param>
        /// <returns></returns>
        private static bool IsDuplicateExist(DataTable dstALMTable, string strTestId, string strTestName, string clmName1, string clmName2)
        {
            return dstALMTable.AsEnumerable().Any(rowItem => rowItem.Field<string>(clmName1) == strTestId && rowItem.Field<string>(clmName2) == strTestName);
        }

        /// <summary>
        /// Read Excel File Data
        /// </summary>
        /// <param name="strFilePath">File Path</param>
        /// <param name="strFileName">File Name</param>
        /// <param name="lstrClmnName">Excel Column Name</param>
        /// <returns></returns>
        public static List<string> ReadExcelFile(string strFilePath, string strFileName, string lstrClmnName)
        {
            List<string> lstTestIds = new List<string>();

            try
            {
                if (!string.IsNullOrEmpty(strFilePath) && !string.IsNullOrEmpty(strFileName) && !string.IsNullOrEmpty(lstrClmnName))
                {
                    // Check For File Existence
                    string strAbsolutePath = string.Format(strFilePath + strFileName);

                    if (File.Exists(string.Format(strAbsolutePath + ".xlsx"))) // Check only Excel Formats
                    {
                        Application xlApp = new Application();
                        Range range;
                        int clmnIndex = 0;
                        Workbook xlWorkBook = xlApp.Workbooks.Open(strAbsolutePath, 0, true, 5, "", "", true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                        Worksheet xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
                        range = xlWorkSheet.UsedRange;

                        for (int clCnt = 1; clCnt <= range.Columns.Count; clCnt++)
                        {
                            if (!string.IsNullOrEmpty((string)(range.Cells[clCnt] as Range).Value2))
                            {
                                string strClmnValue = (string)(range.Cells[clCnt] as Range).Value2;
                                if (strClmnValue.ToUpper().Trim() == lstrClmnName.ToUpper().Trim())
                                {
                                    clmnIndex = clCnt;
                                    break;
                                }
                            }

                        }

                        if (clmnIndex > 0)
                        {
                            for (int rCnt = 2; rCnt <= range.Rows.Count; rCnt++)
                            {
                                if (!string.IsNullOrEmpty((string)(range.Cells[rCnt, clmnIndex] as Range).Value2))
                                {
                                    string strRowValue = (string)(range.Cells[rCnt, clmnIndex] as Range).Value2;
                                    lstTestIds.Add(strRowValue);
                                }
                            }
                        }

                        xlWorkBook.Close(true, null, null);
                        xlApp.Quit();

                        Marshal.ReleaseComObject(xlWorkSheet);
                        Marshal.ReleaseComObject(xlWorkBook);
                        Marshal.ReleaseComObject(xlApp);
                    }
                    else
                        LogVerbose(string.Format("Error Files Name : '{0}' not exist in the directory: '{1}'.Either all Test Case has Successfully imported in Jira or directory not exist.", strFileName, strFilePath));
                }
                else
                    LogVerbose("FilePath, FileName or Column Name is Empty or Not Found. Unable to Process.");

                return lstTestIds;
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw ex;
            }
        }
        public static string ParseExactDate(string strDatetime)
        {
            string strformatedDate = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(strDatetime))
                    strformatedDate = DateTime.ParseExact(strDatetime, "dd-MM-yyyy hh:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                else
                {
                    Utilities.LogVerbose("Dattime Input field is empty. Setting up with the Current dateTime value.");
                    strformatedDate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                return strformatedDate;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                Utilities.LogError(string.Format("Dattime Input '{0}' is not in the Correct format. Setting up with the Current dateTime value.", strDatetime));
                return DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// Parse the Dattime in Jira ISO Date Format
        /// </summary>
        /// <param name="strDatetime"></param>
        /// <returns></returns>
        public static string ParseDate(string strDatetime)
        {
            string strformatedDate = string.Empty;
            try
            {
                DateTime formatedDateValue;
                if (!string.IsNullOrEmpty(strDatetime))
                {
                    if (DateTime.TryParse(strDatetime, out formatedDateValue))
                        strformatedDate = formatedDateValue.ToString("yyyy-MM-dd");
                    else
                    {
                        Utilities.LogVerbose("Unable to Parse the input with DateTime. Setting up with the Current dateTime value.");
                        strformatedDate = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                }
                else
                {
                    Utilities.LogVerbose("Dattime Input field is empty. Setting up with the Current dateTime value.");
                    strformatedDate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                return strformatedDate;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                Utilities.LogError(string.Format("Dattime Input '{0}' is not in the Correct format. Setting up with the Current dateTime value.", strDatetime));
                return DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// For Removing Hexadecimal or Invalid Characters which are allowed by HPQC
        /// </summary>
        /// <param name="StrInput"> Input string</param>
        /// <returns></returns>
        public static string CleanInvalidXmlChars(string StrInput)
        {
            //Returns same value if the value is empty.
            if (string.IsNullOrWhiteSpace(StrInput))
            {
                return StrInput;
            }
            // From xml spec valid chars:
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]    
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF.
            string RegularExp = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(StrInput, RegularExp, String.Empty);
        }
    }
}