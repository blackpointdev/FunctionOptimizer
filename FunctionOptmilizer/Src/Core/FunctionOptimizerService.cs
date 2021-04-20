using FunctionOptimizer.Model;
using FunctionOptimizer.Chromosome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunctionOptimizer.Selection;
using FunctionOptimizer.Selection.Methods;
using FunctionOptimizer.Function;
using FunctionOptimizer.Operations.Crossing;
using FunctionOptimizer.Operations.Crossing.Methods;
using FunctionOptimizer.Operations.Mutation;
using FunctionOptimizer.Operations.Mutation.Methods;
using FunctionOptimizer.Operations;
using FunctionOptimizer.Utility;

namespace FunctionOptimizer.Core
{
    class FunctionOptimizerService
    {
        public InputDataEntity InputDataEntity { get; private set; }
        public List<BinaryChromosome> BinaryChromosomes { get; private set; }
        public double NumberOfBitsInChromosome { get; private set; }
        private readonly PopulationService PopulationService;
        private readonly IFunction Function;
        private readonly Inversion Inversion;

        public FunctionOptimizerService()
        {
            PopulationService = new PopulationService();
            Function = new HolderTableFunction();
            Inversion = new Inversion();
        }

        public OptimizationResult Optimize(InputDataEntity inputDataEntity)
        {
            InputDataEntity = inputDataEntity;
            NumberOfBitsInChromosome = PopulationService.CalculateNumberOfBitsInChromosome(inputDataEntity.DataRange, inputDataEntity.Accuracy);
            BinaryChromosomes = PopulationService.GeneratePopulation(inputDataEntity.PopulationAmount, NumberOfBitsInChromosome);
            // Method selection
            ISelection selectionMethod = GetSelectionMethod(InputDataEntity.SelectionMethod, inputDataEntity);
            ICross crossMethod = GetCrossMethod(InputDataEntity.CrossMethod);
            IMutation mutationMethod = GetMutationMethod(InputDataEntity.MutationMethod);
            for (int i = 0; i < InputDataEntity.EpochsAmount; i++)
            {
                List<BinaryChromosome> newPopulation = selectionMethod.ApplySelection(BinaryChromosomes, inputDataEntity.DataRange, NumberOfBitsInChromosome);
                List<BinaryChromosome> crossedPopulation = crossMethod.Cross(newPopulation, InputDataEntity.CrossProbability);
                List<BinaryChromosome> mutatedPopulation = mutationMethod.Mutate(crossedPopulation, InputDataEntity.MutationProbability);
                List<BinaryChromosome> populationAfterInversion = Inversion.PerformInversion(mutatedPopulation, InputDataEntity.InversionProbability);
                BinaryChromosomes = populationAfterInversion;
                if (BinaryChromosomes.Count <= 2) break;
            }

            var bestIndividuals = selectionMethod.ApplySelection(BinaryChromosomes, inputDataEntity.DataRange, NumberOfBitsInChromosome).Take(2).ToArray();
            double x1 = BinaryUtils.BinaryToDecimalRepresentation(bestIndividuals[0].BinaryRepresentation, inputDataEntity.DataRange, NumberOfBitsInChromosome);
            double x2 = BinaryUtils.BinaryToDecimalRepresentation(bestIndividuals[1].BinaryRepresentation, inputDataEntity.DataRange, NumberOfBitsInChromosome);
            double minimum = Function.Compute(x1, x2);
            return new OptimizationResult(minimum, x1, x2);
        }

        private ISelection GetSelectionMethod(SelectionMethod selectionMethod, InputDataEntity inputDataEntity)
        {
            switch (selectionMethod)
            {
                case (SelectionMethod.Best):
                    return new BestSelection(inputDataEntity.BestSelectionPercentage);
                default:
                    throw new ArgumentException("Selection method was not foud.");
            }
        }

        private ICross GetCrossMethod(CrossMethod crossMethod)
        {
            switch (crossMethod)
            {
                case (CrossMethod.OnePoint):
                    return new OnePointCross();
                default:
                    throw new ArgumentException("Cross method was not foud.");
            }
        }

        private IMutation GetMutationMethod(MutationMethod mutationMethod)
        {
            switch (mutationMethod)
            {
                case (MutationMethod.Edge):
                    return new EdgeMutation();
                default:
                    throw new ArgumentException("Mutation method was not foud.");
            }
        }

    }
}
