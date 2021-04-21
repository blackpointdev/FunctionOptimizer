using FunctionOptimizer.Chromosome;
using System.Collections.Generic;

namespace FunctionOptimizer.Model
{
    class RouletteItem
    {
        public List<BinaryChromosome> BinaryChromosomes { get; set; }
        public double FunctionResult { get; set; }
        public double Distributor { get; set; }
        public double PValue { get; set; }
    }
}
