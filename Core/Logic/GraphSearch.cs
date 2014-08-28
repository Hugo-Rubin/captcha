using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Core.Logic.Tratamento;
using Core.Logic.Types;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace Core.Logic
{
    /// <summary>
    ///   http://www.codeproject.com/Articles/5603/QuickGraph-A-100-C-graph-library-with-Graphviz-Sup
    ///   http://quickgraph.codeplex.com/wikipage?title=User%20Manual
    /// </summary>
    public class GraphSearch
    {
        private readonly ImgArray source;
        private Point root;

        public GraphSearch(ImgArray img)
        {
            source = img;
            Graphs = new AdjacencyGraph<Point, Edge<Point>>();
            InitGraph();
        }

        public AdjacencyGraph<Point, Edge<Point>> Graphs { get; set; }

        private void InitGraph()
        {
            root = source.GetMinXPoint();
            //target = source.GetMaxXPoint();
            Graphs.AddVertex(root);
        }

        private List<Point> GetNeighborNodes(Point node)
        {
            var n = new Neighbors(source, node);
            var result = new List<Point>();
            if (n.P2 != 0)
            {
                result.Add(new Point(node.X, node.Y - 1));
            }
            if (n.P3 != 0)
            {
                result.Add(new Point(node.X + 1, node.Y - 1));
            }
            if (n.P4 != 0)
            {
                result.Add(new Point(node.X + 1, node.Y));
            }
            if (n.P5 != 0)
            {
                result.Add(new Point(node.X + 1, node.Y + 1));
            }
            if (n.P6 != 0)
            {
                result.Add(new Point(node.X, node.Y + 1));
            }

            return result;
        }

        public void FindGraphs()
        {
            var n = Graphs.Vertices.First();
            var unexplored = new List<Point>();
            var explored = new List<Point>();
            unexplored.Add(n);

            while (unexplored.Count != 0)
            {
                var neighborhood = GetNeighborNodes(n);
                ExpandNode(n, neighborhood);
                unexplored.AddRange(
                    neighborhood.FindAll(p => !unexplored.Contains(p) && !explored.Contains(p))
                    );

                unexplored.Remove(n);
                if (!explored.Contains(n))
                {
                    explored.Add(n);
                }
                n = unexplored.FirstOrDefault();
            }
        }

        private void ExpandNode(Point node, IEnumerable<Point> neighborhood)
        {
            foreach (var neighbor in neighborhood)
            {
                var edge = new Edge<Point>(node, neighbor);
                if (!Graphs.ContainsVertex(neighbor))
                {
                    Graphs.AddVertex(neighbor);
                }
                if (!Graphs.ContainsEdge(edge))
                {
                    Graphs.AddEdge(edge);
                }
            }
        }

        public IEnumerable<List<Point>> GetLongestPaths()
        {
            var dij = new GraphProcesserDijkstra<Point, Edge<Point>>(Graphs);
            var distances = dij.GetLongestPath(root);

            var max = distances.Values.Max();
            var maxPaths = distances.Where(d => d.Value == max);

            return maxPaths.Select(fim => dij.GetPathTo(fim.Key)).ToList();
        }

        public void PrintGraph(String imgDir = @"C:\graph.png")
        {
            var graphviz = new GraphvizAlgorithm<Point, Edge<Point>>(Graphs) { ImageType = GraphvizImageType.Png };
            var generator = new BitmapGeneratorDotEngine();
            var output = graphviz.Generate(generator, "grafo");
            Image.FromStream(new MemoryStream(Encoding.Default.GetBytes(output))).Save(imgDir);
        }

        public void PrintLongestPaths(String imgDir = @"C:\")
        {
            var dij = new GraphProcesserDijkstra<Point, Edge<Point>>(Graphs);
            var distances = dij.GetLongestPath(root);

            var max = distances.Values.Max();
            var maxPaths = distances.Where(d => d.Value == max);

            foreach (var fim in maxPaths)
            {
                var p = fim.Key;
                var w = 10 + (p.X > root.X ? p.X : root.X);
                var h = 10 + (p.Y > root.Y ? p.Y : root.Y);
                var res = new ImgArray(w, h);

                var pontos = dij.GetPathTo(p);

                foreach (var pa in pontos)
                {
                    res.SetPixel(pa.X, pa.Y, Color.Black);
                }

                res.Save(String.Format(imgDir + @"{0}{1}.png", p.X, p.Y));
            }
        }

        #region Nested type: BitmapGeneratorDotEngine

        public class BitmapGeneratorDotEngine : IDotEngine
        {
            private readonly byte[] buffer = new byte[4096];
            private readonly MemoryStream memoryStream = new MemoryStream();
            private Stream standardOutput;

            #region IDotEngine Members

            public string Run(GraphvizImageType imageType, string dot, string outputFileName)
            {
                using (var process = new Process())
                {
                    //We'll launch dot.exe in command line
                    process.StartInfo.FileName = @"D:\Programas\Graphviz2.32\bin\dot.exe";
                    //Let's give the type we want to generate to, and a charset
                    //to support accent
                    process.StartInfo.Arguments = "-Tgif -Gcharset=latin1";
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

                    //We'll receive the bitmap thru the standard output stream
                    process.StartInfo.RedirectStandardOutput = true;
                    //We'll need to give the dot structure in the standard input stream
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    standardOutput = process.StandardOutput.BaseStream;
                    standardOutput.BeginRead(buffer, 0, buffer.Length, StandardOutputReadCallback, null);
                    //Let's sent the dot structure and close the stream to send the data and tell
                    //we won't give any more
                    process.StandardInput.Write(dot);
                    process.StandardInput.Close();
                    //Wait the process is finished and get back the image (binary format)
                    process.WaitForExit();

                    //Image.FromStream(memoryStream).Save(@"C:\Users\Hugo\Desktop\stream1.png");
                    return Encoding.Default.GetString(memoryStream.ToArray());
                }
            }

            #endregion

            private void StandardOutputReadCallback(IAsyncResult result)
            {
                var numberOfBytesRead = standardOutput.EndRead(result);
                memoryStream.Write(buffer, 0, numberOfBytesRead);

                // Read next bytes.   
                standardOutput.BeginRead(buffer, 0, buffer.Length, StandardOutputReadCallback, null);
            }
        }

        #endregion
    }
}