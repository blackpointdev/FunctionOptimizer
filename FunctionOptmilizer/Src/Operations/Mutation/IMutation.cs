using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunctionOptimizer.Chromosome;

namespace FunctionOptimizer.Operations.Mutation
{
    interface IMutation
    {
        List<BinaryChromosome> Mutate(List<BinaryChromosome >binaryChromosome, int probability);
    }
}
