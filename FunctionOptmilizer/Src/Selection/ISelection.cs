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
        List<BinaryChromosome> ApplySelection(List<BinaryChromosome> chromosomes, Range range, double numberOfBitsInChromosom);
    }
}
