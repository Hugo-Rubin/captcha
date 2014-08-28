using System;
using System.Linq;
using Core.Logic.Types;

namespace Core.Logic.Tratamento
{
    public interface IThinning
    {
        ImgArray Apply(ImgArray img);
    }

    public interface IConditionalBool : IConditional
    {
        bool Apply(Neighbors neigbors);
    }

    public interface IConditionalInt : IConditional
    {
        int Apply(Neighbors neigbors);
    }

    public class EndPoint : IConditionalInt
    {
        #region ConditionalInt Members

        public int Apply(Neighbors neighbors)
        {
            return Math.Abs(neighbors.P1 - neighbors.ToStack().Sum());
        }

        #endregion
    }

    public class CountFilledNeighbors : IConditionalInt
    {
        #region ConditionalInt Members

        public int Apply(Neighbors neighbors)
        {
            return neighbors.ToStack().Sum();
        }

        #endregion
    }

    public class Find01Pattern : IConditionalInt
    {
        #region ConditionalInt Members

        public int Apply(Neighbors neighbors)
        {
            var count = 0;
            var neighborhood = neighbors.ToStack();
            var firstElement = neighborhood.Pop();
            var previousElement = firstElement;
            do
            {
                var currentElement = neighborhood.Pop();

                if (previousElement == 0 && currentElement == 1)
                {
                    count++;
                }
                previousElement = currentElement;
            } while (neighborhood.Count != 0);

            if (firstElement == 1 && previousElement == 0) // Compara P2 e P9
            {
                count++;
            }

            return count;
        }

        #endregion
    }
}