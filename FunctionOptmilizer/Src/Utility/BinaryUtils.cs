using FunctionOptimizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Utility
{
    static class BinaryUtils
    {
        public static double BinaryToDecimalRepresentation(String binaryRepresentation, Range range, double bitsInChromosome)
        {
            return range.Start + Convert.ToInt64(binaryRepresentation, 2) * (range.End - range.Start) / (Math.Pow(2, bitsInChromosome) - 1);
        }

        public static string DecimalToBinaryRepresentation(int value, Range range, double bitsInChromosome)
        {
            Int64 decimalValue = Convert.ToInt64(Math.Round((value - range.Start) * (Math.Pow(2, bitsInChromosome) - 1) / (range.End - range.Start)));
            return Convert.ToString(decimalValue, 2);
        }
    }
}
