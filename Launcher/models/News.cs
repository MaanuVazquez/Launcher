using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.models {
    [JsonObject(MemberSerialization.OptIn)]
    public class News
     {

        [JsonProperty]
        public int id {
            get; set;
        }

        [JsonProperty]
        public string title {
            get; set;
        }

        [JsonProperty]
        public string content {
            get; set;
        }

        [JsonProperty]
        public string author {
            get; set;
        }

        [JsonProperty]
        public DateTime date {
            get; set;
        }
    }
}
