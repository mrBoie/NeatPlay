using SoNEAT.InnovationPointGenerator;
using SoNEAT.Models;
using SoNEAT.Mutations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoNEAT
{
    public class Evaluator
    {
        private readonly INeatConfiguration _neatConfiguration;
        private readonly IInnovationPointGenerator _nodeInnovationGenerator;
        private readonly IInnovationPointGenerator _connectionInnovationGenerator;
        private readonly IMutation _addConnectionMutation;
        private readonly IMutation _addNodeMutation;
        private readonly IMutation _weightMutation;
        private List<Genome> _genomes;
        private List<FitnessGenome> _evaluatedGenomes;
        private List<Genome> _nextGenerator;
        private List<FitnessGenome> _lastGenerationResults;

        public Evaluator(INeatConfiguration neatConfiguration, IGenesisGenomeProvider genomeProvider, IInnovationPointGenerator nodeInnovationGenerator, IInnovationPointGenerator connectionInnovationGenerator, IMutation addConnectionMutation, IMutation addNodeMutation, IMutation weightMutation)
        {
            _neatConfiguration = neatConfiguration;
            _nodeInnovationGenerator = nodeInnovationGenerator;
            _connectionInnovationGenerator = connectionInnovationGenerator;
            _addConnectionMutation = addConnectionMutation;
            _addNodeMutation = addNodeMutation;
            _weightMutation = weightMutation;
            _genomes = Enumerable.Repeat(genomeProvider.GenerateGenesisGenome(), neatConfiguration.PopulationSize).ToList();

            _evaluatedGenomes = new List<FitnessGenome>();
            _nextGenerator = new List<Genome>();

            _lastGenerationResults = new List<FitnessGenome>();
        }


    }

    public interface IGenesisGenomeProvider
    {
        Genome GenerateGenesisGenome();
    }
}
