using FunctionOptimizer.Chromosome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Operations.Crossing
{
    interface ICross
    {
        List<BinaryChromosome> Cross(List<BinaryChromosome> binaryChromosomes, int probability);
    }
}
