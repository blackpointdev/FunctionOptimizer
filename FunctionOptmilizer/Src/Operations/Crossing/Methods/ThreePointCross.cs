using FunctionOptimizer.Chromosome;
using FunctionOptimizer.Operations.Crossing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Operations.Crossing.Methods
{
    class ThreePointCross : ICross
    {
        public List<BinaryChromosome> Cross(List<BinaryChromosome> binaryChromosomes, int probability)
        {
            if (binaryChromosomes.Count <= 2)
            {
                return binaryChromosomes;
            }
            var newChromosomes = new List<BinaryChromosome>(binaryChromosomes.Count);
            for (int i = 0; i < binaryChromosomes.Count; i += 4)
            {
                if (i + 4 > binaryChromosomes.Count)
                {
                    newChromosomes.AddRange(new List<BinaryChromosome>() {
                        binaryChromosomes[i], binaryChromosomes[i + 1]});
                    break;
                }
                if (new Random().Next(0, 100) >= probability)
                {
                    newChromosomes.AddRange(new List<BinaryChromosome>() {
                        binaryChromosomes[i], binaryChromosomes[i + 1], binaryChromosomes[i + 2], binaryChromosomes[i + 3]
                    });
                    continue;
                }
                var firstCrossed = CalculateThreePointsCross(new List<BinaryChromosome>() { binaryChromosomes[i], binaryChromosomes[i + 2] });
                var secondCrossed = CalculateThreePointsCross(new List<BinaryChromosome>() { binaryChromosomes[i + 1], binaryChromosomes[i + 3] });
                newChromosomes.AddRange(new List<BinaryChromosome>() { firstCrossed[0], secondCrossed[0], firstCrossed[1], secondCrossed[1] });
            }
            return newChromosomes;
        }

        private List<BinaryChromosome> CalculateThreePointsCross(List<BinaryChromosome> binaryChromosomes)
        {
            if (binaryChromosomes.Count != 2)
            {
                throw new ArgumentException("Incorrect input chromosomes sequence.");
            }

            string firstChromosomeValue = binaryChromosomes[0].BinaryRepresentation;
            string secondChromosomeValue = binaryChromosomes[1].BinaryRepresentation;
            int length = firstChromosomeValue.Length;
            int oneFourthLength = firstChromosomeValue.Length / 4;
            StringBuilder firstChromosomeUpdatedValue = new StringBuilder();
            StringBuilder secondChromosomeUpdatedValue = new StringBuilder();

            firstChromosomeUpdatedValue.Append(firstChromosomeValue.Substring(0, oneFourthLength));
            firstChromosomeUpdatedValue.Append(secondChromosomeValue.Substring(oneFourthLength, oneFourthLength));
            firstChromosomeUpdatedValue.Append(firstChromosomeValue.Substring(2 * oneFourthLength, oneFourthLength));
            firstChromosomeUpdatedValue.Append(firstChromosomeValue.Substring(3 * oneFourthLength, length - 3 * oneFourthLength));
            secondChromosomeUpdatedValue.Append(secondChromosomeValue.Substring(0, oneFourthLength));
            secondChromosomeUpdatedValue.Append(firstChromosomeValue.Substring(oneFourthLength, oneFourthLength));
            secondChromosomeUpdatedValue.Append(secondChromosomeValue.Substring(2 * oneFourthLength, oneFourthLength));
            secondChromosomeUpdatedValue.Append(secondChromosomeValue.Substring(3 * oneFourthLength, length - 3 * oneFourthLength));

            return new List<BinaryChromosome>() {
                new BinaryChromosome(firstChromosomeUpdatedValue.ToString()), new BinaryChromosome(secondChromosomeValue.ToString())
            };
        }
    }
}
