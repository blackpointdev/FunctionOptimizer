using FunctionOptimizer.Model;
using FunctionOptimizer.Function;
using FunctionOptimizer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Chromosome
{
    class PopulationService
    {
        public double NumberOfBits { get; private set; }
        private readonly IFunction Function;

        public PopulationService()
        {
            Function = new BoothFunction();
        }

        public List<BinaryChromosome> GeneratePopulation(int amount, double numberOfBits)
        {
            var chromosomes = new List<BinaryChromosome>(Convert.ToInt32(amount));
            for (int i = 0; i < amount * 2; i++) // Function has two parameters
            {
                chromosomes.Add(GenerateChromosome(numberOfBits));
            }
            return chromosomes;
        }

        public double CalculateNumberOfBitsInChromosome(Range range, uint accuracy)
        {
            return Math.Ceiling(Math.Log((range.End - range.Start) * Math.Pow(10, accuracy), 2));
        }

        private BinaryChromosome GenerateChromosome(double numberOfBits)
        {
            StringBuilder generatedValue = new StringBuilder();
            var random = new Random();
            for (uint i = 0; i < numberOfBits; i++)
            {
                generatedValue.Append(random.Next(0, 2).ToString());
            }
            return new BinaryChromosome(generatedValue.ToString());
        }
    }
}
