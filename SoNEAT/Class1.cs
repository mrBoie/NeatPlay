using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SoNEAT
{
    public class Genome
    {
        private readonly IInnovationPointGenerator _connectionInnovationGenerator;
        private readonly IInnovationPointGenerator _nodeInnovationPointGenerator;
        private readonly Random _random;

        public Dictionary<int, Node> Nodes { get; set; }
        public Dictionary<int, Connection> Connections { get; set; }

        public Genome(IInnovationPointGenerator connectionInnovationGenerator, IInnovationPointGenerator nodeInnovationPointGenerator, Random rand)
        {
            _connectionInnovationGenerator = connectionInnovationGenerator;
            _nodeInnovationPointGenerator = nodeInnovationPointGenerator;
            _random = rand;
        }

        public void Mutate(double ProbabilityPerturbing)
        {
            foreach(var connection in Connections.Values)
            {
                //TODO: Look into Gaussian
                if(_random.NextDouble() < ProbabilityPerturbing)
                {
                    //TODO: NOTE IN THE LONG RUN THIS WILL CAUSE THE WEIGHTS TO FALL!
                    connection.Weight = connection.Weight * _random.NextDouble() - 0.5;
                }
                else
                {
                    connection.Weight = _random.NextDouble() * 4.0 - 2.0;
                }
            }
        }

        public void AddConnectionMutation(int maxAttempts)
        {
            var tries = 0;
            var success = false;

            var nodeIds = Nodes.Keys.ToArray();

            while(tries < maxAttempts && !success)
            {
                tries++;

                var node1 = Nodes[nodeIds[_random.Next(nodeIds.Count())]];
                var node2 = Nodes[nodeIds[_random.Next(nodeIds.Count())]];

                ReverseNodesIfNeeded(ref node1, ref node2);

                if(CheckForSimpleImpossibleConnections(node1, node2))
                {
                    continue;
                }

                if(CheckForCircularStructures(node1, node2))
                {
                    continue;
                }

                if(AreNodesConnected(node1.Id, node2.Id))
                {
                    continue;
                }

                AddConnection(node1.Id, node2.Id);
                success = true;
            }
        }

        private bool CheckForCircularStructures(Node node1, Node node2)
        {
            var needsChecking = new List<int>();
            var nodeIds = new List<int>();

            foreach(var connection in Connections.Values)
            {
                if(connection.InNodeId == node2.Id)
                {
                    needsChecking.Add(connection.OutNodeId);
                    nodeIds.Add(connection.OutNodeId);
                }
            }

            while(needsChecking.Count != 0)
            {
                int nodeId = needsChecking.First();
                foreach(var connection in Connections.Values)
                {
                    if(connection.InNodeId == nodeId)
                    {
                        nodeIds.Add(connection.OutNodeId);
                        needsChecking.Add(connection.OutNodeId);
                    }
                }
                needsChecking.RemoveAt(0);
            }

            foreach (var nodeId in nodeIds)
            {
                if(nodeId == node1.Id)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CheckForSimpleImpossibleConnections(Node node1, Node node2)
        {
            if (node1.NodeType == NodeType.Sensor && node2.NodeType == NodeType.Sensor) return true;
            else if (node1.NodeType == NodeType.Output && node2.NodeType == NodeType.Output) return true;
            else if (node1 == node2) return true;
            return false;
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

        public void AddNodeMutation()
        {
            var enabledConnections = Connections.Where(o => o.Value.IsEnabled);

            var connection = enabledConnections.ElementAt(_random.Next(enabledConnections.Count()));
            AddNodeInPlaceOfConnection(connection.Value);
        }

        private void AddNodeInPlaceOfConnection(Connection connection)
        {
            var node = new Node()
            {
                Id = _nodeInnovationPointGenerator.GetNextInnovation(),
                NodeType = NodeType.Hidden
            };

            var inConnection = connection.Copy(_connectionInnovationGenerator);
            inConnection.OutNodeId = node.Id;
            inConnection.Weight = 1;

            var outConnection = connection.Copy(_connectionInnovationGenerator);
            outConnection.InNodeId = node.Id;

            Nodes.Add(node.Id, node);
            Connections.Add(inConnection.InnovationPoint, inConnection);
            Connections.Add(outConnection.InnovationPoint, outConnection);

            connection.IsEnabled = false;
        }

        private void AddConnection(int inNode, int outNode)
        {
            var innovationNumber = _connectionInnovationGenerator.GetNextInnovation();
            Connections.Add(innovationNumber, new Connection
            {
                InNodeId = inNode,
                OutNodeId = outNode,
                IsEnabled = false,
                Weight = (_random.NextDouble()*2.0) - 1.0, //RANDOM WEIGHT
                InnovationPoint = innovationNumber
            });
        }

        private bool AreNodesConnected(int nodeId1, int nodeId2)
        {
            return Connections.Any(
                c => 
                    (c.Value.InNodeId == nodeId1 && c.Value.OutNodeId == nodeId2) 
                    || 
                    (c.Value.InNodeId == nodeId2 && c.Value.OutNodeId == nodeId1));
        }

        public static Genome GenerateBaby(Genome moreFitParent, Genome lessFitParent, IInnovationPointGenerator connectionInnovationGenerator, IInnovationPointGenerator nodeInnovationGenerator, Random random, double disabledGeneInheritingChance)
        {
            var newGenome = new Genome(connectionInnovationGenerator, nodeInnovationGenerator, random);

            newGenome.Nodes = GenerateBabyNodes(moreFitParent, lessFitParent);

            newGenome.Connections = GenerateBabyConnections(moreFitParent, lessFitParent, random, disabledGeneInheritingChance);

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

        public string GenerateMermaid()
        {
            var output = "";

            //var grouped = Nodes.Select(n => n.Value).GroupBy(o => o.NodeType);

            output += "subgraph SensorNodes\n";
            output += string.Join("\n", Nodes.Where(o => o.Value.NodeType == NodeType.Sensor).Select(n => $"{n.Value.Id}({n.Value.Id})"));
            output += "\nend\n";

            output += "subgraph HiddenNodes\n";
            output += string.Join("\n", Nodes.Where(o => o.Value.NodeType == NodeType.Hidden).Select(n => $"{n.Value.Id}(({n.Value.Id}))"));
            output += "\nend\n";

            output += "subgraph OutputNodes\n";
            output += string.Join("\n", Nodes.Where(o => o.Value.NodeType == NodeType.Output).Select(n => $"{n.Value.Id}[{n.Value.Id}]"));
            output += "\nend\n";

            output += "\n";

            output += string.Join("\n",
                Connections.Select(
                    c =>
                    {
                        var outvar = $"{c.Value.InNodeId}";
                        switch (c.Value.IsEnabled)
                        {
                            case true:
                                outvar += "-->";
                                break;
                            case false:
                                outvar += "-.->";
                                break;
                        }
                        outvar += $"|I:{c.Value.InnovationPoint}, W:{c.Value.Weight.ToString(CultureInfo.InvariantCulture)}|{c.Value.OutNodeId}";
                        return outvar;
                    }));

            return output;
        }
    }
}
