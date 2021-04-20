using FunctionOptimizer.Chromosome;
using FunctionOptimizer.Function;
using FunctionOptimizer.Model;
using FunctionOptimizer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Selection.Methods
{
    class TournamentSelection : ISelection
    {
        public int TournamentAmount { get; set; }
        private readonly IFunction Function;

        public TournamentSelection(int tournamentAmount)
        {
            if (tournamentAmount <= 1) throw new ArgumentException("Incorrect tournaments amount given");
            TournamentAmount = tournamentAmount;
            Function = new HolderTableFunction();
        }

        public List<BinaryChromosome> ApplySelection(List<BinaryChromosome> chromosomes, Range range, double numberOfBitsInChromosom)
        {
            var functionValuesByChromosome = GetFunctionValueByChromosome(chromosomes, range, numberOfBitsInChromosom);
            var tournaments = GetTournaments(functionValuesByChromosome);
            return GetBestFromTournaments(tournaments);
        }

        private Dictionary<List<BinaryChromosome>, double> GetFunctionValueByChromosome(List<BinaryChromosome> chromosomes, Range range, double bitsInChromosome)
        {
            var valuesByChromosomes = new Dictionary<List<BinaryChromosome>, double>(chromosomes.Count);
            for (int i = 0; i < chromosomes.Count; i += 2)
            {
                valuesByChromosomes.Add(new List<BinaryChromosome> { chromosomes[i], chromosomes[i + 1] },
                    Function.Compute(
                        BinaryUtils.BinaryToDecimalRepresentation(chromosomes[i].BinaryRepresentation, range, bitsInChromosome),
                        BinaryUtils.BinaryToDecimalRepresentation(chromosomes[i + 1].BinaryRepresentation, range, bitsInChromosome)));
            }
            return valuesByChromosomes;
        }

        private List<Dictionary<List<BinaryChromosome>, double>> GetTournaments(Dictionary<List<BinaryChromosome>, double> values)
        {
            var tournaments = new List<Dictionary<List<BinaryChromosome>, double>>(TournamentAmount);
            int length = values.Count;
            int singleTournamentSize = length / TournamentAmount;
            int lastTournamentSize = length - singleTournamentSize * TournamentAmount;
            if (lastTournamentSize == 0) lastTournamentSize = singleTournamentSize;
            List<List<BinaryChromosome>> keys = Enumerable.ToList(values.Keys);
            Random rand = new Random();
            for (int i = 0; i < TournamentAmount-1; i++)
            {
                Dictionary<List<BinaryChromosome>, double> tmp = new Dictionary<List<BinaryChromosome>, double>();
                for (int j = 0; j < singleTournamentSize; j++)
                {
                    List<BinaryChromosome> selectedKey = keys[rand.Next(length)];
                    tmp.Add(selectedKey, values.GetValueOrDefault(selectedKey));
                    keys.Remove(selectedKey);
                    length--;
                }
                tournaments.Add(tmp);
            }
            Dictionary<List<BinaryChromosome>, double> tmp2 = new Dictionary<List<BinaryChromosome>, double>();
            for (int j = 0; j < lastTournamentSize; j++)
            {
                List<BinaryChromosome> selectedKey = keys[rand.Next(length)];
                tmp2.Add(selectedKey, values.GetValueOrDefault(selectedKey));
                keys.Remove(selectedKey);
                length--;
            }
            tournaments.Add(tmp2);
            return tournaments;
        } 

        private List<BinaryChromosome> GetBestFromTournaments(List<Dictionary<List<BinaryChromosome>, double>> tournaments)
        {
            var bestDictionary = new Dictionary<List<BinaryChromosome>, double>();
            var bestChromosomes = new List<BinaryChromosome>();
            foreach(Dictionary<List<BinaryChromosome>, double> tournament in tournaments) 
            {
                List<BinaryChromosome> key = tournament.FirstOrDefault(x => x.Value == tournament.Values.Min()).Key;
                bestDictionary.Add(key, tournament.GetValueOrDefault(key));
            }
            foreach (var data in bestDictionary.OrderBy(key => key.Value))
            {
                bestChromosomes.AddRange(data.Key);
            }
            return bestChromosomes;
        }
    }
}
