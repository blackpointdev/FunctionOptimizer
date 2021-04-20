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
            var bestOfEachEpoch = new List<FunctionResult>();
            List<BinaryChromosome> tmpBest = new List<BinaryChromosome>(2);
            for (int i = 0; i < InputDataEntity.EpochsAmount; i++)
            {
                List<BinaryChromosome> newPopulation = selectionMethod.ApplySelection(BinaryChromosomes, inputDataEntity.DataRange, NumberOfBitsInChromosome);
                if (i != 0 && GetFunctionResultForChromosomes(newPopulation[0], newPopulation[1], inputDataEntity.DataRange).FunctionValue > 
                    GetFunctionResultForChromosomes(tmpBest[0], tmpBest[1], inputDataEntity.DataRange).FunctionValue)
                {
                    newPopulation.InsertRange(0, tmpBest);
                }
                bestOfEachEpoch.Add(GetFunctionResultForChromosomes(newPopulation[0], newPopulation[1], inputDataEntity.DataRange));
                tmpBest.Add(newPopulation[0]);
                tmpBest.Add(newPopulation[1]);
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
            return new OptimizationResult
            {
                ExtremeValue = minimum,
                X1 = x1,
                X2 = x2,
                BestFromPreviousEpochs = bestOfEachEpoch
            };
        }

        private FunctionResult GetFunctionResultForChromosomes(BinaryChromosome chromosome1, BinaryChromosome chromosome2, Range range)
        {
            var x1Best = BinaryUtils.BinaryToDecimalRepresentation(chromosome1.BinaryRepresentation, range, NumberOfBitsInChromosome);
            var x2Best = BinaryUtils.BinaryToDecimalRepresentation(chromosome2.BinaryRepresentation, range, NumberOfBitsInChromosome);
            return new FunctionResult(Function.Compute(x1Best, x2Best), x1Best, x2Best);
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
                case (MutationMethod.OnePoint):
                    return new OnePointMutation();
                case (MutationMethod.TwoPoints):
                    return new TwoPointMutation();
                default:
                    throw new ArgumentException("Mutation method was not foud.");
            }
        }

    }
}
