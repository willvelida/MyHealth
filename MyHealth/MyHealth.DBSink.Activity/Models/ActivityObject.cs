using Newtonsoft.Json;
using mdl = MyHealth.Schemas;

namespace MyHealth.DBSink.Activity.Models
{
    public class ActivityObject
    {
        [JsonProperty("id")]
        public string ActivityId { get; set; }
        public mdl.Activity Activity { get; set; }
    }
}
