using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MK.Modules.BlackDesert.BossTimer.Models
{
    public class BossTimerConfig
    {
        [JsonProperty("bossChannelID")]
        public ulong BossChannelID { get; set; }
    }
}
