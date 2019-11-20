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

        [Test]
        public void AddConnectionMutateGenome()
        {
            //ARRANGE
            var config = new DefaultNeatConfiguration(23);

            _random = Substitute.For<IRandom>();
            var i = 0;
            _random.Next(Arg.Is<int>(2)).Returns(i++);
            _random.NextDouble().ReturnsForAnyArgs(1.0);

            _innovationPointGenerator = Substitute.For<IInnovationPointGenerator>();

            var genome = new Genome();
            genome.Nodes = new Dictionary<int, Node>();
            genome.Connections = new Dictionary<int, SoNEAT.Models.Connection>();

            genome.Nodes.Add(1, new Node { Id = 1, NodeType = SoNEAT.Models.NodeType.Sensor });
            genome.Nodes.Add(2, new Node { Id = 2, NodeType = SoNEAT.Models.NodeType.Output });

            var addConnectionMutator = new AddConnectionMutation(_innovationPointGenerator, _random, config);

            //ACT
            var result = addConnectionMutator.Mutate(ref genome);

            //ASSERT
            Assert.IsFalse(result);

            Assert.AreEqual(0, genome.Connections.Count());
        }
    }
}
