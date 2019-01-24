using System;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FutureNow.Models;
using FutureNow.Data.ViewModels;
using System.Net.Http;
using FutureNow.Models.Prediction;
using FutureNow.Models.Image;
using System.IO;
using System.Net.Http.Headers;
using FutureNow.Services;
using Microsoft.Azure.Services.AppAuthentication;

namespace FutureNow.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITagAndAnalyzeService _tagAndAnalyzeService;
        private readonly IOpticalCharacterRecognitionService _opticalCharacterRecognitionService;
        private readonly IHandwrittenRecognitionService _handwrittenRecognitionService;

        public HomeController(ITagAndAnalyzeService tagAndAnalyzeService, IOpticalCharacterRecognitionService opticalCharacterRecognitionService,
            IHandwrittenRecognitionService handwrittenRecognitionService)
        {
            _tagAndAnalyzeService = tagAndAnalyzeService;
            _opticalCharacterRecognitionService = opticalCharacterRecognitionService;
            _handwrittenRecognitionService = handwrittenRecognitionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DocumentUploadViewModel model)
        {
            ViewBag.OcrResult = await _opticalCharacterRecognitionService.AnalyzeImageAsync(model.File);
            ViewBag.HandwrittenRecognitionResult = await _handwrittenRecognitionService.AnalyzeImageAsync(model.File);
            ViewBag.ResultImages = await _tagAndAnalyzeService.AnalyzeImageAsync(model.File, model.PredictionPercentage);

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // This method fetches a token from Azure Active Directory which can then be provided to Azure Key Vault to authenticate
        public async Task<string> GetAccessTokenAsync()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://vault.azure.net");
            return accessToken;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
