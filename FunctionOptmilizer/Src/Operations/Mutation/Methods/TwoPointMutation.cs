using FunctionOptimizer.Chromosome;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionOptimizer.Operations.Mutation.Methods
{
    class TwoPointMutation : IMutation
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
            Random rand = new Random();
            int index1 = rand.Next(0, newBinaryRepresentation.Length);
            int index2 = index1;
            while (index1 == index2) index2 = rand.Next(0, newBinaryRepresentation.Length);
            newBinaryRepresentation[index1] = newBinaryRepresentation[index1] == '0' ? '1' : '0';
            newBinaryRepresentation[index2] = newBinaryRepresentation[index2] == '0' ? '1' : '0';
            return new BinaryChromosome(newBinaryRepresentation.ToString());
        }
    }
}
