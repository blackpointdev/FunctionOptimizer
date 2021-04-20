using FunctionOptimizer.Chromosome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Operations.Mutation.Methods
{
    class OnePointMutation : IMutation
    {
        public List<BinaryChromosome> Mutate(List<BinaryChromosome> binaryChromosomes, int probability)
        {
            var newChromosomes = new List<BinaryChromosome>(binaryChromosomes.Count);
            foreach (BinaryChromosome chromosome in binaryChromosomes)
            {
                if (new Random().Next(0, 100) >= probability)
                {
                    newChromosomes.Add(chromosome);
                    continue;
                }
                newChromosomes.Add(Mutate(chromosome));
            }
            return newChromosomes;
        }
        private BinaryChromosome Mutate(BinaryChromosome binaryChromosome)
        {
            var binaryRepresentation = binaryChromosome.BinaryRepresentation;
            StringBuilder newBinaryRepresentation = new StringBuilder(binaryRepresentation);
            int index = new Random().Next(0, newBinaryRepresentation.Length);
            newBinaryRepresentation[index] = newBinaryRepresentation[index] == '0' ? '1' : '0';
            return new BinaryChromosome(newBinaryRepresentation.ToString());
        }
    }
}
