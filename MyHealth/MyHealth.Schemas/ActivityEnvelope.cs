using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHealth.Schemas
{
    public class ActivityEnvelope
    {
        [JsonProperty("id")]
        public string ActivityId { get; set; }
        public Activity Activity { get; set; }
        public string DocumentType { get; set; }
        public string FileName { get; set; }
    }
}
