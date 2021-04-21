using FunctionOptimizer.Chromosome;
using FunctionOptimizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Selection
{
    interface ISelection
    {
        List<BinaryChromosome> ApplySelection(List<BinaryChromosome> population, Range range, double numberOfBitsInChromosome);
    }
}
