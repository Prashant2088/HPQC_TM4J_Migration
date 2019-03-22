using System;
using System.Collections.Generic;
using TDAPIOLELib;

namespace ALM_EXTRACT
{
    public class TestPlan
    {
        /// <summary>
        /// TDAPIOLELib.TDConnection Object for the current ALM Connection
        /// </summary>
        private TDConnection tDConnection;

        /// <summary>
        /// Creates Helper Test Class Object
        /// </summary>
        /// <param name="OALMConnection">Pass TDConnection object to create the Test Object.</param>
        public TestPlan(TDConnection OALMConnection)
        {
            this.tDConnection = OALMConnection;
        }

        public Recordset GetTestByIds(string strTestIds)
        {
            try
            {
                string sql = string.Format("SELECT TEST.* , ALL_LISTS.AL_ITEM_ID, DESSTEPS.DS_ID, DESSTEPS.DS_STEP_NAME ,DESSTEPS.DS_DESCRIPTION , DESSTEPS.DS_EXPECTED , DESSTEPS.DS_ATTACHMENT  FROM TEST " +
                "LEFT JOIN DESSTEPS ON TEST.TS_TEST_ID = DESSTEPS.DS_TEST_ID " +
                "LEFT JOIN ALL_LISTS ON TEST.TS_SUBJECT = ALL_LISTS.AL_ITEM_ID " +
                "where TEST.TS_TEST_ID IN ({0}) ORDER BY  TEST.TS_TEST_ID , DESSTEPS.DS_ID", strTestIds);

                Recordset recordset = Utilities.ExecuteQuery(sql, tDConnection);
                recordset.First();
                return recordset;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get All tests & Design Sets object under TestPlan
        /// </summary>
        /// <returns></returns>
        public Recordset GetAllTestsAndDesignSteps()
        {
            try
            {
                string sql = "SELECT TEST.* , ALL_LISTS.AL_ITEM_ID, DESSTEPS.DS_ID, DESSTEPS.DS_STEP_NAME ,DESSTEPS.DS_DESCRIPTION , DESSTEPS.DS_EXPECTED , DESSTEPS.DS_ATTACHMENT  FROM TEST " +
                "LEFT JOIN DESSTEPS ON TEST.TS_TEST_ID = DESSTEPS.DS_TEST_ID " +
                "LEFT JOIN ALL_LISTS ON TEST.TS_SUBJECT = ALL_LISTS.AL_ITEM_ID " +
                "ORDER BY  TEST.TS_TEST_ID , DESSTEPS.DS_ID";

                Recordset recordset = Utilities.ExecuteQuery(sql, tDConnection);
                recordset.First();
                return recordset;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Finds the list of tests & Design Steps under unattached folder
        /// <para/>returns TDAPIOLELib.List Object. Each item from this list can be converted to TDAPIOLELib.Test Object
        /// </summary>
        /// <returns>TDAPIOLELib.List Object</returns>
        public Recordset GetAllUnattachedTestsAndDesignSteps()
        {
            try
            {
                string sql = "SELECT TEST.* , ALL_LISTS.AL_ITEM_ID, DESSTEPS.DS_ID, DESSTEPS.DS_STEP_NAME ,DESSTEPS.DS_DESCRIPTION , DESSTEPS.DS_EXPECTED , DESSTEPS.DS_ATTACHMENT  FROM TEST " +
               "LEFT JOIN DESSTEPS ON TEST.TS_TEST_ID = DESSTEPS.DS_TEST_ID " +
               "LEFT JOIN ALL_LISTS ON TEST.TS_SUBJECT = ALL_LISTS.AL_ITEM_ID " +
               "where TEST.TS_SUBJECT = -2 ORDER BY  TEST.TS_TEST_ID , DESSTEPS.DS_ID";

                Recordset recordset = Utilities.ExecuteQuery(sql, tDConnection);
                recordset.First();
                return recordset;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get All tests set object under TestPlan
        /// </summary>
        /// <returns></returns>
        public Recordset GetAllTests()
        {
            try
            {
                Recordset recordset = Utilities.ExecuteQuery("Select * from  Test", tDConnection);
                recordset.First();
                return recordset;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }
        /// <summary>
        /// Gel All the Test Ids from Test Plan
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllTestId()
        {
            try
            {
                Recordset recordset = Utilities.ExecuteQuery("Select TS_TEST_ID from Test", tDConnection);
                return Utilities.GetRecordetIterationsint(recordset, "TS_TEST_ID");
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Gel All the Test Name from Test Plan
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllTestName()
        {
            try
            {
                Recordset recordset = Utilities.ExecuteQuery("Select TS_NAME from  Test", tDConnection);
                return Utilities.GetRecordetIterationsString(recordset, "TS_NAME");
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Finds the list of tests under unattached folder
        /// <para/>returns TDAPIOLELib.List Object. Each item from this list can be converted to TDAPIOLELib.Test Object
        /// </summary>
        /// <returns>TDAPIOLELib.List Object</returns>
        public Recordset GetAllUnattachedTests()
        {
            try
            {
                Recordset recordset = Utilities.ExecuteQuery("Select * from test where TS_SUBJECT = -2", tDConnection);
                recordset.First();
                return recordset;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }


        /// <summary>
        /// Gel All distinct Test Ids from Test Plan : Unattached
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllDistinctUnattachedTestId()
        {
            try
            {
                Recordset recordset = Utilities.ExecuteQuery("Select DISTINCT TS_TEST_ID from test where TS_SUBJECT = -2 order by TS_TEST_ID", tDConnection);
                return Utilities.GetRecordetIterationsint(recordset, "TS_TEST_ID");
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Gel All distinct Test Ids from Test Plan : 
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllDistinctTestId()
        {
            try
            {
                Recordset recordset = Utilities.ExecuteQuery("Select DISTINCT TS_TEST_ID from test order by TS_TEST_ID", tDConnection);
                return Utilities.GetRecordetIterationsint(recordset, "TS_TEST_ID");
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Gel All the Test names from Test Plan : Unattached
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllUnattachedTestName()
        {
            try
            {
                Recordset recordset = Utilities.ExecuteQuery("Select TS_NAME from test where TS_SUBJECT = -2", tDConnection);
                return Utilities.GetRecordetIterationsString(recordset, "TS_NAME");
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Check How many test counts in the OTA (Including Unattahhed)
        /// </summary>
        /// <returns></returns>
        public int CountAllTests()
        {
            try
            {
                return GetAllDistinctTestId().Count;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Check How many test counts in the OTA (Unattached)
        /// </summary>
        /// <returns></returns>
        public int CountAllUnAttachedTests()
        {
            try
            {
                Recordset ORecordSet = Utilities.ExecuteQuery("Select Count(*) from Test where TS_SUBJECT = -2", tDConnection);
                ORecordSet.First();
                return Convert.ToInt32(ORecordSet[0]);
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get SYSTEM_FIELD from Project Entities for Test Table
        /// </summary>
        /// <returns></returns>
        public Recordset GetSystemFieldDetailsForTest()
        {
            try
            {
                Recordset recordset = Utilities.ExecuteQuery("SELECT SF_COLUMN_NAME, SF_USER_LABEL FROM SYSTEM_FIELD where SF_TABLE_NAME='TEST' and SF_IS_ACTIVE ='Y' order by SF_COLUMN_NAME", tDConnection);
                recordset.First();
                return recordset;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get SYSTEM_FIELD from Project Entities for Design Steps Table
        /// </summary>
        /// <returns></returns>
        public Recordset GetSystemFieldDetailsForDesignSteps()
        {
            try
            {
                Recordset recordset = Utilities.ExecuteQuery("SELECT SF_COLUMN_NAME, SF_USER_LABEL FROM SYSTEM_FIELD where SF_TABLE_NAME='DESSTEPS' and SF_IS_ACTIVE ='Y' order by SF_COLUMN_NAME", tDConnection);
                recordset.First();
                return recordset;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get the Test Object by Test Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Test GetTestObjectWithID(int id)
        {
            TestFactory OTestFactory = tDConnection.TestFactory as TestFactory;
            TDFilter OTDFilter = OTestFactory.Filter as TDFilter;
            List OTestList;

            Test OTest;

            try
            {
                OTDFilter["TS_TEST_ID"] = Convert.ToString(id);
                OTestList = OTestFactory.NewList(OTDFilter.Text);

                if (OTestList != null && OTestList.Count == 1)
                {
                    OTest = OTestList[1];
                    return OTest;
                }
                else
                {
                    throw (new Exception("Unable to find test with ID : " + id));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}