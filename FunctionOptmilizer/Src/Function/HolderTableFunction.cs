using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace FunctionOptimizer.Function
{
    class HolderTableFunction : IFunction
    {
        public double Compute(double x1, double x2)
        {
            double expcomponent = Exp(Abs(1 - (Sqrt((x1 * x1) + (x2 * x2)) / PI)));
            return -Abs(Sin(x1) * Cos(x2) * expcomponent);
        }
    }
}
