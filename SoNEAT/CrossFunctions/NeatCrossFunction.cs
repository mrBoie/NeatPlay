using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoNEAT.CrossFunctions
{
    public class NeatCrossFunction : ICrossFunctionStrategy
    {
        private readonly Random _random;
        private readonly double _disabledGeneInheritingChance;

        public NeatCrossFunction(Random random, double disabledGeneInheritingChance)
        {
            _random = random;
            _disabledGeneInheritingChance = disabledGeneInheritingChance;
        }

        public Genome GenerateOffspring(Genome moreFitParent, Genome lessFitParent)
        {
            var newGenome = new Genome();

            newGenome.Nodes = GenerateBabyNodes(moreFitParent, lessFitParent);

            newGenome.Connections = GenerateBabyConnections(moreFitParent, lessFitParent, _random, _disabledGeneInheritingChance);

            return newGenome;
        }

        private static Dictionary<int, Connection> GenerateBabyConnections(Genome moreFitParent, Genome lessFitParent, Random random, double disabledGeneInheritingChance)
        {
            var childConnections = new Dictionary<int, Connection>();

            foreach (var moreConnection in moreFitParent.Connections)
            {
                Connection childConnection;

                if (lessFitParent.Connections.TryGetValue(moreConnection.Key, out var lessConnection))
                {
                    childConnection = random.Next(2) == 1 ? moreConnection.Value.Copy() : lessConnection.Copy();

                    var idDisabledInOneParent = !(moreConnection.Value.IsEnabled && lessConnection.IsEnabled);
                    if (idDisabledInOneParent && random.NextDouble() < disabledGeneInheritingChance)
                    {
                        childConnection.IsEnabled = false;
                    }
                }
                else
                {
                    childConnection = moreConnection.Value.Copy();
                }
                childConnections.Add(childConnection.InnovationPoint, childConnection);
            }

            return childConnections;
        }

        private static Dictionary<int, Node> GenerateBabyNodes(Genome moreFitParent, Genome lessFitParent)
        {
            var babyNodes = moreFitParent.Nodes.Select(n => n.Value.Copy()).ToDictionary(k => k.Id, n => n);

            foreach (var lessFitNode in lessFitParent.Nodes.Values)
            {
                if (!babyNodes.ContainsKey(lessFitNode.Id))
                {
                    var babyNode = lessFitNode.Copy();
                    babyNodes.Add(babyNode.Id, babyNode);
                }
            }

            return babyNodes;
        }
    }
}
