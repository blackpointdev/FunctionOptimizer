using FunctionOptimizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Chromosome
{
    class BinaryChromosome
    { 
        public string BinaryRepresentation { get; set; }
        
        public BinaryChromosome(String binaryRepresentation)
        {
            BinaryRepresentation = binaryRepresentation;
        }

        public BinaryChromosome(BinaryChromosome other)
        {
            this.BinaryRepresentation = other.BinaryRepresentation;
        }
    }
}
