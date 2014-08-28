using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Core.Common;

namespace Core.Logic.Types
{
    /// <summary>
    ///   Classe que representa imagem em escala de cinza, representada por um vetor simplificado, onde:
    ///   Indice par representa Quantidade de pixels brancos
    ///   Indice impar representa Quantidade de pixels pretos
    /// </summary>
    [Serializable]
    public class NanoArray : IEnumerator, IEnumerable
    {
        private readonly int height;
        private readonly int[] intNanoArray;

        private readonly int width;
        private int position = -1;

        public NanoArray(Bitmap bmpSource)
        {
            width = bmpSource.Width;
            height = bmpSource.Height;
            intNanoArray = bmpSource.NanoPixelIntensity();
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int this[int i]
        {
            get { return intNanoArray[i]; }
            set { intNanoArray[i] = value; }
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion

        public static Bitmap BitmapFromNanoArray(int[] imgNanoArray, int width, int height)
        {
            var lista = new List<int>();
            var idx = 0;
            // Converter NanoArray em imgArray
            foreach (var item in imgNanoArray)
            {
                // Indice Par = Brancos
                if (idx % 2 == 0)
                {
                    for (var i = 0; i < item; i++)
                    {
                        lista.Add(1);
                    }
                }
                else
                {
                    //Indice impar = Pretos
                    for (var i = 0; i < item; i++)
                    {
                        lista.Add(0);
                    }
                }
                idx++;
            }

            var result = lista.ToArray();

            return result.BitmapFromArray(width, height);
        }

        public int[] GetInternalArray()
        {
            return intNanoArray;
        }

        #region Implementação de IEnumerator

        public object Current
        {
            get { return intNanoArray[position]; }
        }

        public bool MoveNext()
        {
            position++;
            return (position < intNanoArray.Length);
        }

        public void Reset()
        {
            position = 0;
        }

        #endregion
    }
}