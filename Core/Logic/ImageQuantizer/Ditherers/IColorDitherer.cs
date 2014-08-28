using System;
using Core.Logic.ImageQuantizer.Helpers;
using Core.Logic.ImageQuantizer.PathProviders;
using Core.Logic.ImageQuantizer.Quantizers;

namespace Core.Logic.ImageQuantizer.Ditherers
{
    public interface IColorDitherer : IPathProvider
    {
        /// <summary>
        ///   Gets a value indicating whether this ditherer uses only actually process pixel.
        /// </summary>
        /// <value> <c>true</c> if this ditherer is inplace; otherwise, <c>false</c> . </value>
        Boolean IsInplace { get; }

        /// <summary>
        ///   Prepares this instance.
        /// </summary>
        void Prepare(IColorQuantizer quantizer, Int32 colorCount, ImageBuffer sourceBuffer, ImageBuffer targetBuffer);

        /// <summary>
        ///   Processes the specified buffer.
        /// </summary>
        Boolean ProcessPixel(Pixel sourcePixel, Pixel targetPixel);

        /// <summary>
        ///   Finishes this instance.
        /// </summary>
        void Finish();
    }
}