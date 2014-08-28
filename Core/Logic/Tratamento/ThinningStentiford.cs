using System.Collections.Generic;
using System.Drawing;
using Core.Logic.Types;

namespace Core.Logic.Tratamento
{
    public class ConditionT1 : IConditionalBool
    {
        #region ConditionalBool Members

        public bool Apply(Neighbors neighbors)
        {
            IConditionalInt find01Pattern = new Find01Pattern();
            IConditionalInt countFilledNeighbors = new CountFilledNeighbors();

            return neighbors.P2 == 0 && neighbors.P6 == 1 && countFilledNeighbors.Apply(neighbors) != 0 &&
                   find01Pattern.Apply(neighbors) == 1;
        }

        #endregion
    }

    public class ConditionT2 : IConditionalBool
    {
        #region ConditionalBool Members

        public bool Apply(Neighbors neighbors)
        {
            IConditionalInt find01Pattern = new Find01Pattern();
            IConditionalInt countFilledNeighbors = new CountFilledNeighbors();

            return neighbors.P8 == 0 && neighbors.P4 == 1 && countFilledNeighbors.Apply(neighbors) != 0 &&
                   find01Pattern.Apply(neighbors) == 1;
        }

        #endregion
    }

    public class ConditionT3 : IConditionalBool
    {
        #region ConditionalBool Members

        public bool Apply(Neighbors neighbors)
        {
            IConditionalInt find01Pattern = new Find01Pattern();
            IConditionalInt countFilledNeighbors = new CountFilledNeighbors();

            return neighbors.P2 == 1 && neighbors.P6 == 0 && countFilledNeighbors.Apply(neighbors) != 0 &&
                   find01Pattern.Apply(neighbors) == 1;
        }

        #endregion
    }

    public class ConditionT4 : IConditionalBool
    {
        #region ConditionalBool Members

        public bool Apply(Neighbors neighbors)
        {
            IConditionalInt find01Pattern = new Find01Pattern();
            IConditionalInt countFilledNeighbors = new CountFilledNeighbors();

            return neighbors.P8 == 1 && neighbors.P4 == 0 && countFilledNeighbors.Apply(neighbors) != 0 &&
                   find01Pattern.Apply(neighbors) == 1;
        }

        #endregion
    }

    public class Template1 : IConditionalBool
    {
        #region ConditionalBool Members

        public bool Apply(Neighbors neighbors)
        {
            return neighbors.P2 == 0 && neighbors.P6 == 1;
        }

        #endregion
    }

    public class Template2 : IConditionalBool
    {
        #region ConditionalBool Members

        public bool Apply(Neighbors neighbors)
        {
            return neighbors.P8 == 0 && neighbors.P4 == 1;
        }

        #endregion
    }

    public class Template3 : IConditionalBool
    {
        #region ConditionalBool Members

        public bool Apply(Neighbors neighbors)
        {
            return neighbors.P2 == 1 && neighbors.P6 == 0;
        }

        #endregion
    }

    public class Template4 : IConditionalBool
    {
        #region ConditionalBool Members

        public bool Apply(Neighbors neighbors)
        {
            return neighbors.P8 == 1 && neighbors.P4 == 0;
        }

        #endregion
    }

    public class ThinningStentiford : IThinning
    {
        #region Thinning Members

        public ImgArray Apply(ImgArray img)
        {
            var m = img.Clone();
            IConditionalBool[] conds = {new ConditionT1(), new ConditionT2(), new ConditionT3(), new ConditionT4()};
            var matches = new List<Point>();

            do
            {
                matches.Clear();
                foreach (var cond in conds)
                {
                    for (var y = 1; y < m.Height - 1; y++)
                    {
                        for (var x = 1; x < m.Width - 1; x++)
                        {
                            if (m.GetPixel(x, y).IsBlackPixel() &&
                                cond.Apply(new Neighbors(m, new Point {X = x, Y = y})))
                            {
                                matches.Add(new Point {X = x, Y = y});
                            }
                        }
                    }
                }

                foreach (var p in matches)
                {
                    m.SetPixel(p.X, p.Y, Color.FromArgb(255, 255, 255));
                }
            } while (matches.Count > 0);

            return m;
        }

        #endregion
    }

    public class ThinningStentiford2 : IThinning
    {
        #region Thinning Members

        public ImgArray Apply(ImgArray img)
        {
            var imgBin = img.Clone();
            IConditionalBool[] conds = {new Template1(), new Template2(), new Template3(), new Template4()};

            var outBinary = new ImgArray(imgBin.Width, imgBin.Height);

            var con = 5;
            var template = 0;
            IConditionalInt find01Pattern = new Find01Pattern();
            IConditionalInt countFilledNeighbors = new EndPoint();

            while (con < 15)
            {
                var cond = conds[template];

                for (var y = 1; y < imgBin.Height - 1; y++)
                {
                    for (var x = 1; x < imgBin.Width - 1; x++)
                    {
                        var window = new Neighbors(imgBin, new Point {X = x, Y = y});
                        if (imgBin.GetPixel(x, y).IsBlackPixel() && cond.Apply(window))
                        {
                            var cn = find01Pattern.Apply(window);
                            if (cn == 1)
                            {
                                var endPoint = countFilledNeighbors.Apply(window);
                                if (endPoint != 0)
                                {
                                    outBinary.SetPixel(x, y, Color.Black);
                                }
                            }
                        }
                    }
                }

                var checkVal = outBinary.CountPixelsWithColor(Color.Black);
                if (checkVal == 0)
                {
                    con++;
                }

                for (var i = 0; i < imgBin.Height; i++)
                {
                    for (var j = 0; j < imgBin.Width; j++)
                    {
                        if (outBinary.GetPixel(i, j).IsBlackPixel())
                        {
                            imgBin.SetPixel(i, j, Color.White);
                        }
                    }
                }

                outBinary.Clear();
                template = (template + 1)%4;
            }

            return imgBin;
        }

        #endregion
    }
}