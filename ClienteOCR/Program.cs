using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Core.Common;

namespace ClienteSintegra
{
    internal static class Program
    {
        private static void GravarResposta(String palavra, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || palavra == null)
            {
                return;
            }
            StreamWriter fileWriter = null;
            try
            {
                fileWriter = File.CreateText(filePath);
                fileWriter.Write(palavra);
            }
            finally
            {
                if (fileWriter != null)
                {
                    fileWriter.Close();
                }
            }
        }

        /// <summary>
        ///   The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            /*
             Parametros aceitos:
             * /ser=Servico_para_reconhecer_imagem
             * /in=Endereço_da_imagem
             * /out=Endereço_do_txt_com_resposta
             */
            var respFile = String.Empty;
            var debbug = false;
            var palavra = "!!!!";

            var parametros = new List<String>(args);
            if (parametros.Count >= 3)
            {
                try
                {
                    var token = new Token();
                    if (!token.IsValid())
                    {
                        Log.Append("Licença de uso inválida ou inexistente.");
                        return;
                    }

                    var servico = parametros[0].Replace("/ser=", "").Replace("'", "'");
                    var imgEntradaFile = parametros[1].Replace("/in=", "").Replace("'", "'");
                    respFile = parametros[2].Replace("/out=", "").Replace("'", "'");

                    if (parametros.Count > 3)
                    {
                        debbug = parametros[3].Replace("/tempo=", "").Replace("'", "'").ToUpper() == "S";
                    }

                    var consulta = new ConsultaCaptcha();
                    consulta.CarregarCaptcha(imgEntradaFile);
                    var dt = DateTime.Now;
                    palavra = consulta.ReconhecerCaptcha(servico, token.Key);
                    var tempo = DateTime.Now - dt;
                    if (debbug)
                    {
                        palavra = String.Format("{0}; Tempo: {1} segundos.", palavra, tempo.ToString(@"s\.fff"));
                    }
                }
                catch (Exception exception)
                {
                    Log.Append(String.Format("{0}", exception.Message));
                }
                finally
                {
                    if (respFile != String.Empty)
                    {
                        GravarResposta(palavra, respFile);
                    }
                    Application.Exit();
                }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var f = new Form1();
                Application.Run(f);
            }
        }
    }
}