using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher {
    public static class Constants {
        /**
         * LAUNCHER OPTIONS
         */
        public static readonly string PATH = Directory.GetCurrentDirectory();
        public static readonly string MAIN_PATH = PATH + "\\main.exe";
        public static readonly string LAUNCHER_PATH = System.Reflection.Assembly.GetEntryAssembly().Location;
        public static readonly string LOG_PATH = PATH + "\\log.mu";
        public static readonly string UPDATES_LIST = "http://launcher.mueurus.com/update/files.txt";
        public static readonly string UPDATES_LOCATION = "http://launcher.mueurus.com/update/files";
        public static readonly string API_NEWS_WEB = "http://mueurus.com/api/news?max=5";
        public static readonly string API_RANKING = "http://mueurus.com/api/ranking/players?max=5";
        public static readonly string NEWS_WEB = "https://mueurus.com/news/";
        public static readonly bool ONLY_ONE_MAIN = false;
    }
}
