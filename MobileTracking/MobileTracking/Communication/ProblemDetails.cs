using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MobileTracking.Communication
{
    public class ProblemDetails
    {

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
       
        [JsonPropertyName("status")]
        public int? Status { get; set; }

        public string Detail { get; set; } = string.Empty;

        [JsonPropertyName("instance")]
        public string Instance { get; set; } = string.Empty;
        
        [JsonExtensionData]
        public IDictionary<string, object>? Extensions { get; }
    }
}
