using FunctionOptimizer.Chromosome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
