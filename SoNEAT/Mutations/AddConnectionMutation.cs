using SoNEAT.InnovationPointGenerator;
using SoNEAT.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoNEAT.Mutations
{
    public class AddConnectionMutation : IMutation
    {
        private readonly IInnovationPointGenerator _connectionInnovationGenerator;
        private readonly Random _random;
        private readonly double _probabilityPerturbing;
        private readonly int _maxAttempts;

        public AddConnectionMutation(IInnovationPointGenerator connectionInnovationGenerator, Random random, double probabilityPerturbing, int maxAttempts)
        {
            _connectionInnovationGenerator = connectionInnovationGenerator;
            _random = random;
            _probabilityPerturbing = probabilityPerturbing;
            _maxAttempts = maxAttempts;
        }

        public AddConnectionMutation(InnovationGenerator connectionInovator, Random random, INeatConfiguration configuration) :
            this(connectionInovator, random, configuration.PerturbingRate, configuration.MaxAttemptsAtFindingConnections)
        { }

        public void Mutate(Genome genome)
        {
            var tries = 0;
            var success = false;

            var nodeIds = genome.Nodes.Keys.ToArray();

            while (tries < _maxAttempts && !success)
            {
                tries++;

                var node1 = genome.Nodes[nodeIds[_random.Next(nodeIds.Count())]];
                var node2 = genome.Nodes[nodeIds[_random.Next(nodeIds.Count())]];

                ReverseNodesIfNeeded(ref node1, ref node2);

                if (CheckForSimpleImpossibleConnections(node1, node2))
                {
                    continue;
                }

                if (CheckForCircularStructures(node1, node2, genome))
                {
                    continue;
                }

                if (AreNodesConnected(node1.Id, node2.Id, genome))
                {
                    continue;
                }

                var newConnection = CreateConnection(node1.Id, node2.Id);
                genome.Connections.Add(newConnection.Id, newConnection);

                success = true;
            }
        }

        private static void ReverseNodesIfNeeded(ref Node node1, ref Node node2)
        {
            var reversed = false;
            if (node1.NodeType == NodeType.Hidden && node2.NodeType == NodeType.Sensor) reversed = true;
            else if (node1.NodeType == NodeType.Output && node2.NodeType == NodeType.Hidden) reversed = true;
            else if (node1.NodeType == NodeType.Output && node2.NodeType == NodeType.Sensor) reversed = true;

            if (reversed)
            {
                var temp = node1;
                node1 = node2;
                node2 = temp;
            }
        }

        private static bool CheckForSimpleImpossibleConnections(Node node1, Node node2)
        {
            if (node1.NodeType == NodeType.Sensor && node2.NodeType == NodeType.Sensor) return true;
            else if (node1.NodeType == NodeType.Output && node2.NodeType == NodeType.Output) return true;
            else if (node1 == node2) return true;
            return false;
        }

        private bool CheckForCircularStructures(Node node1, Node node2, Genome genome)
        {
            var needsChecking = new List<int>();
            var nodeIds = new List<int>();

            foreach (var connection in genome.Connections.Values)
            {
                if (connection.InNodeId == node2.Id)
                {
                    needsChecking.Add(connection.OutNodeId);
                    nodeIds.Add(connection.OutNodeId);
                }
            }

            while (needsChecking.Count != 0)
            {
                int nodeId = needsChecking.First();
                foreach (var connection in genome.Connections.Values)
                {
                    if (connection.InNodeId == nodeId)
                    {
                        nodeIds.Add(connection.OutNodeId);
                        needsChecking.Add(connection.OutNodeId);
                    }
                }
                needsChecking.RemoveAt(0);
            }

            foreach (var nodeId in nodeIds)
            {
                if (nodeId == node1.Id)
                {
                    return true;
                }
            }

            return false;
        }

        private Connection CreateConnection(int inNode, int outNode)
        {
            return new Connection
            {
                InNodeId = inNode,
                OutNodeId = outNode,
                IsEnabled = false,
                Weight = (_random.NextDouble() * 2.0) - 1.0, //RANDOM WEIGHT
                Id = _connectionInnovationGenerator.GetNextInnovation()
            };
        }

        private bool AreNodesConnected(int nodeId1, int nodeId2, Genome genome)
        {
            return genome.Connections.Any(
                c =>
                    (c.Value.InNodeId == nodeId1 && c.Value.OutNodeId == nodeId2)
                    ||
                    (c.Value.InNodeId == nodeId2 && c.Value.OutNodeId == nodeId1));
        }
    }
}
