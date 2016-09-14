using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ISupportGenesys.Models
{
    public class Entities
    {
        [JsonProperty("entities")]
        public List<Incident> Incidents { get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
        public int total { get; set; }
    }

    public class Incident
    {
        public string id { get; set; }
        public string state { get; set; }
        public string status { get; set; }
        public string milestone { get; set; }
        public Organization organization { get; set; }
        public PrimaryContact primaryContact { get; set; }
        public string incidentType { get; set; }
        public string description { get; set; }
        public string problemCategorization { get; set; }
        public string interimSummary { get; set; }
        public string lastUpdatedDate { get; set; }
        public string createdDateTime { get; set; }
    }
    public class Organization
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class PrimaryContact
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
