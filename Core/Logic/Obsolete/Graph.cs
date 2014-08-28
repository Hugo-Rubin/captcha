using System.Collections.Generic;
using System.Drawing;
using Core.Logic.Types;

namespace Core.Logic.Obsolete
{
    public class Graph
    {
        public Vertice[,] GrafoMx;

        public Graph(Vertice[,] gMx)
        {
            GrafoMx = gMx;
        }

        public Vertice MontaVertice(Point p, ImgArray img)
        {
            //evitar criar um grafo que já foi criado no caso das interseccoes das linhas 
            if (GrafoMx[p.X, p.Y] == null)
            {
                return MontaGrafo(p, img);
            }

            return GrafoMx[p.X, p.Y];
        }

        public Vertice MontaGrafo(Point p, ImgArray img)
        {
            var x = p.X;
            var y = p.Y;

            var verticesArestas = new List<Vertice>(5); //Numero maximo de arestas do grafo é 5

            if (img.GetPixel(x, y - 1).IsBlackPixel())
            {
                verticesArestas.Add(MontaVertice(new Point(x, y - 1), img));
            }
            if (img.GetPixel(x + 1, y - 1).IsBlackPixel())
            {
                verticesArestas.Add(MontaVertice(new Point(x + 1, y - 1), img));
            }
            if (img.GetPixel(x + 1, y).IsBlackPixel())
            {
                verticesArestas.Add(MontaVertice(new Point(x + 1, y), img));
            }
            if (img.GetPixel(x + 1, y + 1).IsBlackPixel())
            {
                verticesArestas.Add(MontaVertice(new Point(x + 1, y + 1), img));
            }
            if (img.GetPixel(x, y + 1).IsBlackPixel())
            {
                verticesArestas.Add(MontaVertice(new Point(x, y + 1), img));
            }

            //importante criar o vertice por ultimo para evitar o consumo de memoria na recursividade
            var vertice = new Vertice {Arestas = verticesArestas};

            GrafoMx[x, y] = vertice;

            return vertice;
        }

        #region Nested type: Vertice

        public class Vertice
        {
            public List<Vertice> Arestas;
        }

        #endregion
    }
}