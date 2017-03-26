using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TE.Apps.Staging
{
    public static class Logging
    {
        private const string LogFileName = "backupstaging.log";


        private static string GetLogFilePath()
        {
            return Path.Combine(Path.GetTempPath(), LogFileName);
        }

        /// <summary>
        /// Deletes the log file.
        /// </summary>
        public static void Delete()
        {
            File.Delete(GetLogFilePath());
        }

        /// <summary>
        /// Writes a line to the log file.
        /// </summary>
        /// <param name="text">
        /// The text to write to the log file.
        /// </param>
        public static void WriteLine(string text)
        {
            using (StreamWriter sw = File.AppendText(GetLogFilePath()))
            {
                sw.WriteLine(
                    string.Format("{0:yyyy-MM-dd HH:mm:ss:ffff} {1}",
                        DateTime.Now,
                        text));
            }
        }
    }
}
