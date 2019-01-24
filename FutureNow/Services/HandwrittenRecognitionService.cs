using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FutureNow.Models.Handwritten;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FutureNow.Services
{
    public class HandwrittenRecognitionService : IHandwrittenRecognitionService
    {
        private readonly IKeyVaultService _keyVaultService;

        public HandwrittenRecognitionService(IKeyVaultService keyVaultService)
        {
            _keyVaultService = keyVaultService;
        }

        public async Task<HandwrittenResultViewModel> AnalyzeImageAsync(IFormFile imageFile)
        {
            var stream = imageFile.OpenReadStream();
            using (var br = new BinaryReader(stream))
            {
                var byteData = br.ReadBytes((int)stream.Length);

                return await AnalyzeImageAsync(byteData);
            }
        }

        public async Task<HandwrittenResultViewModel> AnalyzeImageAsync(byte[] byteData)
        {
            if (byteData.Length > 0)
            {
                using (var client = new HttpClient())
                using (var data = new ByteArrayContent(byteData))
                {
                    data.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", await _keyVaultService.GetSecretAsync("CognitiveServiceKey2"));

                    using (var response = await client.PostAsync("https://southeastasia.api.cognitive.microsoft.com/vision/v1.0/recognizeText?handwriting=true", data))
                    {
                        using (var content = response.Content)
                        {

                            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                            {
                                bool isAnalizying = true;
                                do
                                {
                                    var textOperation = response.Headers.GetValues("Operation-Location").FirstOrDefault();

                                    var result = await client.GetAsync(textOperation);

                                    string jsonResponse = result.Content.ReadAsStringAsync().Result;

                                    var handwrittenAnalyzeResult = JsonConvert.DeserializeObject<HandwrittenResultViewModel>(jsonResponse);

                                    isAnalizying = handwrittenAnalyzeResult.Status != "Succeeded";

                                    if (!isAnalizying)
                                    {
                                        return handwrittenAnalyzeResult;
                                    }

                                } while (isAnalizying);
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
