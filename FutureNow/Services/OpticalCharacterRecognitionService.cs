using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FutureNow.Models.OpticalCharacterRecognition;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;

namespace FutureNow.Services
{
    public class OpticalCharacterRecognitionService : IOpticalCharacterRecognitionService
    {
        private readonly IKeyVaultService _keyVaultService;

        public OpticalCharacterRecognitionService(IKeyVaultService keyVaultService)
        {
            _keyVaultService = keyVaultService;
        }

        public async Task<OpticalCharacterRecognitionResultViewModel> AnalyzeImageAsync(IFormFile imageFile)
        {            
            var stream = imageFile.OpenReadStream();
            using (var br = new BinaryReader(stream))
            {
                var byteData = br.ReadBytes((int)stream.Length);

                if (byteData.Length > 0)
                {
                    using (var client = new HttpClient())
                    using (var data = new ByteArrayContent(byteData))
                    {
                        data.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", await _keyVaultService.GetSecretAsync("CognitiveServiceKey2"));

                        using (HttpResponseMessage response = await client.PostAsync("https://southeastasia.api.cognitive.microsoft.com/vision/v1.0/ocr?language=en", data))
                        {
                            using (HttpContent content = response.Content)
                            {

                                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Accepted)
                                {
                                    string jsonResponse = await content.ReadAsStringAsync();

                                    return JsonConvert.DeserializeObject<OpticalCharacterRecognitionResultViewModel>(jsonResponse);
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
