using System.Collections.Generic;
using FunctionOptimizer.Chromosome;

namespace FunctionOptimizer.Operations.Mutation
{
    interface IMutation
    {
        List<BinaryChromosome> Mutate(List<BinaryChromosome >binaryChromosome, int probability);
    }
}
