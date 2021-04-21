using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Model
{
    class OptimizationResult
    {
        public List<FunctionResult> BestFromPreviousEpochs { get; set; }
        public List<double> MeanFromPreviousEpochs { get; set; }
        public List<double> StandardDeviation { get; set; }
        public double ExtremeValue { get; set; }
        public double X1 { get; set; }
        public double X2 { get; set; }
        public long TimeElapsed { get; set; }

        public OptimizationResult(double extremeValue, double x1, double x2)
        {
            ExtremeValue = extremeValue;
            X1 = x1;
            X2 = x2;
        }

        public OptimizationResult()
        {
        }
    }
}
