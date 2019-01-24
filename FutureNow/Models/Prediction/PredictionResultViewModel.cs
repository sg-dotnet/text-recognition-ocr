using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Models.Prediction
{
    public class PredictionResultViewModel
    {
        public string Id { get; set; }

        public string Project { get; set; }

        public string Iteration { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<Prediction> Predictions { get; set; }

        public int OriginalWidth { get; set; }

        public int OriginalHeight { get; set; }
    }
}
