using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using mdl = MyHealth.Schemas;

namespace MyHealth.DBSink.Sleep.Models
{
    public class SleepObject
    {
        [JsonProperty("id")]
        public string ObjectId { get; set; }
        public mdl.Sleep Sleep { get; set; }
        public string DocumentType { get; set; }
        public string FileName { get; set; }
    }
}
