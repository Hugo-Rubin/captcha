using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;
using Core.Common.Extensions;
using Core.Logic.Types;
using RestSharp;

namespace Core.Logic.Filtros
{
    public class GringoFilter
    {
        private const string ServiceBaseUrl =
            "http://ec2-54-191-202-244.us-west-2.compute.amazonaws.com:8080/cctrace-rest/ccTrace/";

        public ImgArray Apply(Image image, GringoFilterType filterType)
        {
            var client = new RestClient(ServiceBaseUrl);

            var request = new RestRequest("apply", Method.POST)
            {
                AlwaysMultipartFormData = true
            };

            request.AddParameter("algorithm", filterType.ToString());
            request.AddFile("image", image.ToByteArray(ImageFormat.Png), "mlresearch.png");

            var response = client.Execute<object>(request);

            if (response.StatusCode != HttpStatusCode.OK
                || response.Content == null)
            {
                throw new Exception("Erro ao chamar filtro do gringo");
            }

            var bitmapImage = new BitmapImage();
            using (var memoryStream = new MemoryStream(response.RawBytes))
            {
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CreateOptions = BitmapCreateOptions.None;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            using (var stream = new MemoryStream())
            {
                var encode = new BmpBitmapEncoder();
                encode.Frames.Add(BitmapFrame.Create(bitmapImage));
                encode.Save(stream);
                return new ImgArray(new Bitmap(stream));

                //var imag = Image.FromStream(stream);
                //// Derive BitMap object using Image instance, so that you can avoid the issue
                ////"a graphics object cannot be created from an image that has an indexed pixel format"
                //return new ImgArray(new Bitmap(imag));
            }
        }
    }
}
