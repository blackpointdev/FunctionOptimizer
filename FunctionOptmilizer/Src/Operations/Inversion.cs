using FunctionOptimizer.Chromosome;
using FunctionOptimizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Operations
{
    class Inversion
    {
        public List<BinaryChromosome> PerformInversion(List<BinaryChromosome> binaryChromosomes, int probability)
        {
            var newChromosomes = new List<BinaryChromosome>(binaryChromosomes.Count);
            foreach(BinaryChromosome chromosome in binaryChromosomes)
            {
                if (new Random().Next(0, 100) >= probability)
                {
                    newChromosomes.Add(chromosome);
                    continue;
                }
                newChromosomes.Add(PerformInversion(chromosome));
            }
            return newChromosomes;
        }

        private BinaryChromosome PerformInversion(BinaryChromosome binaryChromosome)
        {
            var binaryRepresentation = binaryChromosome.BinaryRepresentation;
            var length = binaryRepresentation.Length;
            Random random = new Random();
            var startRange = random.Next(0, length - 1);
            var endRange = random.Next(startRange + 1, length + 1);
            var subBinaryRepresentation = new StringBuilder(binaryRepresentation.Substring(startRange, endRange - startRange));
            for (int i = 0; i < subBinaryRepresentation.Length; i++) 
                subBinaryRepresentation[i] = subBinaryRepresentation[i] == '0' ? subBinaryRepresentation[i] = '1' : subBinaryRepresentation[i] = '0';
            
            var newChromosome = new BinaryChromosome(new StringBuilder()
                .Append(binaryRepresentation.Substring(0, startRange))
                .Append(subBinaryRepresentation.ToString())
                .Append(binaryRepresentation.Substring(endRange))
                .ToString());
            return newChromosome;
        }
    }
}
