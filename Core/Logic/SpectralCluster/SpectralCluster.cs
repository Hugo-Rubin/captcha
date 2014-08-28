using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MathNet.Numerics.LinearAlgebra;
//using MathNet.Numerics.LinearAlgebra.Double;

using MathNet.Numerics;


namespace BLL.SpectralCluster
{
    //public class SpectralCluster
    //{
    //    private int[] groups;
    //    private double[] xData;
    //    private double[] yData;
    //    private int K;
    //    private SCPointList Points = new SCPointList();

    //    public int[] Groups
    //    {
    //        get { return groups; }
    //    }

    //    public SpectralCluster(double[] data1, double[] data2, int k)
    //    {
    //        //both arrays must be same length.
    //        for (int i = 0; i < data1.Length; i++)
    //        {
    //            Points.Add(new SCPoint(data1[i], data2[i]));
    //        }
    //        xData = data1;
    //        yData = data2;
    //        K = k;
    //        groups = new int[data1.Length];
    //        calculate();
    //    }

    //    private void calculate()
    //    {
    //        //Step 1: Affinity matrix
    //        Matrix<double> affinity = calcAffinityMatrix(xData, yData);
    //        //Step 2: Degree matrix
    //        double[] diag = new double[xData.Length];
    //        for (int row = 0; row < affinity.RowCount; row++)
    //        {
    //            diag[row] = Math.Pow(affinity.Row(row).Sum(), -0.5);
    //        }
    //        //degree to the -1/2
    //        Matrix<double> DMod = new DenseMatrix(xData.Length, yData.Length, 0);
    //        DMod.SetDiagonal(diag);
    //        //L Matrix
    //        Matrix<double> L = DMod.Multiply(affinity.Multiply(DMod));
    //        //Step 3:Do the svd.
    //        Matrix<double> eigenvectors = L.Svd(true).U();
    //        //Get required eigenvectors
    //        Matrix<double> selected = eigenvectors.SubMatrix(0, eigenvectors.RowCount - 1, 0, K);
    //        //Step 4:Normalise the selected matrix
    //        for (int sRow = 0; sRow < selected.RowCount; sRow++)
    //        {
    //            double[] rowData = selected.Row(sRow).ToArray<double>();
    //            selected.SetRow(sRow, selected.Row(sRow).Divide(normalise(rowData)));
    //        }
    //        //Step 5:Do Kmeans on result
    //        Kmeans result = new Kmeans(K, selected);
    //        groups = result.Groups;
    //    }

    //    public SCPoint getNearestPoint(int k, SCPoint test)
    //    {
    //        int index = 0;
    //        SortedList distances = new SortedList();
    //        foreach (SCPoint myPoint in this.Points)
    //        {
    //            distances.Add(index, myPoint.distance(test));
    //            index++;
    //        }
    //        //var orderByVal = distances.OrderBy(kvp => kvp.Value);
    //        //return (SCPoint)this[orderByVal.ElementAt(k - 1).Key];
    //        return (SCPoint)distances.GetKey(k - 1);
    //    }
    //    private Matrix calcAffinityMatrix(double[] data1, double[] data2)
    //    {
    //        //data1 and data2 must contain the same number of points
    //        DenseMatrix result = new DenseMatrix(data1.Length, data1.Length, 0);
    //        for (int i = 0; i < data1.Length; i++)
    //        {
    //            for (int j = 0; j < data1.Length; j++)
    //            {
    //                if (i == j)
    //                {
    //                    result[i, j] = 0;
    //                }
    //                else
    //                {
    //                    double sigmaI = 0;
    //                    double sigmaJ = 0;
    //                    SCPoint firstPoint = new SCPoint(data1[i], data2[i]);
    //                    SCPoint secondPoint = new SCPoint(data1[j], data2[j]);
    //                    SCPoint thing = this.Points.getNearestPoint(7, firstPoint);
    //                    sigmaI = firstPoint.distance(thing);
    //                    thing = this.Points.getNearestPoint(7, secondPoint);
    //                    sigmaJ = secondPoint.distance(thing);
    //                    double dist = firstPoint.squaredDistance(secondPoint);
    //                    result[i, j] = Math.Exp(-dist / (sigmaI * sigmaJ));
    //                }
    //            }
    //        }
    //        return result;
    //    }
    //}
}
