using FunctionOptimizer.Chromosome;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionOptimizer.Operations.Mutation.Methods
{
    class EdgeMutation : IMutation
    {
        public List<BinaryChromosome> Mutate(List<BinaryChromosome> binaryChromosomes, int probability)
        {
            var newChromosomes = new List<BinaryChromosome>(binaryChromosomes.Count);
            foreach(BinaryChromosome chromosome in binaryChromosomes)
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
            newBinaryRepresentation.Remove(binaryRepresentation.Length - 1, 1);
            newBinaryRepresentation.Append(binaryRepresentation[binaryRepresentation.Length - 1] == '0' ? '1' : '0');
            return new BinaryChromosome(newBinaryRepresentation.ToString());
        }
    }
}
