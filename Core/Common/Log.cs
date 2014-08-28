using System;
using System.IO;
using System.Text;

namespace Core.Common
{
    public static class Log
    {
        public static void Append(string linha, string logName = "Log.txt")
        {
            var sb = new StringBuilder();
            sb.AppendLine(DateTime.Now + " - " + linha);
            GravarLog(sb, logName);
        }

        public static void Append(StringBuilder sb, string logName = "Log.txt")
        {
            GravarLog(sb, logName);
        }

        private static void GravarLog(StringBuilder lines, string logName = "Log.txt")
        {
            var logFile = logName;

            if (string.IsNullOrEmpty(logName))
                return;
            if (lines == null || lines.Length == 0)
                return;

            StreamWriter fileWriter = null;
            try
            {
                fileWriter = File.Exists(logFile) ? File.AppendText(logFile) : File.CreateText(logFile);

                fileWriter.Write(lines.ToString());
            }
            finally
            {
                if (fileWriter != null)
                {
                    fileWriter.Close();
                }
            }
        }
    }
}