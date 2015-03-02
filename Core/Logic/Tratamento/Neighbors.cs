using System.Collections.Generic;
using System.Drawing;
using Core.Logic.Types;

namespace Core.Logic.Tratamento
{
    public class Neighbors
    {
        private int p2;
        public Point p2Position;
        
        private int p3;
        public Point p3Position;

        private int p4;
        public Point p4Position;

        private int p5;
        public Point p5Position;

        private int p6;
        public Point p6Position;

        private int p7;
        public Point p7Position;

        private int p8;
        public Point p8Position;

        private int p9;
        public Point p9Position;

        public Neighbors(ImgArray img, Point p1, bool invertValues = true)
        {
            var x = p1.X;
            var y = p1.Y;

            p2Position = new Point(x, y - 1);
            p3Position = new Point(x + 1, y - 1);
            p4Position = new Point(x + 1, y);
            p5Position = new Point(x + 1, y + 1);
            p6Position = new Point(x, y + 1);
            p7Position = new Point(x - 1, y + 1);
            p8Position = new Point(x - 1, y);
            p9Position = new Point(x - 1, y - 1);

            if (invertValues)
            {
                P1 = img.GetPixel(x, y).IsBlackPixel() ? 1 : 0;
                p2 = img.GetPixel(x, y - 1).IsBlackPixel() ? 1 : 0;
                p3 = img.GetPixel(x + 1, y - 1).IsBlackPixel() ? 1 : 0;
                p4 = img.GetPixel(x + 1, y).IsBlackPixel() ? 1 : 0;
                p5 = img.GetPixel(x + 1, y + 1).IsBlackPixel() ? 1 : 0;
                p6 = img.GetPixel(x, y + 1).IsBlackPixel() ? 1 : 0;
                p7 = img.GetPixel(x - 1, y + 1).IsBlackPixel() ? 1 : 0;
                p8 = img.GetPixel(x - 1, y).IsBlackPixel() ? 1 : 0;
                p9 = img.GetPixel(x - 1, y - 1).IsBlackPixel() ? 1 : 0;
            }
            else
            {
                P1 = GetValue(img, x, y);
                p2 = GetValue(img, x, y - 1);
                p3 = GetValue(img, x + 1, y - 1);
                p4 = GetValue(img, x + 1, y);
                p5 = GetValue(img, x + 1, y + 1);
                p6 = GetValue(img, x, y + 1);
                p7 = GetValue(img, x - 1, y + 1);
                p8 = GetValue(img, x - 1, y);
                p9 = GetValue(img, x - 1, y - 1);
            }
        }

        public int P1 { get; set; }

        public int P2
        {
            get { return p2; }
            set { p2 = value; }
        }

        public int P3
        {
            get { return p3; }
            set { p3 = value; }
        }

        public int P4
        {
            get { return p4; }
            set { p4 = value; }
        }

        public int P5
        {
            get { return p5; }
            set { p5 = value; }
        }

        public int P6
        {
            get { return p6; }
            set { p6 = value; }
        }

        public int P7
        {
            get { return p7; }
            set { p7 = value; }
        }

        public int P8
        {
            get { return p8; }
            set { p8 = value; }
        }

        public int P9
        {
            get { return p9; }
            set { p9 = value; }
        }

        private int GetValue(ImgArray img, int x, int y)
        {
            const int xMin = 0;
            const int yMin = 0;
            var xMax = img.Width - 1;
            var yMax = img.Height - 1;

            if (x < xMin || y < yMin || x > xMax || y > yMax)
            {
                return -1;
            }

            return img.GetPixel(x, y).R == 255 ? 1 : 0;
        }

        public Stack<int> ToStack()
        {
            var stack = new Stack<int>();

            stack.Push(p9);
            stack.Push(p8);
            stack.Push(p7);
            stack.Push(p6);
            stack.Push(p5);
            stack.Push(p4);
            stack.Push(p3);
            stack.Push(p2);

            return stack;
        }
    }
}