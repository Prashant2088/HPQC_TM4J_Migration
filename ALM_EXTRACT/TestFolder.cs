using TDAPIOLELib;
using System;
using System.Collections.Generic;

namespace ALM_EXTRACT
{
    public class TestFolder
    {
        /// <summary>
        /// TDAPIOLELib.TDConnection Object for the current ALM Connection
        /// </summary>
        private TDConnection tDConnection;

        /// <summary>
        /// Creates Helper Test Class Object
        /// </summary>
        /// <param name="OALMConnection">Pass TDConnection object to create the Test Object.</param>
        public TestFolder(TDConnection OALMConnection)
        {
            this.tDConnection = OALMConnection;
        }

        public SysTreeNode GetNodeObject(String folderPath)
        {
            
            TreeManager OTManager = tDConnection.TreeManager;
            return OTManager.NodeByPath[folderPath];
        }

        //string folderPath = @"C:\Users\1019884\Desktop\HP ALM";
        public List<String> GetChildFolderNames(String folderPath)
        {
            List<String> OFNames = new List<string>();
            SysTreeNode OSysTreeNode = GetNodeObject(folderPath);

            for (int Counter = 1; Counter <= OSysTreeNode.Count; Counter++)
                OFNames.Add(OSysTreeNode.Child[Counter].Name);

            return OFNames;
        }


        /// <summary>
        /// Get Test oject by Folder Path : @"Subject\Reporting"
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public List GetTestSetByFolderPath(String folderPath)
        {
            TreeManager OTManager = tDConnection.TreeManager;
            var OTFolder = OTManager.get_NodeByPath(folderPath);
            TestFactory OTFactory = OTFolder.TestFactory;

            //var subjNodeList = testNode.FindChildren("", false, "");
            //    foreach (var element in subjNodeList)

            return OTFactory.NewList("");
        }

        private TestFactory addSubjectTreeStructure(String subjectField)
        {
            String folderRootString = subjectField;
            folderRootString = folderRootString.Replace("\\", "/");
            String[] folders = folderRootString.Split('/');

            // Test Plan Tree Manager
            TreeManager treeMgr = tDConnection.TreeManager;
            SubjectNode subjectNode = treeMgr.get_NodeByPath("Subject");

            ISysTreeNode node = (ISysTreeNode)subjectNode;

            // Creating the folders in test plan.
            for (int i = 0; i < folders.Length; i++)
            {
                try
                {
                    node = node.FindChildNode(folders[i]);
                }
                catch (Exception ex)
                {
                    node = node.AddNode(folders[i]);
                    Console.WriteLine(ex.Message + ".\nChild not found. Adding new node: " + folders[i]);
                }
            }

            // Set the leaf folder and then returning the TestFactory from where all test cases will be generated from.
            SubjectNode folder = treeMgr.get_NodeById(node.NodeID);
            return folder.TestFactory;
        }
    }
}