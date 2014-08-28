using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Text;
using Core.Logic.Types;

namespace Core.Logic
{
    public static class ServerLog
    {
        private static readonly string LogDir = ConfigurationManager.AppSettings["LogDir"] ?? ServerUtil.AbsolutePath + @"\log\";

        public static void Append(string linha, string logName = "Log.txt")
        {
            var sb = new StringBuilder();
            sb.AppendLine(DateTime.Now.ToHorarioBrasileiro().ToString("dd/MM/yyyy hh:mm:ss") + " - " + linha);
            GravarLog(sb, logName);
        }

        public static void Append(StringBuilder sb, string logName = "Log.txt")
        {
            if (logName == null)
            {
                throw new ArgumentNullException("logName");
            }
            GravarLog(sb, logName);
        }

        public static void AppendErrorLog(string linha, Bitmap image = null, string logName = "Log.txt")
        {
            var imgFile = string.Empty;
            const string imgDir = "ImagensComErro\\";

            try
            {
                if (image != null)
                {
                    var imgLogDir = LogDir + imgDir;

                    if (!Directory.Exists(imgLogDir))
                    {
                        Directory.CreateDirectory(imgLogDir);
                    }
                    imgFile = String.Format("{0}.png", DateTime.Now.ToHorarioBrasileiro().ToString("yyyyMMddhhmmssffff"));
                    image.Save(imgLogDir + imgFile);
                }

                var sb = new StringBuilder();
                sb.AppendFormat("{0} - {1}<br>", DateTime.Now.ToHorarioBrasileiro().ToString("dd/MM/yyyy hh:mm:ss"),
                                linha);
                if (imgFile != string.Empty)
                {
                    sb.AppendFormat("Imagem que gerou o erro: {0}<br>", imgFile);
                }
                else
                {
                    sb.AppendLine("Não informou a imagem na chamada do LOG<br>");
                }

                var sbHtml = new StringBuilder();
                sbHtml.Append(sb);
                if (imgFile != string.Empty)
                {
                    sbHtml.AppendFormat("<img src='{0}{1}'><br>", imgDir, imgFile);
                    sbHtml.AppendLine("_______");
                }

                sb.AppendLine("_______");
                GravarLog(sbHtml, "index.htm");
                GravarLog(sb.Replace("<br>", ""), logName);
            }
            catch (Exception exception)
            {
                Append("Erro no método AppendErrorLog " + exception.Message);
            }
        }

        public static void AppendErrorLog(string linha, ImgArray image = null, string logName = "Log.txt")
        {
            if (logName == null)
            {
                throw new ArgumentNullException("logName");
            }
            var imgFile = string.Empty;
            const string imgDir = "ImagensComErro\\";

            try
            {
                if (image != null)
                {
                    var imgLogDir = LogDir + imgDir;

                    if (!Directory.Exists(imgLogDir))
                    {
                        Directory.CreateDirectory(imgLogDir);
                    }
                    imgFile = String.Format("{0}.png", DateTime.Now.ToHorarioBrasileiro().ToString("yyyyMMddhhmmssffff"));
                    image.Save(imgLogDir + imgFile);
                }

                var sb = new StringBuilder();
                sb.AppendFormat("{0} - {1}<br>", DateTime.Now.ToHorarioBrasileiro().ToString("dd/MM/yyyy hh:mm:ss"),
                                linha);
                if (imgFile != string.Empty)
                {
                    sb.AppendFormat("Imagem que gerou o erro: {0}<br>", imgFile);
                }
                else
                {
                    sb.AppendLine("Não informou a imagem na chamada do LOG<br>");
                }

                var sbHtml = new StringBuilder();
                sbHtml.Append(sb);
                if (imgFile != string.Empty)
                {
                    sbHtml.AppendFormat("<img src='{0}{1}'><br>", imgDir, imgFile);
                    sbHtml.AppendLine("_______");
                }

                sb.AppendLine("_______");
                GravarLog(sbHtml, "index.htm");
                GravarLog(sb.Replace("<br>", ""), logName);
            }
            catch (Exception exception)
            {
                Append("Erro no método AppendErrorLog " + exception.Message);
            }
        }

        private static void GravarLog(StringBuilder lines, string logName = "Log.txt")
        {
            if (!Directory.Exists(LogDir))
            {
                Directory.CreateDirectory(LogDir);
            }
            var logFile = LogDir + logName;

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