using SoNEAT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SoNEAT.Facades;

namespace SoNEAT.CrossFunctions
{
    public class NeatCrossFunction : ICrossFunctionStrategy
    {
        private readonly IRandom _random;
        private readonly double _disabledGeneInheritingChance;

        public NeatCrossFunction(IRandom random, INeatConfiguration configuration)
        {
            _random = random;
            _disabledGeneInheritingChance = configuration.DisabledGeneInheritingChance;
        }

        public Genome GenerateOffspring(Genome moreFitParent, Genome lessFitParent)
        {
            var newGenome = new Genome();

            newGenome.Nodes = GenerateBabyNodes(moreFitParent);

            newGenome.Connections = GenerateBabyConnections(moreFitParent, lessFitParent, _random, _disabledGeneInheritingChance);

            return newGenome;
        }

        private static Dictionary<int, Connection> GenerateBabyConnections(Genome moreFitParent, Genome lessFitParent, IRandom random, double disabledGeneInheritingChance)
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
                childConnections.Add(childConnection.Id, childConnection);
            }

            return childConnections;
        }

        private static Dictionary<int, Node> GenerateBabyNodes(Genome moreFitParent)
        {
            var babyNodes = moreFitParent.Nodes.Select(n => n.Value.Copy()).ToDictionary(k => k.Id, n => n);


            return babyNodes;
        }
    }
}
