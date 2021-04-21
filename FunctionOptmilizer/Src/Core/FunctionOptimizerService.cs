using FunctionOptimizer.Model;
using FunctionOptimizer.Chromosome;
using System;
using System.Collections.Generic;
using System.Linq;
using FunctionOptimizer.Selection;
using FunctionOptimizer.Selection.Methods;
using FunctionOptimizer.Function;
using FunctionOptimizer.Operations.Crossing;
using FunctionOptimizer.Operations.Crossing.Methods;
using FunctionOptimizer.Operations.Mutation;
using FunctionOptimizer.Operations.Mutation.Methods;
using FunctionOptimizer.Operations;
using FunctionOptimizer.Utility;
using System.Diagnostics;

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
            Function = new BoothFunction();
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
            List<BinaryChromosome> tmpBest = new List<BinaryChromosome>();
            List<double> means = new List<double>();
            List<double> standardDeviation = new List<double>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < InputDataEntity.EpochsAmount; i++)
            {
                List<BinaryChromosome> newPopulation = selectionMethod.ApplySelection(BinaryChromosomes, inputDataEntity.DataRange, NumberOfBitsInChromosome);
                if (i != 0)
                {
                    double newValue = GetFunctionResultForChromosomes(newPopulation[0], newPopulation[1], inputDataEntity.DataRange).FunctionValue;
                    double oldValue = GetFunctionResultForChromosomes(tmpBest[0], tmpBest[1], inputDataEntity.DataRange).FunctionValue;
                    if ((inputDataEntity.Maximize == false && newValue > oldValue) || (inputDataEntity.Maximize == true && newValue < oldValue))
                    {
                        newPopulation.InsertRange(0, tmpBest);
                    }
                    else
                    {
                        newPopulation.InsertRange(newPopulation.Count - 1, tmpBest);
                    }
                }
                bestOfEachEpoch.Add(GetFunctionResultForChromosomes(newPopulation[0], newPopulation[1], inputDataEntity.DataRange));
                tmpBest.Clear();
                for (int j = 0; j < inputDataEntity.EliteStrategyAmount * 2; j++) tmpBest.Add(newPopulation[j]);
                means.Add(CalculateMean(newPopulation, inputDataEntity.DataRange));
                standardDeviation.Add(CalculateDeviation(newPopulation, inputDataEntity.DataRange, means[i]));
                List<BinaryChromosome> crossedPopulation = crossMethod.Cross(newPopulation, InputDataEntity.CrossProbability);
                List<BinaryChromosome> mutatedPopulation = mutationMethod.Mutate(crossedPopulation, InputDataEntity.MutationProbability);
                List<BinaryChromosome> populationAfterInversion = Inversion.PerformInversion(mutatedPopulation, InputDataEntity.InversionProbability);
                BinaryChromosomes = populationAfterInversion;
                if (BinaryChromosomes.Count <= 2) break;
            }

            var bestIndividuals = selectionMethod.ApplySelection(BinaryChromosomes, inputDataEntity.DataRange, NumberOfBitsInChromosome).Take(2).ToArray();
            double x1 = BinaryUtils.BinaryToDecimalRepresentation(bestIndividuals[0].BinaryRepresentation, inputDataEntity.DataRange, NumberOfBitsInChromosome);
            double x2 = BinaryUtils.BinaryToDecimalRepresentation(bestIndividuals[1].BinaryRepresentation, inputDataEntity.DataRange, NumberOfBitsInChromosome);
            double extremum = Function.Compute(x1, x2);
            stopwatch.Stop();
            return new OptimizationResult
            {
                ExtremeValue = extremum,
                X1 = x1,
                X2 = x2,
                BestFromPreviousEpochs = bestOfEachEpoch,
                MeanFromPreviousEpochs = means,
                StandardDeviation = standardDeviation,
                TimeElapsed = stopwatch.ElapsedMilliseconds
            };
        }

        private double CalculateMean(List<BinaryChromosome> binaryChromosomes, Range range)
        {
            double sum = 0;
            for (int i = 0; i < binaryChromosomes.Count; i += 2)
            {
                sum += GetFunctionResultForChromosomes(binaryChromosomes[i], binaryChromosomes[i + 1], range).FunctionValue;
            }
            return sum / (binaryChromosomes.Count / 2);
        }

        private double CalculateDeviation(List<BinaryChromosome> binaryChromosomes, Range range, double mean)
        {
            double sumFactor = 0;
            for (int i = 0; i < binaryChromosomes.Count; i += 2)
            {
                sumFactor += Math.Pow(GetFunctionResultForChromosomes(binaryChromosomes[i], binaryChromosomes[i + 1], range).FunctionValue - mean, 2);
            }
            return Math.Sqrt(sumFactor / binaryChromosomes.Count / 2);
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
                    return new BestSelection(inputDataEntity.BestSelectionPercentage, inputDataEntity.Maximize);
                case (SelectionMethod.Roulette):
                    return new RouletteSelection(inputDataEntity.BestSelectionPercentage, inputDataEntity.Maximize);
                case (SelectionMethod.Tournament):
                    return new TournamentSelection(inputDataEntity.TournamentAmount, inputDataEntity.Maximize);
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
                case (CrossMethod.TwoPoints):
                    return new TwoPointCross();
                case (CrossMethod.ThreePoints):
                    return new ThreePointCross();
                case (CrossMethod.Homogeneous):
                    return new HomogeneousCross();
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
