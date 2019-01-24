using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Models.Handwritten
{
    public class HandwrittenResultViewModel
    {
        public string Status { get; set; }

        public RecognitionResult RecognitionResult { get; set; }
    }
}
