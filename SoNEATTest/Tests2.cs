using NUnit.Framework;
using SoNEAT;
using SoNEAT.CrossFunctions;
using SoNEAT.InnovationPointGenerator;
using SoNEAT.Models;
using SoNEAT.Mutations;
using System;
using System.Linq;
using System.Collections.Generic;
namespace SoNEATTest
{
    [TestFixture]
    public class Tests2
    {
        [Test]
        public void MyTestMethod()
        {
            var random = new Random(23);
            INeatConfiguration configuration = new DefaultNeatConfiguration(500);
            var nodeInovator = new InovationGenerator(1);
            var ConnectionInovator = new InovationGenerator(1);

            var genomeProvider = new GenomeProvider(nodeInovator, ConnectionInovator, random);

            var crossFunction = new NeatCrossFunction(random, configuration);

            var weightMutation = new ApplyWeightMutation(random, configuration);
            var addNodeMutation = new AddNodeMutation(nodeInovator, ConnectionInovator, random);
            var addConnectionMutation = new AddConnectionMutation(ConnectionInovator, random, configuration);

            var evaluator = new WeightOfAHundred(configuration, genomeProvider, nodeInovator, ConnectionInovator, addConnectionMutation, addNodeMutation, weightMutation, crossFunction, random);

            for(int i = 0; i < 1000; i++)
            {
                evaluator.EvaluateGeneration();
                Console.WriteLine($"Best Fitness: {evaluator.FittestGenome.Fitness}");
            }

            Assert.That(evaluator.FittestGenome.Fitness > 90);
        }

        public class WeightOfAHundred : Evaluator
        {
            public WeightOfAHundred(INeatConfiguration neatConfiguration, IGenesisGenomeProvider genomeProvider, IInnovationPointGenerator nodeInnovationGenerator, IInnovationPointGenerator connectionInnovationGenerator, IMutation addConnectionMutation, IMutation addNodeMutation, IMutation weightMutation, ICrossFunctionStrategy crossFunctionStrategy, Random random) : base(neatConfiguration, genomeProvider, nodeInnovationGenerator, connectionInnovationGenerator, addConnectionMutation, addNodeMutation, weightMutation, crossFunctionStrategy, random)
            {
            }

            public override double EvaluateGenome(Genome genome)
            {
                var sum = genome.Connections.Where(g => g.Value.IsEnabled).Sum(g => g.Value.Weight);
                sum = sum > 100 ? 0 : sum;

                var connections = genome.Connections.Count(c => c.Value.IsEnabled);
                connections = connections == 0 ? 1 : connections;

                sum = sum / connections;

                return sum;
            }
        }

        public class GenomeProvider : IGenesisGenomeProvider
        {
            private readonly Random _random;
            private Genome _GenomeGrandfather;

            public GenomeProvider(IInnovationPointGenerator nodeGenerator, IInnovationPointGenerator connectionGenerator, Random random)
            {
                _GenomeGrandfather = GenerateTemplateGenome(nodeGenerator, connectionGenerator);
                _random = random;
            }

            public GenomeProvider(IInnovationPointGenerator nodeGenerator, IInnovationPointGenerator connectionGenerator, Random random, Genome grandFather)
            {
                _GenomeGrandfather = grandFather;
                _random = random;
            }

            public Genome GenerateGenesisGenome()
            {
                var genome = _GenomeGrandfather.Copy();
                foreach (var connection in genome.Connections)
                {
                    connection.Value.Weight = _random.NextDouble();
                }
                return genome;
            }

            private Genome GenerateTemplateGenome(IInnovationPointGenerator nodeGenerator, IInnovationPointGenerator connectionGenerator)
            {
                var genome = new Genome();

                genome.Nodes = 
                    Enumerable
                        .Repeat(1, 3)
                        .Select(n => new Node
                            {
                                Id = nodeGenerator.GetNextInnovation(),
                                NodeType = NodeType.Sensor
                            })
                        .ToDictionary(n => n.Id, n => n);

                var outputId = nodeGenerator.GetNextInnovation();

                genome.Nodes.Add(outputId, new Node { Id = outputId, NodeType = NodeType.Output });

                var c1 = new Connection
                {
                    Id = connectionGenerator.GetNextInnovation(),
                    InNodeId = genome.Nodes.First(n => n.Value.NodeType == NodeType.Sensor).Key,
                    OutNodeId = genome.Nodes.First(n => n.Value.NodeType == NodeType.Output).Key,
                    IsEnabled = true,
                    Weight = 0.5
                };
                var c2 = new Connection
                {
                    Id = connectionGenerator.GetNextInnovation(),
                    InNodeId = genome.Nodes.Last(n => n.Value.NodeType == NodeType.Sensor).Key,
                    OutNodeId = genome.Nodes.First(n => n.Value.NodeType == NodeType.Output).Key,
                    IsEnabled = true,
                    Weight = 0.5
                };

                genome.Connections = new Dictionary<int, Connection>();
                genome.Connections.Add(c1.Id, c1);
                genome.Connections.Add(c2.Id, c2);

                return genome;
            }
        }
    }
}
