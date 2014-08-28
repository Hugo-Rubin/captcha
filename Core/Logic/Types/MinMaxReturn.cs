using System.Linq;

namespace Core.Logic.Types
{
    public class MinMaxReturn
    {
        public MinMaxReturn(int[] numbers)
        {
            Min = numbers.Min();
            Max = numbers.Max();
        }

        public int Min { get; set; }
        public int Max { get; set; }

        public int GetDelta()
        {
            return Max - Min;
        }

        public int GetSum()
        {
            return Max + Min;
        }
    }
}