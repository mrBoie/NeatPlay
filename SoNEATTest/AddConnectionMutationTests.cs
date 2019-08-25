using NUnit.Framework;
using SoNEAT;
using SoNEAT.InnovationPointGenerator;
using SoNEAT.Mutations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoNEATTest
{
    [TestFixture]
    public class AddConnectionMutationTests
    {
        private IInnovationPointGenerator _innovationPointGenerator;
        private Random _random;

        [Test]
        public void AddConnectionMutateGenome()
        {
            var config = new DefaultNeatConfiguration(23);

            var genome = new Genome();
            genome.Nodes = new Dictionary<int, Node>();
            genome.Connections = new Dictionary<int, SoNEAT.Models.Connection>();

            genome.Nodes.Add(1, new Node { Id = 1, NodeType = SoNEAT.Models.NodeType.Sensor });
            genome.Nodes.Add(2, new Node { Id = 2, NodeType = SoNEAT.Models.NodeType.Output });

            var addConnectionMutator = new AddConnectionMutation(_innovationPointGenerator, _random, config);
        }
    }
}
