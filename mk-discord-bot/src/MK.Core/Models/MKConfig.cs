using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MK.Models
{
    public class MKConfig
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
