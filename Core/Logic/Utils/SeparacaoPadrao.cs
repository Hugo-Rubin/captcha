using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Separacao;
using Core.Logic.Types;

namespace Core.Logic.Utils
{
    public class SeparacaoPadrao
    {
        private readonly ImgArray imgArray;
        private readonly int numeroMinimoDeLetras;
        private Point tamanhoImagemLetra;
        private ColorFillingSegmentation2 cfs;
        private SeamCarving2 sc;

        public SeparacaoPadrao(Captcha captcha)
        {
            imgArray = captcha.ImgArray;
            numeroMinimoDeLetras = captcha.NumeroMinimoDeLetras;
            tamanhoImagemLetra = captcha.TamanhoImagemLetra;
        }

        public SeparacaoPadrao(Captcha captcha, ColorFillingSegmentation2 cfs, SeamCarving2 sc = null)
        {
            imgArray = captcha.ImgArray;
            numeroMinimoDeLetras = captcha.NumeroMinimoDeLetras;
            tamanhoImagemLetra = captcha.TamanhoImagemLetra;
            this.cfs = cfs;
            this.sc = sc;
        }

        /// <summary>
        ///   Executa a separação com ColorFillingSegmentation2 e só exexuta SeamCarving2 se for necessário
        /// </summary>
        /// <returns> </returns>
        public ImgArray[] ColorFillingSegmentation2AndSeamCarving2(ImgArray imgTratado = null, bool tamanhoVariavel = true, bool checkclusters = false)
        {
            // TODO: Essa chamada ao CFS2 mescla caracteres que cruzam no eixo X (onde algum pixel do caractere X está na mesma coluna de algum pixel do caractere Y), o que pode fazer com...
            // TODO: ...que duas imagens que não se tocam sejam colocadas e retornadas em um mesmo cluster, sobrando para o SeamCarving2 a tarefa de separá-las.
            if (cfs == null)
            {
                cfs = new ColorFillingSegmentation2(imgTratado ?? imgArray);
            }

            var chars = cfs.GetCaracteres().RemoveWhiteBordersTodos();

            for (int i = 0; i < chars.Count; i++)
            {
                int pCount = chars[i].CountPixelsWithColor(Color.Black);
                if (pCount < 1)
                {
                    chars.RemoveAt(i);
                }
            }

            // TODO: Precisamos organizar melhor essa parte de verificação dos clusters.
            if (tamanhoVariavel == false)
            {
                while (chars.Count > numeroMinimoDeLetras)
                {
                    var menorCluster = 0;
                    for (var i = 1; i < chars.Count; i++)
                    {
                        if (chars[i].CountPixelsWithColor(Color.Black) <
                            chars[menorCluster].CountPixelsWithColor(Color.Black))
                        {
                            menorCluster = i;
                        }
                    }

                    chars.RemoveAt(menorCluster);
                }
            }

            /*if (checkclusters)
            {
                int tamMin = tamanhoImagemLetra.X > tamanhoImagemLetra.Y ? tamanhoImagemLetra.X : tamanhoImagemLetra.Y;
                for (int i = 0; i < chars.Count; i++)
                {
                    int pCount = chars[i].CountPixelsWithColor(Color.Black);
                    if (pCount < tamMin)
                    {
                        chars.RemoveAt(i);
                    }
                }
            }*/

            while (chars.Count < numeroMinimoDeLetras && chars.Count > 0)
            {
                var maxIdx = chars.IndexOf(chars.FirstOrDefault(i => i.Width == chars.Max(m => m.Width)));

                // TODO: Esta chamada do SeamCarving2 passa a imagem a ser separada sem qualquer tipo borda, podendo causar uma separação errada. O certo seria chamar...
                // TODO: ... InserirBordaX() e InserirBordaY() antes de calcular o width (que é usado no intervalo) e de passar a imagem para o GetClusters().

                // BLL do ConsigRJ abaixo:
                /*ImgArray maxC = new ImgArray(chars[maxIdx]).InserirBordaX(6).InserirBordaY(6);
                SeamCarving2 seamCarving = new SeamCarving2(2, maxC.Width / 6, true);
                ImgArray[] clusters = seamCarving.GetClusters(maxC);
                
                if (clusters.Length == 1)
                {
                    maxC = new ImgArray(chars[maxIdx]).InserirBordaX(2).InserirBordaY(2);
                    seamCarving = new SeamCarving2(2, maxC.Width / 2);
                    clusters = seamCarving.GetClusters(maxC);
                }*/

                var maxC = new ImgArray(chars[maxIdx]).InserirBordaX(2).InserirBordaY();
                sc = new SeamCarving2(2, maxC.Width / 2);

                // TODO: Verificar se a separação padrão funciona para todos os captchas. Foi adicionado o InserirBordaX e InserirBordaY.
                var clusters = sc.GetClusters(maxC);

                // Substitui Imagem Colada pelos clusters encontrados
                chars.RemoveAt(maxIdx);
                chars.InsertRange(maxIdx, clusters);                
            }            

            return chars.ToArray().CortarECentralizarTodos(tamanhoImagemLetra.X, tamanhoImagemLetra.Y).ToArray();
        }

        public ImgArray[] ColorFillingSegmentation2AndCorteCego(int tamanholetra)
        {
            if (cfs == null)
            {
                cfs = new ColorFillingSegmentation2(imgArray);
            }

            var chars = cfs.GetCaracteres().RemoveWhiteBordersTodos();
            var indicesDeCorte = new List<int>();
            var charsCorte = new List<ImgArray>();
            var tamOriginalLetra = tamanholetra;
            var controllerCount = 0;

            // TODO: adicionar tratamento para o caso do número de caracteres ser igual ao número de saída esperada, mas um dos clusters ser maior do que o tamanho máximo de um caractere sozinho
            // TODO: adicionar tratamento para descartar um cluster muito pequeno, ou mesclá-lo à esquerda, caso seja encontrado

            if (chars.Count == numeroMinimoDeLetras)
            {
                for (var i = 0; i < chars.Count; i++)
                {
                    if (chars[i].CountPixelsWithColor(Color.Black) < 50)
                    {
                        chars.RemoveAt(i);
                    }
                }
            }

            while (chars.Count < numeroMinimoDeLetras
                && chars.Count > 0
                && controllerCount < numeroMinimoDeLetras + 1)
            {
                var maxWidth = chars.Max(i => i.Width);
                var maxIdx = chars.IndexOf(chars.FirstOrDefault(i => i.Width == chars.Max(m => m.Width)));
                indicesDeCorte.Add(maxIdx);
                tamanholetra = tamOriginalLetra;

                var maxC = new ImgArray(chars[maxIdx]);

                var clusters = new List<ImgArray>();

                // Cortar a cada tamanholetra e adicionar na lista clusters

                var numCarac = -1;

                if (maxWidth < 55)
                {
                    // Se for menor do q 53, então são 2 caracteres. Caso seja maior do que 42 e menor q 53 não cortar no pixel 19, e sim num pixel mais alto. 
                    // Analisar a possibilidade de identificar se o caractere maior, a letra 'm' normalmente, está à direita ou à esquerda para cortarmos certo.

                    numCarac = 2;

                    if (maxWidth > 42)
                    {
                        var bigCharPosition = ProcurarLetraM(maxC);

                        if (bigCharPosition != -1)
                        {
                            tamanholetra = bigCharPosition;
                        }
                        else
                        {
                            tamanholetra = maxC.Width / 2 - 1;
                        }
                    }
                }

                for (var i = 0; i < maxC.Width; i += tamanholetra)
                {
                    if ((i != 0 && numCarac == 2) || maxWidth - (i + tamanholetra) < tamanholetra / 2)
                    {
                        clusters.Add(maxC.GetSegment(new Rectangle(i, 0, maxC.Width - i, maxC.Height)));
                        break;
                    }

                    var r = new Rectangle(i, 0, tamanholetra, maxC.Height);
                    clusters.Add(maxC.GetSegment(r));
                }

                charsCorte.AddRange(clusters);
                chars.RemoveAt(maxIdx);
                chars.InsertRange(maxIdx, clusters);
                controllerCount++;
            }

            while (chars.Count > numeroMinimoDeLetras)
            {
                var menorCluster = 0;
                var clusterSize = new int[chars.Count];

                for (var i = 0; i < chars.Count; i++)
                {
                    clusterSize[i] = chars[i].CountPixelsWithColor(Color.Black);

                    if (clusterSize[i] < clusterSize[menorCluster])
                    {
                        menorCluster = i;
                    }
                }

                /*if (clusterSize[menorCluster] > 20)
                {
                    // TODO: Terminar implementação do MesclarCLusters
                    MesclarClusters(charsCorte.ToArray(), clusterSize, indicesDeCorte.ToArray());
                }
                else
                {*/
                chars.RemoveAt(menorCluster);
                //}
            }

            for (var i = 0; i < chars.Count; i++)
            {
                chars[i] = chars[i].PreencherPixels().CortarECentralizar(tamanhoImagemLetra.X, tamanhoImagemLetra.Y);
            }

            return chars.ToArray();
        }

        private int ProcurarLetraM(ImgArray img)
        {
            var pixelsPretosEmSerie = 0;
            var pixelsBrancosEmSerie = 0;
            var segundaPerna = false;
            var topoPrimeiroVao = 0;

            for (var x = 0; x < img.Width; x++)
            {
                for (var y = img.Height - 1; y >= 0; y--)
                {
                    if (img.GetPixel(x, y).IsBlackPixel())
                    {
                        for (var u = x; u < img.Width; u++)
                        {
                            if (pixelsPretosEmSerie > 8 || pixelsBrancosEmSerie > 12)
                            {
                                break;
                            }

                            if (!img.GetPixel(u, y).IsBlackPixel())
                            {
                                ChaveValor<int, bool> verificaVaoBrancoResult;
                                if (pixelsPretosEmSerie >= 4 && !segundaPerna)
                                {
                                    verificaVaoBrancoResult = VerificarVaoBranco(img, u, y, 8);
                                    topoPrimeiroVao = verificaVaoBrancoResult.Chave;
                                    var obedeceLimitesVao1 = verificaVaoBrancoResult.Valor;

                                    if (!obedeceLimitesVao1)
                                    {
                                        break;
                                    }

                                    pixelsPretosEmSerie = 0;
                                    pixelsBrancosEmSerie++;
                                }
                                else
                                {
                                    if (segundaPerna)
                                    {
                                        if (pixelsPretosEmSerie >= 4)
                                        {
                                            verificaVaoBrancoResult = VerificarVaoBranco(img, u, y, 8);
                                            var topoSegundoVao = verificaVaoBrancoResult.Chave;
                                            var obedeceLimitesVao2 = verificaVaoBrancoResult.Valor;

                                            if (!obedeceLimitesVao2 || Math.Abs(topoPrimeiroVao - topoSegundoVao) > 4)
                                            {
                                                break;
                                            }

                                            pixelsBrancosEmSerie++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (pixelsPretosEmSerie < 4 && pixelsBrancosEmSerie == 0)
                                        {
                                            break;
                                        }

                                        pixelsBrancosEmSerie++;
                                    }
                                }
                            }
                            else
                            {
                                if (pixelsPretosEmSerie == 0 && pixelsBrancosEmSerie > 0)
                                {
                                    pixelsBrancosEmSerie = 0;
                                    pixelsPretosEmSerie++;
                                    segundaPerna = true;
                                }
                                else
                                {
                                    if (pixelsPretosEmSerie > 0 && pixelsBrancosEmSerie > 0 && segundaPerna)
                                    {
                                        if (x > 10)
                                        {
                                            return x + 2;
                                            // Caso o M seja o 2º caractere, retorno a coluna de seu início para que seja feito o corte.
                                        }

                                        // Caso o M seja o 1º caractere, retorno a coluna de seu fim para que seja feito o corte.
                                        return u + 5;
                                        // Achou a terceira perna do M, a posição do corte cego deve ser cerca de 5 pixels a direita (assumindo q a 3ª perna do M tenha algo próximo de 5 pixels de espessura).
                                    }

                                    pixelsPretosEmSerie++;
                                }
                            }
                        }

                        pixelsPretosEmSerie = 0;
                        pixelsBrancosEmSerie = 0;
                        segundaPerna = false;
                        break;
                        // Caso o primeiro pixel da coluna não tenha passado nos testes, passo para a coluna seguinte.
                    }
                }
            }

            return -1;
        }

        private ChaveValor<int, bool> VerificarVaoBranco(ImgArray img, int x, int v, int lim)
        {
            var numPixelsBrancosAcima = 0;
            var numPixelsBrancosAbaixo = 0;
            var idxTopo = 0;
            var existeCaminhoAteABase = true;

            for (var y = v; y >= 0; y--)
            {
                if (!img.GetPixel(x, y).IsBlackPixel())
                {
                    numPixelsBrancosAcima++;
                    idxTopo = y;
                }
                else
                {
                    idxTopo = y + 1;
                    break;
                }
            }

            for (var y = v; y < img.Height; y++)
            {
                if (!img.GetPixel(x, y).IsBlackPixel())
                {
                    numPixelsBrancosAbaixo++;
                }
                else
                {
                    existeCaminhoAteABase = false;
                    var k = x + 1;
                    var brk = false;

                    for (var w = y; w < img.Height; w++)
                    {
                        if (brk)
                        {
                            break;
                        }

                        while (img.GetPixel(k, w).IsBlackPixel())
                        {
                            if (k > x + 3)
                            {
                                brk = true;
                                break;
                            }
                            k++;
                        }

                        if (w == img.Height - 1)
                        {
                            existeCaminhoAteABase = true;
                        }
                    }

                    break;
                }
            }

            return new ChaveValor<int, bool>
                       {
                           Chave = idxTopo,
                           Valor = numPixelsBrancosAcima + numPixelsBrancosAbaixo >= lim && existeCaminhoAteABase
                       };
        }
    }
}