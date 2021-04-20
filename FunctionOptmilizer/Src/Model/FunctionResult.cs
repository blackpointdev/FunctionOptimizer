using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Model
{
    class FunctionResult
    {
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double FunctionValue {get; set;}

        public FunctionResult(double functionValue, double x1, double x2)
        {
            FunctionValue = functionValue;
            X1 = x1;
            X2 = x2;
        }
    }
}
