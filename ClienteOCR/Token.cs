using System;
using System.IO;

namespace ClienteSintegra
{
    public class Token
    {
        private const string TokenFile = "License.tk";
        private readonly String proprietario = String.Empty;

        private String key = String.Empty;

        public Token()
        {
            CarregarLicenca();
        }

        public String Key
        {
            get { return key; }
        }

        public String Proprietario
        {
            get { return proprietario; }
        }

        public bool CarregarLicenca()
        {
            var result = false;
            if (File.Exists(TokenFile))
            {
                string arquivo;
                using (var reader = new StreamReader(TokenFile))
                {
                    arquivo = reader.ReadToEnd();
                }

                var campos = arquivo.Split(';');
                if (campos[0].Length == 20)
                {
                    key = campos[0];
                    result = true;
                }
            }
            return result;
        }

        public bool IsValid()
        {
            return key.Length == 20;
        }
    }
}