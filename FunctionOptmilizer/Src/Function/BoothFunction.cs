using System;

namespace FunctionOptimizer.Function
{
    class BoothFunction : IFunction
    {
        public double Compute(double x1, double x2)
        {
            return Math.Pow(x1 + 2 * x2 - 7, 2) + Math.Pow(2 * x1 + x2 - 5, 2);
        }
    }
}
