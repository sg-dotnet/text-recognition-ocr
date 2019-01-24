using FutureNow.Models.Handwritten;
using FutureNow.Models.Image;
using FutureNow.Models.Prediction;
using Microsoft.AspNetCore.Http;
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

namespace FutureNow.Services
{
    public class TagAndAnalyzeService : ITagAndAnalyzeService
    {
        private readonly IKeyVaultService _keyVaultService;
        private readonly IUploadToAzureStorageService _uploadToAzureStorageService;
        private readonly IHandwrittenRecognitionService _handwrittenRecognitionService;

        public TagAndAnalyzeService(IKeyVaultService keyVaultService,
            IUploadToAzureStorageService uploadToAzureStorageService, 
            IHandwrittenRecognitionService handwrittenRecognitionService)
        {
            _keyVaultService = keyVaultService;
            _uploadToAzureStorageService = uploadToAzureStorageService;
            _handwrittenRecognitionService = handwrittenRecognitionService;
        }

        public async Task<List<CroppedImage>> AnalyzeImageAsync(IFormFile imageFile, double probability)
        {
            var stream = imageFile.OpenReadStream();
            int height = 0;
            int width = 0;

            string url = $"https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/{ await _keyVaultService.GetSecretAsync("CognitiveServiceKey1") }/image?iterationId={ await _keyVaultService.GetSecretAsync("IterationId") }";

            byte[] data;
            var result = new PredictionResultViewModel();
            var croppedImages = new List<CroppedImage>();

            using (var client = new HttpClient())
            using (var br = new BinaryReader(stream))
            {

                data = br.ReadBytes((int)stream.Length);
                var image = Image.FromStream(stream);
                var bitmap = new Bitmap(image);

                height = bitmap.Height;
                width = bitmap.Width;

                if (data.Length > 0)
                {
                    using (var content = new ByteArrayContent(data))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        client.DefaultRequestHeaders.Add("Prediction-Key", await _keyVaultService.GetSecretAsync("PredictionKey"));

                        using (var response = await client.PostAsync(url, content))
                        {
                            result = await response.Content.ReadAsAsync<PredictionResultViewModel>();

                            result.OriginalHeight = height;
                            result.OriginalWidth = width;

                            return await CropImageAsync(bitmap, result, probability / 100d);
                        }
                        
                    }
                }
            }

            return new List<CroppedImage>();
        }

        async Task<List<CroppedImage>> CropImageAsync(Bitmap original, PredictionResultViewModel predictionResultModel, double probability)
        {
            var croppedImages = new List<CroppedImage>();

            // An empty bitmap which will hold the cropped image
            foreach (var image in predictionResultModel.Predictions)
            {
                if (image.Probability > probability)
                {
                    using (var bmp = new Bitmap((int)(predictionResultModel.OriginalWidth * image.BoundingBox.Width), (int)(predictionResultModel.OriginalHeight * image.BoundingBox.Height)))
                    {
                        var section = new Rectangle((int)(predictionResultModel.OriginalWidth * image.BoundingBox.Left),
                                                        (int)(predictionResultModel.OriginalHeight * image.BoundingBox.Top),
                                                        (int)(predictionResultModel.OriginalWidth * image.BoundingBox.Width),
                                                        (int)(predictionResultModel.OriginalHeight * image.BoundingBox.Height));

                        var g = Graphics.FromImage(bmp);

                        // Draw the given area (section) of the source image
                        // at location 0,0 on the empty bitmap (bmp)
                        g.DrawImage(original, 0, 0, section, GraphicsUnit.Pixel);

                        using (var memoryStream = new MemoryStream())
                        {
                            bmp.Save(memoryStream, ImageFormat.Jpeg);

                            var handwrittenResult = await _handwrittenRecognitionService.AnalyzeImageAsync(memoryStream.ToArray());

                            if (handwrittenResult == null || 
                                handwrittenResult.RecognitionResult == null || 
                                handwrittenResult.RecognitionResult.Lines == null || 
                                handwrittenResult.RecognitionResult.Lines.Count == 0 ||
                                handwrittenResult.RecognitionResult.Lines.Count(l => l.Words.Count == 0) > 0)
                            {
                                continue;
                            }

                            var id = Guid.NewGuid();

                            string fileName = $"{ original.Tag }_{ id }.jpg";

                            string imageUrl = await _uploadToAzureStorageService.UploadFileAsync(memoryStream.ToArray(), fileName,
                                await _keyVaultService.GetSecretAsync("AzureStorageAccountAccessKey"),
                                "croppedimages");

                            croppedImages.Add(new CroppedImage
                            {
                                ImageUrl = imageUrl,
                                HandwrittenResult = handwrittenResult
                            });
                        }
                    }
                    
                }

            }
            return croppedImages;
        }
    }
}
