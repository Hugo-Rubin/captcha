using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BLL.SpectralCluster
{
    public class SCPointList : IList
    {
        private List<SCPoint> points = new List<SCPoint>();

        public SCPoint getNearestPoint(int k, SCPoint test)
        {
            int index = 0;
            SortedList distances = new SortedList();
            foreach (SCPoint myPoint in this)
            {
                distances.Add(index, myPoint.distance(test));
                index++;
            }
            //var orderByVal = distances.OrderBy(kvp => kvp.Value);
            //return this[orderByVal.ElementAt(k - 1).Key];
            return (SCPoint)distances.GetKey(k - 1);
        }


        public int Add(object value)
        {
            this.points.Add((SCPoint)value);
            return this.points.IndexOf((SCPoint)value);
        }

        public void Clear()
        {
            this.points.Clear();
        }

        public bool Contains(object value)
        {
            return this.points.Contains(value);
        }

        public int IndexOf(object value)
        {
            return this.points.IndexOf((SCPoint)value);
        }

        public void Insert(int index, object value)
        {
            this.points.Insert(index, (SCPoint)value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void Remove(object value)
        {
            this.points.Remove((SCPoint)value);
        }

        public void RemoveAt(int index)
        {
            this.points.RemoveAt(index);
        }

        public object this[int index]
        {
            get
            {
                return this.points[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return this.points.Count(); }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
