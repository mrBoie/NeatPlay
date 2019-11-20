using NUnit.Framework;
using SoNEAT;
using SoNEAT.CrossFunctions;
using SoNEAT.InnovationPointGenerator;
using SoNEAT.Models;
using SoNEAT.Mutations;
using SoNEAT.NeuralNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoNEAT.Facades;
using static SoNEATTest.Tests2;

namespace SoNEATTest
{
    [TestFixture]
    public class TestXORNN
    {
        [Test]
        public void test()
        {
            var random = new RandomImplementation(23);
            INeatConfiguration configuration = new DefaultNeatConfiguration(500);
            var nodeInovator = new InnovationGenerator(1);
            var connectionInovator = new InnovationGenerator(1);

            var problemDomain = new ProblemDomain(
                inputs:
                    new double[,]
                    {
                        { 0, 0, 1 },
                        { 0, 1, 1 },
                        { 1, 0, 1 },
                        { 1, 1, 1 }
                    },
                outputs:
                    new double[,]
                    {
                        { 0 },
                        { 1 },
                        { 1 },
                        { 0 }
                    });
            var genome = CreateOriginalGenome(nodeInovator, connectionInovator, problemDomain);

            var genomeProvider = new XORNNProvider(nodeInovator, connectionInovator, random, genome);

            var crossFunction = new NeatCrossFunction(random, configuration);

            var weightMutation = new ApplyWeightMutation(random, configuration);
            var addNodeMutation = new AddNodeMutation(nodeInovator, connectionInovator, random);
            var addConnectionMutation = new AddConnectionMutation(connectionInovator, random, configuration);

            var evaluator = new XORNNEvaluator(configuration, genomeProvider, nodeInovator, connectionInovator, addConnectionMutation, addNodeMutation, weightMutation, crossFunction, random, problemDomain);


            for (int i = 0; i < 1000; i++)
            {
                evaluator.EvaluateGeneration();
                Console.WriteLine($"Best Fitness: {evaluator.FittestGenome.Fitness}");
            }

            Assert.That(evaluator.FittestGenome.Fitness > 90);
        }

        private static Genome CreateOriginalGenome(InnovationGenerator nodeInovator, InnovationGenerator connectionInovator, ProblemDomain problemDomain)
        {
            var grandGenome = new Genome();

            grandGenome.Nodes = new Dictionary<int, Node>();
            for (int i = 0; i < problemDomain.Inputs.GetLength(1); i++)
            {
                var node = new Node { Id = nodeInovator.GetNextInnovation(), NodeType = SoNEAT.Models.NodeType.Sensor };
                grandGenome.Nodes.Add(node.Id, node);
            }

            for (int i = 0; i < problemDomain.Outputs.GetLength(1); i++)
            {
                var node = new Node { Id = nodeInovator.GetNextInnovation(), NodeType = SoNEAT.Models.NodeType.Output };
                grandGenome.Nodes.Add(node.Id, node);
            }

            grandGenome.Connections = new Dictionary<int, SoNEAT.Models.Connection>();
            foreach(var inNode in grandGenome.Nodes.Values.Where(n => n.NodeType == SoNEAT.Models.NodeType.Sensor))
            {
                foreach(var outNode in grandGenome.Nodes.Values.Where(n => n.NodeType == SoNEAT.Models.NodeType.Output))
                {
                    var connection = new Connection()
                    {
                        Id = connectionInovator.GetNextInnovation(),
                        InNodeId = inNode.Id,
                        OutNodeId = outNode.Id,
                        IsEnabled = true,
                        Weight = 1.0
                    };
                    grandGenome.Connections.Add(connection.Id, connection);
                }
            }

            return grandGenome;
        }
    }

    public class XORNNProvider : IGenesisGenomeProvider
    {
        private readonly IInnovationPointGenerator _nodeGenerator;
        private readonly IInnovationPointGenerator _connectionGenerator;
        private readonly IRandom _random;
        private readonly Genome _grandFather;

        public XORNNProvider(IInnovationPointGenerator nodeGenerator, IInnovationPointGenerator connectionGenerator, IRandom random, Genome grandFather)
        {
            _nodeGenerator = nodeGenerator;
            _connectionGenerator = connectionGenerator;
            _random = random;
            _grandFather = grandFather;
        }

        public Genome GenerateGenesisGenome()
        {
            var genome = _grandFather.Copy();
            foreach (var Connection in genome.Connections.Values)
            {
                Connection.Weight = _random.NextDouble();
            }
            return _grandFather.Copy();
        }
    }

    public class XORNNEvaluator : Evaluator
    {
        private readonly ProblemDomain _problemDomain;

        public XORNNEvaluator(INeatConfiguration neatConfiguration, IGenesisGenomeProvider genomeProvider, IInnovationPointGenerator nodeInnovationGenerator, IInnovationPointGenerator connectionInnovationGenerator, IMutation addConnectionMutation, IMutation addNodeMutation, IMutation weightMutation, ICrossFunctionStrategy crossFunctionStrategy, IRandom random, ProblemDomain problemDomain) : base(neatConfiguration, genomeProvider, nodeInnovationGenerator, connectionInnovationGenerator, addConnectionMutation, addNodeMutation, weightMutation, crossFunctionStrategy, random)
        {
            _problemDomain = problemDomain;
        }

        public override double EvaluateGenome(Genome genome)
        {
            var nn = new NeuralNetwork(genome);

            var totalDistance = 0.0;
            for (int i = 0; i < _problemDomain.Inputs.GetLength(0); i++)
            {
                var inputs = new double[]{ _problemDomain.Inputs[i, 0], _problemDomain.Inputs[i, 1], _problemDomain.Inputs[i, 2] };
                var outputs = nn.Calculate(inputs);

                var distance = Math.Abs(_problemDomain.Outputs[i, 0] - outputs[0]);
                totalDistance += Math.Pow(distance, 2);
            }

            if(genome.Connections.Count > 20)
            {
                totalDistance += 1.0 * (genome.Connections.Count - 20);
            }

            return 100.0 - totalDistance * 50.0;
        }
    }

    public class ProblemDomain
    {
        public ProblemDomain(double[,] inputs, double[,] outputs)
        {
            Inputs = inputs;
            Outputs = outputs;
        }

        public double[,] Inputs { get; }
        public double[,] Outputs { get; }
    }
}
