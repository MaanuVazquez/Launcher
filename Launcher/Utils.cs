using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Launcher
{
    public static class Utils
    {

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        /**
         * Escribe sobre la memoria de un proceso
         **/
        public static void WriteMem(Process p, int address, long v)
        {
            var hProc = OpenProcess(ProcessAccessFlags.All, false, (int)p.Id);
            var val = new byte[] { (byte)v };

            int wtf = 0;
            WriteProcessMemory(hProc, new IntPtr(address), val, (UInt32)val.LongLength, out wtf);

            CloseHandle(hProc);
        }

        /**
         * Retorna el Hash MD5 del archivo pasado por parámetro
         */
        public static string getHash(string filePath)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
        }

        /**
         * Compara el hash MD5 pasado por parámetro por el hash MD5 de un archivo pasádo por parámetro
         */
        public static bool compareMD5(string hash, string filePath)
        {
            return string.Equals(hash, getHash(filePath));
        }

        public static string zip(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Create the compressed file.
                using (FileStream outFile = File.Create(fi.FullName + ".gz"))
                {
                    using (GZipStream Compress = new GZipStream(outFile, CompressionMode.Compress))
                    {
                        // Copy the source file into 
                        // the compression stream.
                        inFile.CopyTo(Compress);
                        return fi.FullName + ".gz";
                    }
                }
            }
        }

        public static void unzip(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Get original file extension, for example
                // "doc" from report.doc.gz.
                string curFile = fi.FullName;
                string origName = curFile.Remove(curFile.Length -
                        fi.Extension.Length);

                //Create the decompressed file.
                using (FileStream outFile = File.Create(origName))
                {
                    using (GZipStream Decompress = new GZipStream(inFile,
                            CompressionMode.Decompress))
                    {
                        // Copy the decompression stream 
                        // into the output file.
                        Decompress.CopyTo(outFile);
                    }
                }
            }
        }

        public static Brush brushFromHTML(string html) {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(html));
        }
        
        public static bool isUserAdministrator() {
            bool isAdmin;
            WindowsIdentity user = null;
            try {
                user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            } catch (UnauthorizedAccessException ex) {
                Console.WriteLine(ex.Message);
                isAdmin = false;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                isAdmin = false;
            } finally {
                if (user != null)
                    user.Dispose();
            }
            return isAdmin;
        }

        public static void log(string logMessage) {
            try {
                using (TextWriter txtWriter= File.AppendText(Constants.LOG_PATH)) {
                    txtWriter.Write("\r\nLog Entry : ");
                    txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                        DateTime.Now.ToLongDateString());
                    txtWriter.WriteLine("  :");
                    txtWriter.WriteLine("  :{0}", logMessage);
                    txtWriter.WriteLine("-------------------------------");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
