using FutureNow.Models.Handwritten;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Services
{
    public interface IHandwrittenRecognitionService
    {
        Task<HandwrittenResultViewModel> AnalyzeImageAsync(IFormFile imageFile);

        Task<HandwrittenResultViewModel> AnalyzeImageAsync(byte[] byteData);
    }
}
