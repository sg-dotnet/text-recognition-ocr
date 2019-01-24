using FutureNow.Models.Handwritten;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Models.Image
{
    public class CroppedImage
    {
        public string ImageUrl { get; set; }

        public HandwrittenResultViewModel HandwrittenResult { get; set; }
    }
}
