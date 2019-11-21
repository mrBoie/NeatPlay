using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using SoNEAT;
using SoNEAT.Facades;
using SoNEAT.InnovationPointGenerator;
using SoNEAT.Mutations;
using SoNEAT.Models;

namespace SoNEATTest.Mutations
{
    [TestFixture]
    public class AddNodeMutationTests
    {
        private IInnovationPointGenerator _nodeInnovationGenerator;
        private IInnovationPointGenerator _connectionInnovationGenerator;
        private IRandom _randomizer;

        [SetUp]
        public void Init()
        {
            _nodeInnovationGenerator = Substitute.For<IInnovationPointGenerator>();
            _connectionInnovationGenerator = Substitute.For<IInnovationPointGenerator>();
            _randomizer = Substitute.For<IRandom>();
        }

        [Test]
        public void AddNodeMutation_Mutate_DidNotAddNodesSinceNoConnections()
        {
            //ARRANGE
            var genome = CreateGenome(1, 1);

            var mutator = new AddNodeMutation(_nodeInnovationGenerator, _connectionInnovationGenerator, _randomizer);
            //ACT
            var result = mutator.Mutate(ref genome);

            //ASSERT
            Assert.IsFalse(result);
            Assert.AreEqual(0, genome.Connections.Count);
            Assert.AreEqual(2, genome.Nodes.Count);
        }

        [Test]
        public void AddNodeMutation_Mutate_DidNotAddNodesSinceNoEnabledConnections()
        {
            //ARRANGE
            var genome = CreateGenome(1, 1);
            genome.Connections.Add(1,
                new Connection {Id = 1, InNodeId = 1, OutNodeId = 2, IsEnabled = false, Weight = 1.0});

            var mutator = new AddNodeMutation(_nodeInnovationGenerator, _connectionInnovationGenerator, _randomizer);
            //ACT
            var result = mutator.Mutate(ref genome);

            //ASSERT
            Assert.IsFalse(result);
            Assert.AreEqual(1, genome.Connections.Count);
            Assert.AreEqual(2, genome.Nodes.Count);
        }

        [Test]
        public void AddNodeMutation_Mutate_AddsNodeAndConnection()
        {
            //ARRANGE
            var genome = CreateGenome(1,1);
            genome.Connections.Add(1,
                new Connection { Id = 1, InNodeId = 1, OutNodeId = 2, IsEnabled = true, Weight = 2.0 });

            _nodeInnovationGenerator.GetNextInnovation().Returns(3);
            _connectionInnovationGenerator.GetNextInnovation().Returns(2,3);
            _randomizer.Next(1).Returns(call => call.Arg<int>() - 1);

            var mutator = new AddNodeMutation(_nodeInnovationGenerator, _connectionInnovationGenerator, _randomizer);
            //ACT
            var result = mutator.Mutate(ref genome);

            //ASSERT
            Assert.IsTrue(result);
            Assert.AreEqual(3, genome.Connections.Count);
            Assert.AreEqual(3, genome.Nodes.Count);
            Assert.AreEqual(NodeType.Hidden, genome.Nodes[3].NodeType);

            AssertConnection(genome, 1, false, 1, 2, 2.0);
            AssertConnection(genome, 2, true, 1, 3, 1.0);
            AssertConnection(genome, 3, true, 3, 2, 2.0);
        }

        private void AssertConnection(Genome genome, int id, bool isEnabled, int inNode, int outNode, double weight)
        {
            Assert.AreEqual(isEnabled, genome.Connections[id].IsEnabled);
            Assert.AreEqual(inNode, genome.Connections[id].InNodeId);
            Assert.AreEqual(outNode, genome.Connections[id].OutNodeId);
            Assert.AreEqual(id, genome.Connections[id].Id);
            Assert.AreEqual(weight, genome.Connections[id].Weight);
        }

        private static Genome CreateGenome(int sensorNodes, int outputNodes)
        {
            var genome = new Genome();
            genome.Connections = new Dictionary<int, Connection>();

            var ids = Enumerable.Range(1, outputNodes + sensorNodes).ToArray();

            var sensorIds = ids.Take(sensorNodes).Select(s => new Node{ Id = s, NodeType = NodeType.Sensor});
            var outputIds = ids.Skip(sensorNodes).Take(sensorNodes).Select(s => new Node{ Id = s, NodeType = NodeType.Output});

            genome.Nodes = sensorIds.Concat(outputIds).ToDictionary(n => n.Id);

            return genome;
        }
    }
}
