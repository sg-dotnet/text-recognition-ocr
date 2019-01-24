using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Models.OpticalCharacterRecognition
{
    public class Line
    {
        public string BoundingBox { get; set; }

        public List<Word> Words { get; set; }
    }
}
