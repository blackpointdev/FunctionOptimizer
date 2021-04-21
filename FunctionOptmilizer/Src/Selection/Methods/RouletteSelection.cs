using FunctionOptimizer.Chromosome;
using FunctionOptimizer.Function;
using FunctionOptimizer.Model;
using FunctionOptimizer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Selection.Methods
{
    class RouletteSelection : ISelection
    {
        private readonly int bestPercentage;
        private readonly bool maximize;
        private readonly IFunction function;

        public RouletteSelection(int bestPercentage, bool maximize)
        {
            this.bestPercentage = bestPercentage;
            this.function = new BoothFunction();
            this.maximize = maximize;

        }
        public List<BinaryChromosome> ApplySelection(List<BinaryChromosome> population, Range range, double numberOfBitsInChromosome)
        {
            int numberOfBestIndividuals = Convert.ToInt32(Math.Ceiling(population.Count * this.bestPercentage / 100.0));
            if (numberOfBestIndividuals < 2) numberOfBestIndividuals = 2;
            if (numberOfBestIndividuals % 2 != 0) numberOfBestIndividuals--;
            var rouletteItems = GetRouletteItems(population, range, numberOfBitsInChromosome);
            double valuesSum = 0;
            foreach (var item in rouletteItems) valuesSum += this.maximize ? item.FunctionResult : Math.Abs(1 / item.FunctionResult);
            double distributor = 0;
            for (int i = 0; i < rouletteItems.Count; i++)
            {
                rouletteItems[i].PValue = this.maximize ? Math.Abs(rouletteItems[i].FunctionResult / valuesSum) : Math.Abs(1 / rouletteItems[i].FunctionResult / valuesSum);
                distributor += rouletteItems[i].PValue;
                rouletteItems[i].Distributor = distributor;
            }
            var bestItems = new List<RouletteItem>();
            Random rand = new Random();
            for (int i = 0; i < numberOfBestIndividuals/2; i++)
            {
                double randomNumber = rand.NextDouble();
                var selectedItem = rouletteItems.Aggregate((x, y) => Math.Abs(x.Distributor - randomNumber) < Math.Abs(y.Distributor - randomNumber) ? x : y);
                bestItems.Add(selectedItem);
            }
            var bestChromosomes = new List<BinaryChromosome>();
            foreach(var item in (this.maximize ? bestItems.OrderByDescending(x => x.FunctionResult) : bestItems.OrderBy(x => x.FunctionResult)))
            {
                bestChromosomes.AddRange(item.BinaryChromosomes);
            }
            return bestChromosomes;
        }

        private List<RouletteItem> GetRouletteItems(List<BinaryChromosome> chromosomes, Range range, double bitsInChromosome)
        {
            var resultsList = new List<RouletteItem>();
            for (int i = 0; i < chromosomes.Count; i += 2)
            {
                var item = new RouletteItem();
                item.BinaryChromosomes = new List<BinaryChromosome>() { chromosomes[i], chromosomes[i + 1] };
                item.FunctionResult = CalculateFunctionValue(item.BinaryChromosomes, range, bitsInChromosome);
                resultsList.Add(item);
            }
            return resultsList;
        }

        private double CalculateFunctionValue(List<BinaryChromosome> chromosomes, Range range, double bitsInChromosome)
        {
            if (chromosomes.Count != 2) throw new ArgumentException("Incorrect chromosomes amount.");
            return function.Compute(
                        BinaryUtils.BinaryToDecimalRepresentation(chromosomes[0].BinaryRepresentation, range, bitsInChromosome),
                        BinaryUtils.BinaryToDecimalRepresentation(chromosomes[1].BinaryRepresentation, range, bitsInChromosome));
        }
    }
}
