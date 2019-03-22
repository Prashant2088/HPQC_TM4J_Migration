using System;
using Newtonsoft.Json;

namespace ALM_EXTRACT.JiraAPI
{
    public class CreateTestCase
    {
        [JsonProperty(propertyName: "projectKey")]
        public string ProjectKey { get; set; }

        /// <summary>
        /// If you dont want to serialize the jason like in Update we 
        /// don't need the Project Key
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeProjectKey()
        {
            return (!string.IsNullOrEmpty(ProjectKey));
        }

        //HPQC- TS_NAME
        [JsonProperty(propertyName: "name")]
        public String TestName { get; set; }

        public bool ShouldSerializeTestName()
        {
            return (!string.IsNullOrEmpty(TestName));
        }

        //HPQC - TS_RESPONSIBLE
        [JsonProperty(propertyName: "owner")]
        public String Owner { get; set; }

        public bool ShouldSerializeOwner()
        {
            return (!string.IsNullOrEmpty(Owner));
        }

        //HPQC- TS_STATUS
        [JsonProperty(propertyName: "status")]
        public String Status { get; set; }

        public bool ShouldSerializeStatus()
        {
            return (!string.IsNullOrEmpty(Status));
        }

        //HPQC- TS_SUBJECT
        [JsonProperty(propertyName: "folder")]
        public String Folder { get; set; }

        public bool ShouldSerializeFolder()
        {
            return (!string.IsNullOrEmpty(Folder));
        }




        [JsonProperty(propertyName: "precondition")]
        public String PreCondition { get; set; }

        public bool ShouldSerializePreCondition()
        {
            return (!string.IsNullOrEmpty(PreCondition));
        }

        [JsonProperty(propertyName: "objective")]
        public String Objective { get; set; }

        public bool ShouldSerializeObjective()
        {
            return (!string.IsNullOrEmpty(Objective));
        }

        [JsonProperty(propertyName: "priority")]
        public String Priority { get; set; }

        public bool ShouldSerializePriority()
        {
            return (!string.IsNullOrEmpty(Priority));
        }

        [JsonProperty(propertyName: "component")]
        public String Component { get; set; }

        public bool ShouldSerializeComponent()
        {
            return (!string.IsNullOrEmpty(Component));
        }

        [JsonProperty(propertyName: "estimatedTime")]
        public String EstimatedTime { get; set; }

        public bool ShouldSerializeEstimatedTime()
        {
            return (!string.IsNullOrEmpty(EstimatedTime));
        }

        [JsonProperty(propertyName: "labels")]
        public String[] Labels { get; set; }

        public bool ShouldSerializeLabels()
        {
            return (Labels != null);
        }

        //HPQC- DESSTEPS
        [JsonProperty(propertyName: "testScript")]
        public TestScript TestScript { get; set; }

        public bool ShouldSerializeTestScript()
        {
            return (TestScript != null);
        }

        //HPQC - CUSTOM FIELDS
        [JsonProperty(propertyName: "customFields")]
        public customFields CustomFields { get; set; }

        public bool ShouldSerializeCustomFields()
        {
            return (CustomFields != null);
        }

    }

    public class customFields
    {
        //HPQC - TS_BPTA_CHANGE_DETECTED
        [JsonProperty(propertyName: "change status")]
        public String ChangeStatus { get; set; }

        public bool ShouldSerializeChangeStatus()
        {
            return (!string.IsNullOrEmpty(ChangeStatus));
        }

        //HPQC- TS_CREATION_DATE
        [JsonProperty(propertyName: "Creation Date")]
        public String CreationDate { get; set; }

        public bool ShouldSerializeCreationDate()
        {
            return (!string.IsNullOrEmpty(CreationDate));
        }

        //HPQC - TS_DESCRIPTION
        [JsonProperty(propertyName: "Test Name Description")]
        public String TestNameDescription { get; set; }

        public bool ShouldSerializeTestNameDescription()
        {
            return (!string.IsNullOrEmpty(TestNameDescription));
        }

        //HPQC- TS_DEV_COMMENTS
        [JsonProperty(propertyName: "comments")]
        public String Comments { get; set; }

        public bool ShouldSerializeComments()
        {
            return (!string.IsNullOrEmpty(Comments));
        }

        //HPQC- TS_ESTIMATE_DEVTIME
        [JsonProperty(propertyName: "estimated DevTime")]
        public String EstimatedDevTime { get; set; }

        public bool ShouldSerializeEstimatedDevTime()
        {
            return (!string.IsNullOrEmpty(EstimatedDevTime));
        }

        //HPQC- TS_EXEC_STATUS
        [JsonProperty(propertyName: "execution status")]
        public String ExecutionStatus { get; set; }

        public bool ShouldSerializeExecutionStatus()
        {
            return (!string.IsNullOrEmpty(ExecutionStatus));
        }

        //HPQC- TS_PATH
        [JsonProperty(propertyName: "path")]
        public String Path { get; set; }

        public bool ShouldSerializePath()
        {
            return (!string.IsNullOrEmpty(Path));
        }

        //HPQC- TS_PROTOCOL_TYPE
        [JsonProperty(propertyName: "protocol type")]
        public String ProtocolType { get; set; }

        public bool ShouldSerializeProtocolType()
        {
            return (!string.IsNullOrEmpty(ProtocolType));
        }

        //HPQC- TS_SERVICE_TEST_MODE
        [JsonProperty(propertyName: "testing mode")]
        public String TestingMode { get; set; }

        public bool ShouldSerializeTestingMode()
        {
            return (!string.IsNullOrEmpty(TestingMode));
        }

        //HPQC- TS_TEMPLATE
        [JsonProperty(propertyName: "template")]
        public String Template { get; set; }

        public bool ShouldSerializeTemplate()
        {
            return (!string.IsNullOrEmpty(Template));
        }

        //HPQC- TS_TEST_ID
        [JsonProperty(propertyName: "Test ID")]
        public string TestId { get; set; }

        public bool ShouldSerializeTestId()
        {
            return (!string.IsNullOrEmpty(TestId));
        }

        //HPQC- TS_TREE_PATH
        [JsonProperty(propertyName: "tree path")]
        public string TreePath { get; set; }

        public bool ShouldSerializeTreePath()
        {
            return (!string.IsNullOrEmpty(TreePath));
        }

        //HPQC -TS_TYPE
        [JsonProperty(propertyName: "type")]
        public String Type { get; set; }

        public bool ShouldSerializeType()
        {
            return (!string.IsNullOrEmpty(Type));
        }

        //HPQC -TS_WORKING_MODE
        [JsonProperty(propertyName: "working mode")]
        public String WorkingMode { get; set; }

        public bool ShouldSerializeWorkingMode()
        {
            return (!string.IsNullOrEmpty(WorkingMode));
        }

        //HPQC -TS_EXTENDED_DATA
        [JsonProperty(propertyName: "extended test data")]
        public String ExtendedTestData { get; set; }

        public bool ShouldSerializeExtendedTestData()
        {
            return (!string.IsNullOrEmpty(ExtendedTestData));
        }

        //HPQC -TS_PC_BLOB
        [JsonProperty(propertyName: "performance centers data XML")]

        public String PerformanceCentersDataXML { get; set; }

        public bool ShouldSerializePerformanceCentersDataXML()
        {
            return (!string.IsNullOrEmpty(PerformanceCentersDataXML));
        }

        //HPQC -TS_PC_DURATION
        [JsonProperty(propertyName: "total duration of test")]

        public String TotalDurationOfTest { get; set; }

        public bool ShouldSerializeTotalDurationOfTest()
        {
            return (!string.IsNullOrEmpty(TotalDurationOfTest));
        }

        //HPQC -TS_PC_ERRORS
        [JsonProperty(propertyName: "test internal errors")]

        public String TestInternalErrors { get; set; }

        public bool ShouldSerializeTestInternalErrors()
        {
            return (!string.IsNullOrEmpty(TestInternalErrors));
        }

        //HPQC -TS_PC_SLA_DEFINED
        [JsonProperty(propertyName: "SLA Defined")]

        public String SLADefined { get; set; }

        public bool ShouldSerializeSLADefined()
        {
            return (!string.IsNullOrEmpty(SLADefined));
        }

        //HPQC -TS_PC_TOTAL_VUSERS
        [JsonProperty(propertyName: "total amount of Vusers")]

        public String TotalVusers { get; set; }

        public bool ShouldSerializeTotalVusers()
        {
            return (!string.IsNullOrEmpty(TotalVusers));
        }

        //HPQC -TS_PC_VALID
        [JsonProperty(propertyName: "test validity")]

        public String TestValidity { get; set; }

        public bool ShouldSerializeTestValidity()
        {
            return (!string.IsNullOrEmpty(TestValidity));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "category")]

        public String Category { get; set; }

        public bool ShouldSerializeCategory()
        {
            return (!string.IsNullOrEmpty(Category));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Feature")]

        public String Feature { get; set; }

        public bool ShouldSerializeFeature()
        {
            return (!string.IsNullOrEmpty(Feature));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "jira ID")]

        public String JiraId { get; set; }

        public bool ShouldSerializeJiraId()
        {
            return (!string.IsNullOrEmpty(JiraId));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "origin")]

        public String Origin { get; set; }

        public bool ShouldSerializeOrigin()
        {
            return (!string.IsNullOrEmpty(Origin));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "story")]

        public String Story { get; set; }

        public bool ShouldSerializeStory()
        {
            return (!string.IsNullOrEmpty(Story));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "identified in release")]

        public String IdentifiedInRelease { get; set; }

        public bool ShouldSerializeIdentifiedInRelease()
        {
            return (!string.IsNullOrEmpty(IdentifiedInRelease));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "release")]

        public String Release { get; set; }

        public bool ShouldSerializeRelease()
        {
            return (!string.IsNullOrEmpty(Release));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "req num")]

        public String ReqNum { get; set; }

        public bool ShouldSerializeReqNum()
        {
            return (!string.IsNullOrEmpty(ReqNum));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "DS_STEP_NAME")]

        public String DS_STEP_NAME { get; set; }

        public bool ShouldSerializeDS_STEP_NAME()
        {
            return (!string.IsNullOrEmpty(DS_STEP_NAME));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "reviewer")]

        public String Reviewer { get; set; }

        public bool ShouldSerializeReviewer()
        {
            return (!string.IsNullOrEmpty(Reviewer));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "sprint")]

        public String Sprint { get; set; }

        public bool ShouldSerializeSprint()
        {
            return (!string.IsNullOrEmpty(Sprint));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "automated")]
        public string Automated { get; set; }

        public bool ShouldSerializeAutomated()
        {
            return (!string.IsNullOrEmpty(Automated));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "JDA Scenario ID")]
        public string JDAScenarioID { get; set; }

        public bool ShouldSerializeJDAScenarioID()
        {
            return (!string.IsNullOrEmpty(JDAScenarioID));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "JDA Fitnesse Status")]
        public string JDAFitnesseStatus { get; set; }

        public bool ShouldSerializeJDAFitnesseStatus()
        {
            return (!string.IsNullOrEmpty(JDAFitnesseStatus));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "JDA IF-Version")]
        public string JDA_IF_Version { get; set; }

        public bool ShouldSerializeJDA_IF_Version()
        {
            return (!string.IsNullOrEmpty(JDA_IF_Version));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Defect Id")]
        public string DefectId { get; set; }

        public bool ShouldSerializeDefectId()
        {
            return (!string.IsNullOrEmpty(DefectId));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "automation type")]
        public String Automation_Type { get; set; }

        public bool ShouldSerializeAutomation_Type()
        {
            return (!string.IsNullOrEmpty(Automation_Type));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Release Version")]
        public String ReleaseVersion { get; set; }

        public bool ShouldSerializeReleaseVersion()
        {
            return (!string.IsNullOrEmpty(ReleaseVersion));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "priority(legacy)")]
        public String Priority_Legacy { get; set; }

        public bool ShouldSerializePriority_Legacy()
        {
            return (!string.IsNullOrEmpty(Priority_Legacy));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "source_project")]
        public String Source_Project { get; set; }

        public bool ShouldSerializeSource_Project()
        {
            return (!string.IsNullOrEmpty(Source_Project));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Version")]
        public String Version { get; set; }

        public bool ShouldSerializeVersion()
        {
            return (!string.IsNullOrEmpty(Version));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "version to")]
        public String VersionTo { get; set; }

        public bool ShouldSerializeVersionTo()
        {
            return (!string.IsNullOrEmpty(VersionTo));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "automatd testcase backend")]
        public String Automatd_Testcase_Backend { get; set; }

        public bool ShouldSerializeAutomatd_Testcase_Backend()
        {
            return (!string.IsNullOrEmpty(Automatd_Testcase_Backend));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "automated testcase UI")]
        public String Automated_Testcase_UI { get; set; }

        public bool ShouldSerializeAutomated_Testcase_UI()
        {
            return (!string.IsNullOrEmpty(Automated_Testcase_UI));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "test case type")]
        public String TestCase_Type { get; set; }

        public bool ShouldSerializeTestCase_Type()
        {
            return (!string.IsNullOrEmpty(TestCase_Type));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "source_version")]
        public String Source_Version { get; set; }

        public bool ShouldSerializeSource_Version()
        {
            return (!string.IsNullOrEmpty(Source_Version));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Customer Use Case / Requirement Name")]
        public String Customer_Use_Case { get; set; }

        public bool ShouldSerializeCustomer_Use_Case()
        {
            return (!string.IsNullOrEmpty(Customer_Use_Case));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "legacyId")]
        public string LegacyId { get; set; }

        public bool ShouldSerializeLegacyId()
        {
            return (!string.IsNullOrEmpty(LegacyId));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "automation")]
        public String Automation { get; set; }

        public bool ShouldSerializeAutomation()
        {
            return (!string.IsNullOrEmpty(Automation));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "jira")]
        public String Jira { get; set; }

        public bool ShouldSerializeJira()
        {
            return (!string.IsNullOrEmpty(Jira));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "model")]
        public String Model { get; set; }

        public bool ShouldSerializeModel()
        {
            return (!string.IsNullOrEmpty(Model));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "DB Type")]
        public String DB_Type { get; set; }

        public bool ShouldSerializeDB_Type()
        {
            return (!string.IsNullOrEmpty(DB_Type));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "server")]
        public String Server { get; set; }

        public bool ShouldSerializeServer()
        {
            return (!string.IsNullOrEmpty(Server));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Automation ID")]
        public String AutomationID { get; set; }

        public bool ShouldSerializeAutomationID()
        {
            return (!string.IsNullOrEmpty(AutomationID));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Setup")]
        public String Setup { get; set; }

        public bool ShouldSerializeSetup()
        {
            return (!string.IsNullOrEmpty(Setup));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Vault ID")]
        public String VaultID { get; set; }

        public bool ShouldSerializeVaultID()
        {
            return (!string.IsNullOrEmpty(VaultID));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Test Priority")]
        public String TestPriority { get; set; }

        public bool ShouldSerializeTestPriority()
        {
            return (!string.IsNullOrEmpty(TestPriority));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Client Type")]
        public String ClientType { get; set; }

        public bool ShouldSerializeClientType()
        {
            return (!string.IsNullOrEmpty(ClientType));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Group Name")]
        public String GroupName { get; set; }

        public bool ShouldSerializeGroupName()
        {
            return (!string.IsNullOrEmpty(GroupName));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Set up")]
        public String Set_UP { get; set; }

        public bool ShouldSerializeSet_UP()
        {
            return (!string.IsNullOrEmpty(Set_UP));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Test Client")]
        public String TestClient { get; set; }

        public bool ShouldSerializeTestClient()
        {
            return (!string.IsNullOrEmpty(TestClient));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Implemented Version")]
        public String ImplementedVersion { get; set; }

        public bool ShouldSerializeImplementedVersion()
        {
            return (!string.IsNullOrEmpty(ImplementedVersion));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Deprecated Version")]
        public String DeprecatedVersion { get; set; }

        public bool ShouldSerializeDeprecatedVersion()
        {
            return (!string.IsNullOrEmpty(DeprecatedVersion));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "TestCase Quality")]
        public String TestCaseQuality { get; set; }

        public bool ShouldSerializeTestCaseQuality()
        {
            return (!string.IsNullOrEmpty(TestCaseQuality));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "TestCase Qulaity Comment")]
        public String TestCaseQualityComment { get; set; }

        public bool ShouldSerializeTestCaseQualityComment()
        {
            return (!string.IsNullOrEmpty(TestCaseQualityComment));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "PRICING PAW REGRESSION")]
        public String PricingPawRegression { get; set; }

        public bool ShouldSerializePricingPawRegression()
        {
            return (!string.IsNullOrEmpty(PricingPawRegression));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Pre-requisites")]
        public String Prerequisites { get; set; }

        public bool ShouldSerializePrerequisites()
        {
            return (!string.IsNullOrEmpty(Prerequisites));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Weightage")]
        public String Weightage { get; set; }

        public bool ShouldSerializeWeightage()
        {
            return (!string.IsNullOrEmpty(Weightage));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Project")]
        public String Project { get; set; }

        public bool ShouldSerializeProject()
        {
            return (!string.IsNullOrEmpty(Project));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Severity")]
        public String Severity { get; set; }

        public bool ShouldSerializeSeverity()
        {
            return (!string.IsNullOrEmpty(Severity));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Preference")]
        public String Preference { get; set; }

        public bool ShouldSerializePreference()
        {
            return (!string.IsNullOrEmpty(Preference));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Is HMM")]
        public String Is_HMM { get; set; }

        public bool ShouldSerializeIs_HMM()
        {
            return (!string.IsNullOrEmpty(Is_HMM));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "TM Release")]
        public String TM_Release { get; set; }

        public bool ShouldSerializeTM_Release()
        {
            return (!string.IsNullOrEmpty(TM_Release));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Spec/Ref #")]
        public String Spec_Ref { get; set; }

        public bool ShouldSerializeSpec_Ref()
        {
            return (!string.IsNullOrEmpty(Spec_Ref));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "TestAutomatedBy")]
        public String TestAutomatedBy { get; set; }

        public bool ShouldSerializeTestAutomatedBy()
        {
            return (!string.IsNullOrEmpty(TestAutomatedBy));
        }

        //HPQC Custom Field-User Defined
        [JsonProperty(propertyName: "Issue#")]
        public String Issue { get; set; }

        public bool ShouldSerializeIssue()
        {
            return (!string.IsNullOrEmpty(Issue));
        }

        [JsonProperty(propertyName: "priority")]
        public String Priority { get; set; }

        public bool ShouldSerializePriority()
        {
            return (!string.IsNullOrEmpty(Priority));
        }

    }
}