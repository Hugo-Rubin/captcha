using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static void GravarLinhasEmArquivo(string filePath, IEnumerable<string> lines, bool overrite)
        {
            if (string.IsNullOrEmpty(filePath)
                || lines == null
                || !lines.Any())
            {
                return;
            }

            StreamWriter fileWriter = null;

            try
            {
                if (!overrite && File.Exists(filePath))
                {
                    fileWriter = File.AppendText(filePath);
                }
                else
                {
                    fileWriter = File.CreateText(filePath);
                }

                foreach (var line in lines)
                {
                    fileWriter.WriteLine(line);
                }
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