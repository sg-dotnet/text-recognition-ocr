using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Models.OpticalCharacterRecognition
{
    public class Region
    {
        public string BoundingBox { get; set; }

        public List<Line> Lines { get; set; }
    }
}
