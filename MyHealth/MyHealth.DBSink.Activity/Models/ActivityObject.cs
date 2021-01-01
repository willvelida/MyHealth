using Newtonsoft.Json;
using mdl = MyHealth.Helpers.Models;

namespace MyHealth.DBSink.Activity.Models
{
    public class ActivityObject
    {
        [JsonProperty("id")]
        public string ActivityId { get; set; }
        public mdl.Activity Activity { get; set; }
    }
}
