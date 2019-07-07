using NUnit.Framework;
using SoNEAT;
using SoNEAT.InnovationPointGenerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoNEATTest
{
    [TestFixture]
    public class AddConenctionMutationTest
    {
        private IInnovationPointGenerator _innovationPointGenerator;
        private Random _random;

        [Test]
        public void AddConnectionMutateGenome()
        {
            var genome = new Genome();
            genome.Nodes = new Dictionary<int, Node>();
            genome.Connections = new Dictionary<int, SoNEAT.Models.Connection>();


        }
    }
}
