using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Logic.ImageQuantizer.Quantizers.XiaolinWu;
using Core.Logic.Types;

namespace Core.Logic
{ //TODO: Refatorar
    public class IdentificaPadrao
    {
        public HashSet<TipoPadrao> GetPadrao(ImgArray src)
        {
            uint top = (uint)src.Height, bottom = 0;
            bool segment = false, test = false, foraPadrao = false;

            for (var i = 0; i < src.Height - 2; i++)
            {
                for (var j = 0; j < src.Width - 2; j++)
                {
                    var c = src.GetPixel(j, i);

                    if (c.IsBlackPixel())
                    {
                        if (top > i)
                        {
                            top = (uint)i;
                        }
                        if (bottom < i)
                        {
                            bottom = (uint)i;
                        }
                        if (!test)
                        {
                            try
                            {
                                if (!src.GetPixel(j + 1, i).IsBlackPixel() && !src.GetPixel(j, i - 1).IsBlackPixel()
                                    && !src.GetPixel(j, i + 1).IsBlackPixel())
                                {
                                    segment = false;
                                }
                                else
                                {
                                    if (src.GetPixel(j + 1, i).IsBlackPixel() && src.GetPixel(j + 2, i).IsBlackPixel()
                                        ||
                                        src.GetPixel(j, i + 1).IsBlackPixel() && src.GetPixel(j, i + 2).IsBlackPixel())
                                    {
                                        segment = false;
                                    }
                                    else
                                    {
                                        segment = true;
                                    }
                                    test = true;
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                ServerLog.AppendErrorLog(
                                    String.Format("{0} \t {1} = {2} \t {3}", e.Message, e.ParamName, e.ActualValue,
                                                  e.StackTrace), src);
                            }
                            catch (IndexOutOfRangeException e)
                            {
                                ServerLog.AppendErrorLog(
                                    String.Format("{0} \t {1} \t {2} \t {3}", e.Message, e.InnerException, e.HelpLink,
                                                  e.StackTrace), src);
                            }
                            catch (Exception e)
                            {
                                ServerLog.AppendErrorLog(e.Message, src);
                            }
                        }

                        // Verifica se todos os pixels pretos em uma imagem segmentada formam um quadrado 2 x 2.
                        // Se formarem, então a imagem está reta, senão a imagem está "fora do padrão", ou seja, embandeirada.
                        if (segment && !foraPadrao)
                        {
                            var c1 = src.GetPixel(j - 1, i);
                            var c2 = src.GetPixel(j + 1, i);
                            var c3 = src.GetPixel(j, i + 1);
                            var c4 = src.GetPixel(j + 1, i + 1);
                            if (c2.IsBlackPixel() && c3.IsBlackPixel())
                                if (!(!c1.IsBlackPixel() && c4.IsBlackPixel()))
                                    foraPadrao = true;
                        }
                    }
                }
            }

            if (bottom - top < 41 && !foraPadrao) // 40
            {
                return new HashSet<TipoPadrao> { TipoPadrao.Segmentada };
            }
            if (bottom - top < 65 || foraPadrao) //60 - 61
            {
                if (!segment)
                {
                    return new HashSet<TipoPadrao> { TipoPadrao.Bandeira };
                }
                return new HashSet<TipoPadrao> { TipoPadrao.Segmentada, TipoPadrao.Bandeira };
            }
            return new HashSet<TipoPadrao> { TipoPadrao.Distorcida, TipoPadrao.Segmentada };
        }

        public string DescreverPadrao(HashSet<TipoPadrao> padrao)
        {
            var descricao = "";
            if (padrao.Contains(TipoPadrao.Distorcida)) descricao += "Aplicar distorção inversa\n";
            if (padrao.Contains(TipoPadrao.Segmentada)) descricao += "Aplicar preenchimento\n";
            if (padrao.Contains(TipoPadrao.Bandeira)) descricao += "Aplicar endireitamento\n";
            return descricao;
        }
    }


    public class IdentificaPadraoConsigRJ
    {
        public TipoPadraoConsigRJ GetPadrao(Bitmap src)
        {
            var wu = new WuColorQuantizer();
            var pq = new PalleteQuantizer(src, wu, 2);
            var source = (Bitmap)pq.ApplyFilter();

            var cinza = Color.FromArgb(224, 225, 224);
            var cinzaB = Color.FromArgb(198, 200, 198);
            var verde = Color.FromArgb(243, 250, 242);
            var azul = Color.FromArgb(244, 245, 251);

            var minCinza = int.MaxValue;
            var minCinzaB = int.MaxValue;
            var minVerde = int.MaxValue;
            var minAzul = int.MaxValue;
            var minGlobal = int.MaxValue;
            var cor = source.GetPixel(0, 0);
            var brk = false;

            for (var y = 0; y < source.Height; y++)
            {
                if (brk)
                {
                    break;
                }

                for (var x = 0; x < source.Width; x++)
                {
                    var pixel = source.GetPixel(x, y);
                    var corBuffer = pixel;
                    if (corBuffer != cor)
                    {
                        brk = true;
                    }

                    var min = Math.Abs(pixel.R - cinza.R) + Math.Abs(pixel.G - cinza.G) + Math.Abs(pixel.B - cinza.B);
                    minCinza = min < minCinza ? min : minCinza;
                    min = Math.Abs(pixel.R - cinzaB.R) + Math.Abs(pixel.G - cinzaB.G) + Math.Abs(pixel.B - cinzaB.B);
                    minCinzaB = min < minCinzaB ? min : minCinzaB;
                    min = Math.Abs(pixel.R - verde.R) + Math.Abs(pixel.G - verde.G) + Math.Abs(pixel.B - verde.B);
                    minVerde = min < minVerde ? min : minVerde;
                    min = Math.Abs(pixel.R - azul.R) + Math.Abs(pixel.G - azul.G) + Math.Abs(pixel.B - azul.B);
                    minAzul = min < minAzul ? min : minAzul;
                    min = Math.Min(Math.Min(minCinza, minVerde), Math.Min(minCinzaB, minAzul));
                    minGlobal = min < minGlobal ? min : minGlobal;

                    if (brk)
                    {
                        break;
                    }
                }
            }

            if (minGlobal < 12)
            {
                if (minCinza == minGlobal || minCinzaB == minGlobal)
                {
                    return TipoPadraoConsigRJ.Cinza;
                }

                if (minVerde == minGlobal)
                {
                    return TipoPadraoConsigRJ.Verde;
                }

                if (minAzul == minGlobal)
                {
                    return TipoPadraoConsigRJ.Azul;
                }
            }
            //}
            //}
            return TipoPadraoConsigRJ.NaoEncontrado;
        }
    }
}