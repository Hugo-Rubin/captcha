using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var debug = false;
            var palavra = "!!!!";

            //args = new[]
            //{
            //    "/ser=RF",
            //    @"/in=9cedff0d-748a-48de-bf02-02fd8062e1fb",
            //    @"/out=C:\OCR\resposta.txt"
            //};


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
                        debug = parametros[3].Replace("/tempo=", "").Replace("'", "'").ToUpper() == "S";
                    }

                    bool usarRF3;
                    bool.TryParse(CustomConfigurationManager.ReadAppSetting("usarRF3"), out usarRF3);
                    var dt = DateTime.Now;
                    if (usarRF3 && ((new[] {"RF3", "RF"}).Contains(servico)))
                    {
                        var manager = new RF3Manager();
                        var images = manager.BaixarImagens(imgEntradaFile);
                        //todo: remove hard coded numbers
                        palavra = manager.Reconhecer("RF3", images, 181, 51, token.Key);
                    }
                    else
                    {
                        var consulta = new ConsultaCaptcha();
                        consulta.CarregarCaptcha(imgEntradaFile);
                        palavra = consulta.ReconhecerCaptcha(servico, token.Key);
                    }

                    var tempo = DateTime.Now - dt;
                    if (debug)
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