using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Core.Logic;

namespace TestesManuais
{
    public partial class TreinamentoRede : Form
    {
        private const string rootDirectory = @"E:\OCR\Testes\ESAJ\Rede";

        public TreinamentoRede()
        {
            InitializeComponent();
        }

        private void SalvarVetorDePixels()
        {
            var dicio = new[]
                            {
                                'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F', 'h', 'H', 'i',
                                'j', 'J', 'k', 'K', 'm', 'M', 'n', 'N', 'p', 'P', 'q', 'Q', 'r', 'R', 's',
                                'S', 't', 'T', 'u', 'U', 'v', 'V', 'w', 'W', 'x', 'X', 'y', 'Y', 'z', 'Z'
                            };
            
            bool brk = false;

            var parent = new DirectoryInfo(rootDirectory); // diretório pai
            ServerUtil.ValidatePath(parent.FullName);
            var subdirectories = parent.GetDirectories();

            foreach (var dir in subdirectories)
            {
                if (dir.Name[0] == '_')
                {
                    continue;
                }

                var s = new StringBuilder();
                Color c;
                //int luminance;

                //DirectoryInfo dir2 = new DirectoryInfo(@"C:\Users\Hugo\newTemplates"); // Caso queira criar o PI de apenas uma pasta, use dir2 aqui....

                var vImg = dir.FullName + @"\vTemplates_" + dir.Name + ".txt"; // ...aqui

                var imagens = dir.GetFiles("*.png"); // ... e aqui.

                foreach (var i in imagens)
                {
                    var img = Image.FromFile(i.FullName);
                    var bmp = (Bitmap)img;
                    var m = new double[1, bmp.Width * bmp.Height];
                    var k = 0;

                    for (var x = 0; x < bmp.Width; x++)
                    {
                        for (var y = 0; y < bmp.Height; y++)
                        {
                            c = bmp.GetPixel(x, y);
                            //luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                            //m[0, k] = luminance / 255;
                            m[0, k] = c.R > 127 ? 1 : 0;
                            s.Append(string.Format("{0};", m[0, k]));
                            k++;
                        }
                    }
                    //s.Append(string.Format("{0};", m[0, k]));

                    //int index = (Array.IndexOf(dicio, char.ToUpper(dir.Name[0])) + 1); // Caso o dicionário só use letras maiúsculas...
                    int index = (Array.IndexOf(dicio, dir.Name[0]) + 1); // ...caso contrário.

                    if (index == 0)
                    {
                        MessageBox.Show("O nome da pasta " + dir.Name[0] + " não foi encontrado no dicionário. Verifique se está usando o código de letras maiúsculas com um dicionário de letras minúsculas.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        brk = true;
                        break;
                    }
                    
                    s.Append(index + ";\n");

                    s.Remove(s.Length - 1, 1);
                    if (i.Name != imagens[imagens.Length - 1].Name)
                    {
                        s.AppendLine();
                    }
                }

                if (brk)
                {
                    break;
                }

                var sw = new StreamWriter(vImg);
                sw.Write(s.ToString());
                sw.Close();
                sw.Dispose();
                GC.Collect();
            }
        }

        private void PixelIntensityAction()
        {
            SalvarVetorDePixels();

            var dirs = new DirectoryInfo(rootDirectory).GetDirectories();
            var sb = new StringBuilder();

            foreach (var dir in dirs)
            {
                if (dir.Name[0] == '_')
                {
                    continue;
                }

                using (var sr = new StreamReader(dir.FullName + @"\vTemplates_" + dir.Name + ".txt"))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                    }
                }
            }

            sb.Remove(sb.Length - 1, 1);
            var sw = new StreamWriter(rootDirectory + @"\Templates.txt");
            sw.Write(sb.ToString());
            sw.Close();
            sw.Dispose();
            GC.Collect();
        }

        private void PixelIntensity_Click(object sender, EventArgs e)
        {
            PixelIntensityAction();
            MessageBox.Show("Pronto.");
        }

        private void TreinamentoRede_Load(object sender, EventArgs e)
        {

        }
    }
}