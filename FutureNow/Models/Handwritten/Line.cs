using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Models.Handwritten
{
    public class Line
    {
        public List<int> BoundingBox { get; set; }
        public string Text { get; set; }
        public List<Word> Words { get; set; }
    }
}
