using FunctionOptimizer.Selection;
using FunctionOptimizer.Operations.Crossing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunctionOptimizer.Operations.Mutation;

namespace FunctionOptimizer.Model
{
    class InputDataEntity
    {
        public Range DataRange { get; set; }
        public uint Accuracy { get; set; } // Number of significant figures
        public int PopulationAmount { get; set; }
        public int EpochsAmount { get; set; }
        public int BestSelectionPercentage { get; set; }
        public int TournamentAmount { get; set; }
        public int PercentageToKeep { get; set; }
        public int CrossProbability { get; set; }
        public int MutationProbability { get; set; }
        public int InversionProbability { get; set; }
        public int EliteStrategyAmount { get; set; }
        public SelectionMethod SelectionMethod { get; set; }
        public CrossMethod CrossMethod { get; set; }
        public MutationMethod MutationMethod { get; set; }

        public override string ToString()
        {
            return String.Format(@"DataRange: {0}, 
                                PupulationAmount: {1}, 
                                Accuracy: {2}
                                EpochsAmount: {3}, 
                                BestSelectionPercentage: {4}, 
                                TournamentAmount: {5}, 
                                PercentageToKeep: {6}, 
                                CrossProbability: {7}, 
                                MutationProbability: {8}, 
                                InversionProbability: {9},
                                EliteStrategyAmount: {10}
                                SelectionMethod: {11},
                                CrossMethod: {12},
                                MutationMethod: {13}",
                                DataRange, PopulationAmount, Accuracy, EpochsAmount, BestSelectionPercentage, TournamentAmount, PercentageToKeep,
                                CrossProbability, MutationProbability, InversionProbability, EliteStrategyAmount, SelectionMethod, CrossMethod, MutationMethod);
        }
    }
}
