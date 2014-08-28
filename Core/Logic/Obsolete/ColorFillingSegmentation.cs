using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common;
using Core.Logic.Types;

namespace Core.Logic.Obsolete
{
    /*from __future__ import division
    import cv2 // OpenCV 2.3.1
    import numpy as np
    from scipy.misc.pilutil import imshow*/

    //TODO: Eliminar essa classe
    [Obsolete("Usar CFS2")]
    public class ColorFillingSegmentation
    {
        private const int UnlabeledFlag = 255;
        private readonly ImgArray image;
        private readonly int[,] labels;
        private readonly int nSegments;
        private List<Bitmap> carac;

        /// <summary>
        ///   Separa clusters por conexão de pixels
        /// </summary>
        /// <param name="imagem"> Recebe imagem com fundo BRANCO e letra preta </param>
        public ColorFillingSegmentation(ImgArray imagem)
        {
            /*
            Computes separate labels 0...254 (max) for each flood-fill disconnected segment in the binary input image (values in {0,1})
            Parameters
                connectivity - neighborhood connectivity, default: 8
            */
            if (CountNumberOfColors(imagem) == 2)
            {
                // Black and white image only 

                Init();

                image = imagem.InvertImageColors();

                var labeled = MultiplyMatrixByScalar(imagem, UnlabeledFlag);
                var colorId = 1; // Start with 1 since zero is for the background

                while (true)
                {
                    if (colorId < UnlabeledFlag)
                    {
                        // Cannot exceed flag value

                        // Take first unlabeled as new seed, if available
                        var seed = new Point(-1, -1);
                        var brk = false;


                        for (var x = 0; x < labeled.GetLength(0); x++)
                        {
                            for (var y = 0; y < labeled.GetLength(1); y++)
                            {
                                if (labeled[x, y] == UnlabeledFlag)
                                {
                                    seed = new Point
                                               {
                                                   X = x,
                                                   Y = y
                                               };
                                    brk = true;
                                    break;
                                }
                            }
                            if (brk)
                            {
                                break;
                            }
                        }

                        if (seed.X > -1)
                        {
                            labeled = FloodFill(labeled, seed, colorId);
                            // pinta cada cluster da imagem de uma cor
                            colorId += 1; // Next label
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                labels = labeled;
                nSegments = colorId;

                for (var i = 1; i < nSegments; i++)
                {
                    var segment = GetSegment(i);
                    carac.Add(segment.ToBitmap());
                }
            }
        }

        private void Init()
        {
            if (carac == null)
            {
                carac = new List<Bitmap>();
            }
        }

        public List<Bitmap> GetCaracs()
        {
            return carac;
        }

        public List<Bitmap> GetCaracsFundoBranco()
        {
            var result = carac;
            for (var i = 0; i < result.Count; i++)
            {
                result[i] = result[i].InvertImageColors().RemoveWhiteBorders();
            }
            return result;
        }

        private int CountNumberOfColors(IEnumerable img)
        {
            var l = new List<byte>();
            foreach (var i in from byte i in img where !l.Contains(i) select i)
            {
                l.Add(i);
            }
            return l.Count;
        }

        private int[,] SubMatrix(int[,] img, int id)
        {
            var sub = new int[img.GetLength(0), img.GetLength(1)];
            for (var y = 0; y < img.GetLength(1); y++)
            {
                for (var x = 0; x < img.GetLength(0); x++)
                {
                    if (img[x, y] == id)
                    {
                        sub[x, y] = 0;
                    }
                    else
                    {
                        sub[x, y] = 1;
                    }
                }
            }

            return sub;
        }

        private int[,] MultiplyMatrixByScalar(ImgArray img, int scalar)
        {
            var mult = new int[img.Width, img.Height];
            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    mult[x, y] = img.GetByte(x, y) * scalar;
                }
            }
            return mult;
        }

        private int[,] FloodFill(int[,] img, Point seed, int color)
        {
            //TODO: 30 devera ser calculado de acordo com numero de clusters = 255/clusters
            var fillColor = Color.FromArgb((30 * color) % 255, (30 * color) % 255, (30 * color) % 255);
            var bmp = img.MatrizParaBitmap(img.GetLength(0), img.GetLength(1));
            bmp = bmp.FloodFill(seed, color);
            //this.carac.Add(bmp);

            var result = new int[img.GetLength(0), img.GetLength(1)];
            for (var y = 0; y < bmp.Height; y++)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    var c = bmp.GetPixel(x, y);
                    if (c == fillColor)
                    {
                        result[x, y] = color;
                    }
                    else
                    {
                        result[x, y] = c.R;
                    }
                }
            }


            return result;
        }

        public int[,] GetLabels()
        {
            return labels;
        }

        public int CountSegments()
        {
            return nSegments;
        }

        private ImgArray GetSegment(int segmentId)
        {
            if (!(0 <= segmentId && segmentId <= nSegments))
                return null;

            var segment = new ImgArray(image.Width, image.Height);

            var region = SubMatrix(labels, segmentId);

            for (var y = 0; y < region.GetLength(1); y++)
            {
                for (var x = 0; x < region.GetLength(0); x++)
                {
                    if (region[x, y] == 1)
                    {
                        segment.SetPixel(x, y, Color.Black);
                    }
                }
            }
            return segment;
        }
    }
}