using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Core.Logic.ImageLevels
{
    //ThreadSafe Singleton ImageWindow class
    public sealed class ImageWindow
    {
        private static volatile ImageWindow instance;
        private static readonly object SyncRoot = new Object(); //locking object

        //

        private BitmapSource origBitmapSource; //original bms for full image

        public ImageWindow()
        {
            ImgImage = new Image();
        }

        public static ImageWindow Instance //public static constructor
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                            instance = new ImageWindow(); //invoke private INSTANCE constructor from static ctor
                    }
                }
                return instance;
            }
        }

        public string ImgFileName { get; private set; }

        public byte[] OrigRgb { get; private set; }

        public byte[] ResizedRgb { get; private set; }

        public Image ImgImage { get; private set; }

        public bool ImgLoaded { get; private set; }

        public bool ImgWindowAvalable { get; private set; }

        private void ImgWindowInit()
        {
            ImgImage = new Image();
            ImgWindowAvalable = true;
        }

        //ImgWindowInit()

        private static byte[] GetRgbData(BitmapSource bms)
        {
            //calc its image stride
            var stride = bms.PixelWidth * ((bms.Format.BitsPerPixel + 7) / 8);
            //calc its image data length
            var dataLength = stride * bms.PixelHeight;
            //save orignal bms data needed to clone a BitmapSource using new passed data

            var rgb = new byte[dataLength];
            bms.CopyPixels(rgb, stride, 0);
            return rgb;
        }

        //rbm is the resized BitmapImage that has already done a BeginInit()
        //pix Height and width are the Real pixel dimension of the original image
        //modify either the DecodePixelHeight or width if necessary
        private void SetBitmapSize(Double pixHeight, Double pixWidth)
        {
            //Set the width and Height of image to avoid streching
            ImgImage.Width = pixWidth;
            ImgImage.Height = pixHeight;
        }

        public void LoadFileImage(string imageFileName)
        {
            if (!ImgWindowAvalable) ImgWindowInit();
            ImgFileName = imageFileName;
            var ms = new MemoryStream();
            var stream = new FileStream(imageFileName, FileMode.Open, FileAccess.Read);
            ms.SetLength(stream.Length);
            stream.Read(ms.GetBuffer(), 0, (int)stream.Length); //read entire image file into memory stream
            ms.Seek(0, SeekOrigin.Begin); //come up as pos zero, don't need this
            stream.Close(); //close image file

            var decoder = BitmapDecoder.Create( //decoder to read ms
                ms,
                BitmapCreateOptions.None,
                BitmapCacheOption.Default); //this is changing ms Position to 38+K
            origBitmapSource = decoder.Frames[0];
            OrigRgb = GetRgbData(origBitmapSource);

            var resizedBitmap = new BitmapImage();
            ms.Seek(0, SeekOrigin.Begin); //RESTORE stream POSITION!

            resizedBitmap.BeginInit();
            SetBitmapSize(origBitmapSource.PixelHeight, origBitmapSource.PixelWidth);
            resizedBitmap.StreamSource = ms;
            resizedBitmap.EndInit();
            ResizedRgb = GetRgbData(resizedBitmap); //1/10TH THE SIZE, TRY IT!!!!!
            ImgImage.Source = resizedBitmap;
            ImgLoaded = true;
        }

        public void SaveFileImage(string fileName, byte[] modRgb)
        {
            if (!ImgLoaded) return;
            var stream = new FileStream(fileName, FileMode.Create);
            var encoder = new JpegBitmapEncoder {QualityLevel = 100};
            encoder.Frames.Add(BitmapFrame.Create(CloneOrigBms(modRgb)));
            encoder.Save(stream);
            stream.Close();

            //MemoryStream ms = new MemoryStream(BitmapFrame.Create(CloneOrigBms(modRgb)));

            //System.Drawing.Bitmap bmp = (System.Drawing.Bitmap) System.Drawing.Bitmap.FromStream(CloneOrigBms(ms), true, true);
        }

        //SaveFileImage()


        public void UpdateImage(byte[] newRgbData)
        {
            //Using the currently displayed resized image
            var imageBms = (BitmapSource)ImgImage.Source;
            //calc its image stride
            var stride = imageBms.PixelWidth * ((imageBms.Format.BitsPerPixel + 7) / 8);
            //calc its image data length
            var dataLength = stride * imageBms.PixelHeight;
            //save orignal bms data needed to clone a BitmapSource using new passed data
            var pixelWidth = imageBms.PixelWidth;
            var pixelHeight = imageBms.PixelHeight;
            var dpiX = imageBms.DpiX;
            var dpiY = imageBms.DpiY;
            var format = imageBms.Format;

            if (newRgbData.Length != dataLength) return;
            ImgImage.Source = BitmapSource.Create(pixelWidth, pixelHeight,
                                                  dpiX, dpiY, format, null,
                                                  newRgbData, stride);
        }

        //UpdateImage()

        public void CloseImage()
        {
            ImgWindowAvalable = false;
            ImgLoaded = false;
        }

        //Produce a new full image BMS with new rgb data
        private BitmapSource CloneOrigBms(byte[] newRgbData)
        {
            var stride = origBitmapSource.PixelWidth * ((origBitmapSource.Format.BitsPerPixel + 7) / 8);
            var clonedBms = BitmapSource.Create(origBitmapSource.PixelWidth, origBitmapSource.PixelHeight,
                                                         origBitmapSource.DpiX, origBitmapSource.DpiY,
                                                         origBitmapSource.Format, null,
                                                         newRgbData,
                                                         stride);
            return clonedBms;
        }

        //CloneOrigBms()
    }

    //Class ImageWindow
}