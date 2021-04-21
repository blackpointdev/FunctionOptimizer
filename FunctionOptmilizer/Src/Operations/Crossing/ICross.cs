using FunctionOptimizer.Chromosome;
using System.Collections.Generic;

namespace FunctionOptimizer.Operations.Crossing
{
    interface ICross
    {
        List<BinaryChromosome> Cross(List<BinaryChromosome> binaryChromosomes, int probability);
    }
}
