using FunctionOptimizer.Chromosome;
using FunctionOptimizer.Model;
using System.Collections.Generic;

namespace FunctionOptimizer.Selection
{
    interface ISelection
    {
        List<BinaryChromosome> ApplySelection(List<BinaryChromosome> population, Range range, double numberOfBitsInChromosome);
    }
}
