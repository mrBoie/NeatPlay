using NUnit.Framework;
using SoNEAT;
using SoNEAT.InnovationPointGenerator;
using SoNEAT.Mutations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;
using SoNEAT.Facades;

namespace SoNEATTest
{
    [TestFixture]
    public class AddConnectionMutationTests
    {
        private IInnovationPointGenerator _innovationPointGenerator;
        private IRandom _random;
        private DefaultNeatConfiguration _config;

        [SetUp]
        public void Init()
        {
            _config = new DefaultNeatConfiguration(23);
            _innovationPointGenerator = Substitute.For<IInnovationPointGenerator>();
            _random = Substitute.For<IRandom>();
        }

        [Test]
        public void AddConnectionMutation_Mutate_FailedToAddConnection()
        {
            //ARRANGE
            _random.Next(Arg.Is<int>(2)).Returns(1);
            _random.NextDouble().ReturnsForAnyArgs(1.0);

            var genome = CreateGenome();

            var addConnectionMutator = new AddConnectionMutation(_innovationPointGenerator, _random, _config);

            //ACT
            var result = addConnectionMutator.Mutate(ref genome);

            //ASSERT
            Assert.IsFalse(result);

            Assert.AreEqual(0, genome.Connections.Count());
        }

        [Test]
        public void AddConnectionMutation_Mutate_AddedConnection()
        {
            //ARRANGE
            var i = 0;
            _random.Next(Arg.Is<int>(2)).Returns(call =>
            {
                var ret = i;
                i = i + 1;
                return ret;
            });
            _random.NextDouble().ReturnsForAnyArgs(1.0);

            _innovationPointGenerator.GetNextInnovation().Returns(23);

            var genome = CreateGenome();

            var addConnectionMutator = new AddConnectionMutation(_innovationPointGenerator, _random, _config);

            //ACT
            var result = addConnectionMutator.Mutate(ref genome);

            //ASSERT
            Assert.IsTrue(result);

            Assert.AreEqual(1, genome.Connections.Count());

            Assert.AreEqual(1.0, genome.Connections.First().Value.Weight);
            Assert.AreEqual(23, genome.Connections.First().Value.Id);
        }

        private static Genome CreateGenome()
        {
            var genome = new Genome();
            genome.Nodes = new Dictionary<int, Node>();
            genome.Connections = new Dictionary<int, SoNEAT.Models.Connection>();

            genome.Nodes.Add(1, new Node {Id = 1, NodeType = SoNEAT.Models.NodeType.Sensor});
            genome.Nodes.Add(2, new Node {Id = 2, NodeType = SoNEAT.Models.NodeType.Output});
            return genome;
        }
    }
}
