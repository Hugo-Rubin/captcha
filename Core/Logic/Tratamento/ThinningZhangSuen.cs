using System.Collections.Generic;
using System.Drawing;
using Core.Logic.Types;

namespace Core.Logic.Tratamento
{
    /*
        (a) 2 <= B(P1) <= 6
        (b) A(P1)= 1
        (C) P2*P4*P6 = 0
        (d) P4*P6*P8 = 0
     */

    public class ConditionsZhangSuen : IConditional
    {
        private readonly IConditionalInt countFilledNeighbors = new CountFilledNeighbors();
        private readonly IConditionalInt find01Pattern = new Find01Pattern();


        public bool CheckFirstConditions(Neighbors neighbors)
        {
            return CheckConditionA(neighbors) && CheckConditionB(neighbors) && CheckConditionC(neighbors) &&
                   CheckConditionD(neighbors);
        }

        public bool CheckSecondConditions(Neighbors neighbors)
        {
            return CheckConditionA(neighbors) && CheckConditionB(neighbors) && CheckConditionC2(neighbors) &&
                   CheckConditionD2(neighbors);
        }

        private bool CheckConditionA(Neighbors neighbors)
        {
            return find01Pattern.Apply(neighbors) == 1;
        }

        private bool CheckConditionB(Neighbors neighbors)
        {
            var result = countFilledNeighbors.Apply(neighbors);
            return result >= 2 && result <= 6;
        }

        private bool CheckConditionC(Neighbors neighbors)
        {
            return (neighbors.P2 * neighbors.P4 * neighbors.P6) == 0;
        }

        private bool CheckConditionD(Neighbors neighbors)
        {
            return (neighbors.P4 * neighbors.P6 * neighbors.P8) == 0;
        }

        private bool CheckConditionC2(Neighbors neighbors)
        {
            return (neighbors.P2 * neighbors.P4 * neighbors.P8) == 0;
        }

        private bool CheckConditionD2(Neighbors neighbors)
        {
            return (neighbors.P2 * neighbors.P6 * neighbors.P8) == 0;
        }
    }

    public class ThinningZhangSuen : IThinning
    {
        #region Thinning Members

        public ImgArray Apply(ImgArray img)
        {
            var m = img.Clone();
            var cond = new ConditionsZhangSuen();
            //int count = 0;

            var matches = new List<Point>();

            while (true)
            {
                var c = 0;
                for (var y = 1; y < m.Height - 1; y++)
                {
                    for (var x = 1; x < m.Width - 1; x++)
                    {
                        if (m.GetPixel(x, y).IsBlackPixel() &&
                            cond.CheckFirstConditions(new Neighbors(m, new Point { X = x, Y = y })))
                        {
                            c++;
                            matches.Add(new Point { X = x, Y = y });
                        }
                    }
                }

                foreach (var p in matches)
                {
                    m.SetPixel(p.X, p.Y, Color.FromArgb(255, 255, 255));
                }
                matches.Clear();

                if (c == 0)
                {
                    return m;
                }

                c = 0;
                for (var y2 = 1; y2 < m.Height - 1; y2++)
                {
                    for (var x2 = 1; x2 < m.Width - 1; x2++)
                    {
                        if (m.GetPixel(x2, y2).IsBlackPixel() &&
                            cond.CheckSecondConditions(new Neighbors(m, new Point { X = x2, Y = y2 })))
                        {
                            c++;
                            matches.Add(new Point { X = x2, Y = y2 });
                        }
                    }
                }

                foreach (var p in matches)
                {
                    m.SetPixel(p.X, p.Y, Color.FromArgb(255, 255, 255));
                }
                matches.Clear();

                if (c == 0)
                {
                    return m;
                }

                //M.Save(@"C:\Users\Hugo\Desktop\MH1" + (count++) + ".png");
            }
        }

        #endregion
    }
}