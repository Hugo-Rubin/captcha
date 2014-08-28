using System.Drawing;
using System.Linq;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Separacao;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.SeparacaoADAP_CRJ_Cinza
{
    public class SeparacaoPadraoADAP
    {
        private readonly ImgArray imgArray;
        private readonly int numeroMinimoDeLetras;
        private Point tamanhoImagemLetra;

        public SeparacaoPadraoADAP(Captcha captcha)
        {
            imgArray = captcha.ImgArray;
            numeroMinimoDeLetras = captcha.NumeroMinimoDeLetras;
            tamanhoImagemLetra = captcha.TamanhoImagemLetra;
        }

        /// <summary>
        ///   Executa a separação com ColorFillingSegmentation2 e só exexuta SeamCarving2 se for necessário
        /// </summary>
        /// <returns> </returns>
        public ImgArray[] ColorFillingSegmentation2AndSeamCarving2(ImgArray imgTratado = null,
                                                                   bool tamanhoVariavel = true)
        {
            // TODO: Essa chamada ao CFS2 mescla caracteres que cruzam no eixo X (onde algum pixel do caractere X está na mesma coluna de algum pixel do caractere Y), o que pode fazer com...
            // TODO: ...que duas imagens que não se tocam sejam colocadas e retornadas em um mesmo cluster, sobrando para o SeamCarving2 a tarefa de separá-las.
            var cfs = new ColorFillingSegmentation2(imgTratado ?? imgArray, 8, 35);

            var chars = cfs.GetCaracteres().RemoveWhiteBordersTodos();

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

            while (chars.Count < numeroMinimoDeLetras && chars.Count > 0)
            {

                var maxIdx = chars.IndexOf(chars.FirstOrDefault(i => i.Width == chars.Max(m => m.Width)));

                // TODO: Esta chamada do SeamCarving2 passa a imagem a ser separada sem qualquer tipo borda, podendo causar uma separação errada. O certo seria chamar...
                // TODO: ... InserirBordaX() e InserirBordaY() antes de calcular o width (que é usado no intervalo) e de passar a imagem para o GetClusters().

                var maxC = new ImgArray(chars[maxIdx]).InserirBordaX(6).InserirBordaY(6);
                var seamCarving = new SeamCarving2ADAP(2, maxC.Width / 6, true);
                var clusters = seamCarving.GetClusters(maxC);

                if (clusters.Length == 1)
                {
                    maxC = new ImgArray(chars[maxIdx]).InserirBordaX(2).InserirBordaY(2);
                    seamCarving = new SeamCarving2ADAP(2, maxC.Width / 2);
                    clusters = seamCarving.GetClusters(maxC);
                }

                /*SeamCarving2 seamCarving = new SeamCarving2(2, maxWidth / 2);
                ImgArray[] clusters = seamCarving.GetClusters(new ImgArray(chars[maxIdx]));*/

                // Substitui Imagem Colada pelos clusters encontrados
                chars.RemoveAt(maxIdx);
                chars.InsertRange(maxIdx, clusters);
            }

            return chars.ToArray().CortarECentralizarTodos(tamanhoImagemLetra.X, tamanhoImagemLetra.Y).ToArray();
        }
    }
}