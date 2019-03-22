using Newtonsoft.Json;

namespace ALM_EXTRACT.JiraAPI
{
    public class TestScript
    {
        [JsonProperty(propertyName: "type")]
        public string DesignStepType { get; set; }

        public bool ShouldSerializeDesignStepType()
        {
            return (!string.IsNullOrEmpty(DesignStepType));
        }

        [JsonProperty(propertyName: "steps")]
        public Steps[] Steps { get; set; }

        public bool ShouldSerializeSteps()
        {
            return (Steps != null);
        }
    }

    public class Steps
    {
        [JsonIgnore]
        public string StepId { get; set; }

        [JsonIgnore]
        public bool IsDesignAttachment { get; set; }

        [JsonProperty(propertyName: "description")]
        public string Description { get; set; }

        public bool ShouldSerializeDescription()
        {
            return (!string.IsNullOrEmpty(Description));
        }

        [JsonProperty(propertyName: "testData")]
        public string TestData { get; set; }

        public bool ShouldSerializeTestData()
        {
            return (!string.IsNullOrEmpty(TestData));
        }

        [JsonProperty(propertyName: "expectedResult")]
        public string ExpectedResult { get; set; }

        public bool ShouldSerializeExpectedResult()
        {
            return (!string.IsNullOrEmpty(ExpectedResult));
        }

    }
}