using System.Collections.Generic;

namespace Core.Logic.Types
{
    public class ChaveValor<T, TZ>
    {
        public T Chave { get; set; }
        public TZ Valor { get; set; }
    }

    internal class ChaveValorEqualityComparer : IEqualityComparer<ChaveValor<int, int>>
    {
        #region IEqualityComparer<ChaveValor<int,int>> Members

        public bool Equals(ChaveValor<int, int> cv1, ChaveValor<int, int> cv2)
        {
            if (cv1.Chave == cv2.Chave & cv1.Valor == cv2.Valor)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(ChaveValor<int, int> cvX)
        {
            var hCode = cvX.Chave ^ cvX.Valor;
            return hCode.GetHashCode();
        }

        #endregion
    }
}