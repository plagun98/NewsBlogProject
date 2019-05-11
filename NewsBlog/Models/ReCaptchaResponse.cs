using Newtonsoft.Json;
using System.Collections.Generic;

namespace NewsBlog.Models
{
    public class ReCaptchaResponse
    {
        public bool success
        {
            get;
            set;
        }
        public string challenge_ts
        {
            get;
            set;
        }
        public string hostname
        {
            get;
            set;
        }
        [JsonProperty(PropertyName = "error-codes")]
        public List<string> error_codes
        {
            get;
            set;
        }
    }
}