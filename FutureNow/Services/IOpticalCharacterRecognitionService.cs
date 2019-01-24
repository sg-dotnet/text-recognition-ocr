using FutureNow.Models.OpticalCharacterRecognition;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Services
{
    public interface IOpticalCharacterRecognitionService
    {
        Task<OpticalCharacterRecognitionResultViewModel> AnalyzeImageAsync(IFormFile imageFile);
    }
}
