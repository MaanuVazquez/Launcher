using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher {
    public class Regedit {
        public RegistryKey baseRegistryKey {
            get; private set;
        }
        public string subKey {
            get; private set;
        }

        public Regedit(RegistryKey baseRegistryKey, string subKey) {
            this.baseRegistryKey = baseRegistryKey;
            this.subKey = subKey;
        }

        /**
         * Lee una clave del registro y la devuelve
         */
        public string Read(string KeyName) {
            try {
                RegistryKey rk = baseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(subKey);
                if (sk1 == null) {
                    return null;
                } else {
                    return string.Empty + sk1.GetValue(KeyName);
                }
            } catch (Exception ex) {
                Utils.log(ex.ToString());
                return null;
            }
        }

        /**
         * Escribe una nueva clave en el registro
         */
        public bool Write(string KeyName, object Value) {
            try {
                RegistryKey rk = baseRegistryKey;
                RegistryKey sk1 = rk.CreateSubKey(subKey);
                sk1.SetValue(KeyName, Value);
                return true;
            } catch (Exception ex) {
                Utils.log(ex.ToString());
                return false;
            }
         
        }
    }
}
