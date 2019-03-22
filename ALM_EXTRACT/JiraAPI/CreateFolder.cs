using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ALM_EXTRACT.JiraAPI
{
    public class CreateFolder
    {
        [JsonProperty(propertyName: "projectKey")]
        public string ProjectKey { get; set; }

        [JsonProperty(propertyName: "name")]
        public String FolderName { get; set; }

        [JsonProperty(propertyName: "type")]
        public String FieldType { get; set; }

    }
}