using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Function
{
    interface IFunction
    {
        double Compute(double x1, double x2);
    }
}
