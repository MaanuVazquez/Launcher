using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.models {
    [JsonObject(MemberSerialization.OptIn)]
    public class Ranking {

        [JsonProperty]
        public string Name {
            get; set;
        }

        [JsonProperty]
        public int Level {
            get; set;
        }

        [JsonProperty]
        public int Resets {
            get; set;
        }

        [JsonProperty]
        public int Class {
            get; set;
        }
    }
}
