using FutureNow.Models.Image;
using FutureNow.Models.Prediction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Services
{
    public interface ITagAndAnalyzeService
    {
        Task<List<CroppedImage>> AnalyzeImageAsync(IFormFile imageFile, double probability);
    }
}
