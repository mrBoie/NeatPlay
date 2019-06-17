using SoNEAT.CrossFunctions;
using SoNEAT.InnovationPointGenerator;
using SoNEAT.Models;
using SoNEAT.Mutations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoNEAT
{
    public abstract class Evaluator
    {
        private readonly INeatConfiguration _neatConfiguration;
        private readonly IInnovationPointGenerator _nodeInnovationGenerator;
        private readonly IInnovationPointGenerator _connectionInnovationGenerator;
        private readonly IMutation _addConnectionMutation;
        private readonly IMutation _addNodeMutation;
        private readonly IMutation _weightMutation;
        private readonly ICrossFunctionStrategy _crossFunctionStrategy;
        private readonly Random _random;
        private List<Genome> _genomes;
        private List<FitnessGenome> _evaluatedGenomes;
        private List<Genome> _nextGeneration;
        private List<FitnessGenome> _lastGenerationResults;

        public Evaluator(INeatConfiguration neatConfiguration, IGenesisGenomeProvider genomeProvider, IInnovationPointGenerator nodeInnovationGenerator, IInnovationPointGenerator connectionInnovationGenerator, IMutation addConnectionMutation, IMutation addNodeMutation, IMutation weightMutation, ICrossFunctionStrategy crossFunctionStrategy, Random random)
        {
            _neatConfiguration = neatConfiguration;
            _nodeInnovationGenerator = nodeInnovationGenerator;
            _connectionInnovationGenerator = connectionInnovationGenerator;
            _addConnectionMutation = addConnectionMutation;
            _addNodeMutation = addNodeMutation;
            _weightMutation = weightMutation;
            _crossFunctionStrategy = crossFunctionStrategy;
            _random = random;
            _genomes = Enumerable.Repeat(genomeProvider.GenerateGenesisGenome(), neatConfiguration.PopulationSize).ToList();

            _evaluatedGenomes = new List<FitnessGenome>();
            _nextGeneration = new List<Genome>();

            _lastGenerationResults = new List<FitnessGenome>();
        }

        public FitnessGenome FittestGenome { get; private set; }

        public void EvaluateGeneration()
        {
            _lastGenerationResults.Clear();
            _evaluatedGenomes.Clear();

            for (int i = 0; i < _genomes.Count; i++)
            {
                var genome = _genomes[i];
                var fitnessGenome = new FitnessGenome() { Fitness = EvaluateGenome(genome), Genome = genome };
                _evaluatedGenomes.Add(fitnessGenome);
            }

            //Sort Collections
            _evaluatedGenomes = _evaluatedGenomes.OrderByDescending(g => g.Fitness).ToList();

            _lastGenerationResults.AddRange(_evaluatedGenomes);

            //kill the 9/10 worst genomes
            var cutoffIndex = _evaluatedGenomes.Count / 10;
            _evaluatedGenomes = _evaluatedGenomes.Take(cutoffIndex).ToList();

            //find next generation population
            _nextGeneration.Clear();

            // save champ
            var champion = _evaluatedGenomes.First();
            FittestGenome = champion;
            _nextGeneration.Add(champion.Genome);

            while(_nextGeneration.Count() < _neatConfiguration.PopulationSize)
            {
                if(_random.NextDouble() > _neatConfiguration.ASexualReproductionRate)
                {
                    var parent1 = _evaluatedGenomes[_random.Next(_evaluatedGenomes.Count)];
                    FitnessGenome parent2;
                    do
                    {
                        parent2 = _evaluatedGenomes[_random.Next(_evaluatedGenomes.Count)];
                    } while (parent2 == parent1);

                    Genome child;
                    if(parent1.Fitness > parent2.Fitness)
                    {
                        child = _crossFunctionStrategy.GenerateOffspring(parent1.Genome, parent2.Genome);
                    }
                    else
                    {
                        child = _crossFunctionStrategy.GenerateOffspring(parent2.Genome, parent1.Genome);
                    }

                    if(_random.NextDouble() < _neatConfiguration.MutationRate)
                    {
                        _weightMutation.Mutate(child);
                    }

                    if(_random.NextDouble() < _neatConfiguration.AddConnectionRate)
                    {
                        _addConnectionMutation.Mutate(child);
                    }

                    if(_random.NextDouble() < _neatConfiguration.AddNodeRate)
                    {
                        _addNodeMutation.Mutate(child);
                    }
                    _nextGeneration.Add(child);
                }
                else
                {
                    var parent = _evaluatedGenomes[_random.Next(_evaluatedGenomes.Count)];
                    var child = parent.Genome.Copy();
                    _weightMutation.Mutate(child);
                    _nextGeneration.Add(child);
                }
            }

            //Transfer next generation to current generation
            _genomes.Clear();
            _genomes.AddRange(_nextGeneration);
        }

        public abstract double EvaluateGenome(Genome genome);
    }

    public interface IGenesisGenomeProvider
    {
        Genome GenerateGenesisGenome();
    }
}
