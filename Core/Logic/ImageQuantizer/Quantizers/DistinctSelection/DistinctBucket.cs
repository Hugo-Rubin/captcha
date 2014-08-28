using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Core.Logic.ImageQuantizer.Quantizers.DistinctSelection
{
    public class DistinctBucket
    {
        public DistinctBucket()
        {
            Buckets = new DistinctBucket[16];
        }

        public DistinctColorInfo ColorInfo { get; private set; }
        public DistinctBucket[] Buckets { get; private set; }

        public void StoreColor(Color color)
        {
            var redIndex = color.R >> 5;
            var redBucket = Buckets[redIndex];

            if (redBucket == null)
            {
                redBucket = new DistinctBucket();
                Buckets[redIndex] = redBucket;
            }

            var greenIndex = color.G >> 5;
            var greenBucket = redBucket.Buckets[greenIndex];

            if (greenBucket == null)
            {
                greenBucket = new DistinctBucket();
                redBucket.Buckets[greenIndex] = greenBucket;
            }

            var blueIndex = color.B >> 5;
            var blueBucket = greenBucket.Buckets[blueIndex];

            if (blueBucket == null)
            {
                blueBucket = new DistinctBucket();
                greenBucket.Buckets[blueIndex] = blueBucket;
            }

            var colorInfo = blueBucket.ColorInfo;

            if (colorInfo == null)
            {
                colorInfo = new DistinctColorInfo(color);
                blueBucket.ColorInfo = colorInfo;
            }
            else
            {
                colorInfo.IncreaseCount();
            }
        }

        public List<DistinctColorInfo> GetValues()
        {
            return Buckets.Where(red => red != null).
                SelectMany(redBucket => redBucket.Buckets.
                                            Where(green => green != null), (redBucket, greenBucket) => greenBucket).
                SelectMany(greenBucket => greenBucket.Buckets.
                                              Where(blue => blue != null),
                           (greenBucket, blueBucket) => blueBucket.ColorInfo).
                ToList();
        }
    }
}