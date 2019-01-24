using FutureNow.Models.Handwritten;
using FutureNow.Models.Image;
using FutureNow.Models.Prediction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace FutureNow.Utilities
{
    public class Utilities
    {
        public static async Task<List<CroppedImage>> CropImage(Bitmap original, PredictionResultViewModel predictionResultModel, double probability)
        {
            var croppedImages = new List<CroppedImage>();

            // An empty bitmap which will hold the cropped image
            foreach (var image in predictionResultModel.Predictions)
            {
                if (image.Probability > probability)
                {

                    Bitmap bmp = new Bitmap((int)(predictionResultModel.OriginalWidth * image.BoundingBox.Width), (int)(predictionResultModel.OriginalHeight * image.BoundingBox.Height));
                    Rectangle section = new Rectangle((int)(predictionResultModel.OriginalWidth * image.BoundingBox.Left),
                                                        (int)(predictionResultModel.OriginalHeight * image.BoundingBox.Top),
                                                        (int)(predictionResultModel.OriginalWidth * image.BoundingBox.Width),
                                                        (int)(predictionResultModel.OriginalHeight * image.BoundingBox.Height));

                    Graphics g = Graphics.FromImage(bmp);

                    // Draw the given area (section) of the source image
                    // at location 0,0 on the empty bitmap (bmp)
                    g.DrawImage(original, 0, 0, section, GraphicsUnit.Pixel);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {

                        bmp.Save(memoryStream, ImageFormat.Jpeg);

                        var handwrittenResult = await MakeRequest(memoryStream);

                        if (handwrittenResult.RecognitionResult == null || handwrittenResult.RecognitionResult.Lines.Count == 0 ||
                            handwrittenResult.RecognitionResult.Lines.Count(l => l.Words.Count == 0) > 0)
                        {
                            continue;
                        }

                        var id = Guid.NewGuid();

                        croppedImages.Add(new CroppedImage
                        {
                            Bitmap = bmp,
                            Guid = id,
                            HandwrittenResult = handwrittenResult,

                        });

                        string fileName = "D:\\future-now-images\\" + original.Tag + "\\" + id + ".jpg";

                        bmp.Save(fileName, ImageFormat.Jpeg);
                    }


                    //bmp.Dispose();
                }

            }
            return croppedImages;
        }

        static async Task<HandwrittenResultViewModel> MakeRequest(MemoryStream memoryStream)
        {
            var handwrittenResult = new HandwrittenResultViewModel();
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "e13ad935d73f43edb3581a0f0340c5dc");

            // Request parameters
            queryString["mode"] = "Handwritten";
            var uri = "https://westus.api.cognitive.microsoft.com/vision/v2.0/recognizeText?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = memoryStream.ToArray();

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    var textOperation = response.Headers.GetValues("Operation-Location").FirstOrDefault();

                    var result = await client.GetAsync(textOperation);
                    handwrittenResult = JsonConvert.DeserializeObject<HandwrittenResultViewModel>(result.Content.ReadAsStringAsync().Result);
                }
            }

            return handwrittenResult;
        }

        //Image to byte[]   
        public static byte[] BitmapToBytes(Bitmap Bitmap)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();
                Bitmap.Save(ms, Bitmap.RawFormat);
                byte[] byteImage = new Byte[ms.Length];
                byteImage = ms.ToArray();
                return byteImage;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
            }
        }
    }
}
