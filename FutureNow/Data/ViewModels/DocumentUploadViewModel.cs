using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Data.ViewModels
{
    public class DocumentUploadViewModel
    {
        [Required]
        [Display(Name = "Precision (%)")]
        public int PredictionPercentage { get; set; }

        [Required]
        [Display(Name = "Document")]
        public IFormFile File { get; set; }

        public string FileName
        {
            get
            {
                if (File != null)
                    return File.FileName;

                return "";
            }
        }
    }
}
