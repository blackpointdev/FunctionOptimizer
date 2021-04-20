using FunctionOptimizer.Chromosome;
using FunctionOptimizer.Model;
using FunctionOptimizer.Function;
using FunctionOptimizer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Selection.Methods
{
    class BestSelection : ISelection
    {
        public List<BinaryChromosome> BinaryChromosomes { get; private set; }
        public int BestPercentage { get; private set; }
        private readonly IFunction Function;

        public BestSelection(int bestPercentage)
        {
            Function = new HolderTableFunction();
            BestPercentage = bestPercentage;
        }

        public List<BinaryChromosome> ApplySelection(List<BinaryChromosome> population, Range range, double bitsInChromosome)
        {
            int numberOfBestIndividuals = Convert.ToInt32(Math.Ceiling(population.Count * BestPercentage / 100.0));
            if (numberOfBestIndividuals < 2) numberOfBestIndividuals = 2;
            if (numberOfBestIndividuals % 2 != 0) numberOfBestIndividuals--;
            var bestChromosomes = new List<BinaryChromosome>();
            var functionValuesByChromosomes = GetFunctionValueByChromosome(population, range, bitsInChromosome);
            foreach(var data in functionValuesByChromosomes.OrderBy(key => key.Value))
            {
                bestChromosomes.AddRange(data.Key);
                if (bestChromosomes.Count >= numberOfBestIndividuals) break;
            }
            return bestChromosomes;
        }

        private Dictionary<List<BinaryChromosome>, double> GetFunctionValueByChromosome(List<BinaryChromosome> chromosomes, Range range, double bitsInChromosome)
        {
            var valuesByChromosomes = new Dictionary<List<BinaryChromosome>, double>(chromosomes.Count);
            for (int i = 0; i < chromosomes.Count; i += 2)
            {
                valuesByChromosomes.Add(new List<BinaryChromosome> { chromosomes[i], chromosomes[i + 1] },
                    Function.Compute(
                        BinaryUtils.BinaryToDecimalRepresentation(chromosomes[i].BinaryRepresentation, range, bitsInChromosome),
                        BinaryUtils.BinaryToDecimalRepresentation(chromosomes[i + 1].BinaryRepresentation, range, bitsInChromosome)));
            }
            return valuesByChromosomes;
        }
    }
}
