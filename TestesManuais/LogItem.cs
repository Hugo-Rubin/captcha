using System;

namespace TestesManuais
{
    public class LogItem
    {
        private readonly String fileName;
        private readonly String padrao;
        private readonly String text;

        public LogItem(String fileName, String text, bool correto = true, String padrao = "")
        {
            this.fileName = fileName;
            this.text = text;
            Correto = correto;
            this.padrao = padrao;
        }

        public String Text
        {
            get { return text; }
        }

        public String FileName
        {
            get { return fileName; }
        }

        public String Padrao
        {
            get { return padrao; }
        }

        public bool Correto { get; set; }

        public override string ToString()
        {
            return String.Format("{0}\t({1})", text, fileName.Substring(0, fileName.IndexOf(".")));
        }
    }
}