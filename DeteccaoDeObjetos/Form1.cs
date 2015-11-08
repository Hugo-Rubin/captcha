using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Logic;
using System.IO;

namespace DeteccaoDeObjetos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap img = new Bitmap(@"C:\Users\Hugo\Chromatic\B_MRKU4389620 (5)_Chromatic.png");
             Bitmap certinha = new Bitmap(@"C:\Users\Hugo\Chromatic\B_MRKU4389620 (5)_ChromaticLN.png");

            var output = new Bitmap(img.Width, img.Height);
            
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color c = img.GetPixel(x, y);

                    if(c.B < 90 || c.R > 60) 
                    {
                        img.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                       
                        output.SetPixel(x, y, certinha.GetPixel(x,y));
                    }
                }
            }



            img.Save(@"C:\Users\Hugo\Chromatic\lacre\lacre2.png");
            output.Save(@"C:\Users\Hugo\Chromatic\lacre\lacre2bonitao.png");

            }
        }
    
}
