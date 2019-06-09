using SoNEAT.InnovationPointGenerator;
using SoNEAT.Models;
using System;
using System.Linq;

namespace SoNEAT.Mutations
{
    public class AddNodeMutation : IMutation
    {
        private readonly IInnovationPointGenerator nodeInnovationGenerator;
        private readonly IInnovationPointGenerator connectionInnovationGenerator;
        private readonly Random random;

        public AddNodeMutation(IInnovationPointGenerator nodeInnovationGenerator, IInnovationPointGenerator connectionInnovationGenerator, Random random)
        {
            this.nodeInnovationGenerator = nodeInnovationGenerator;
            this.connectionInnovationGenerator = connectionInnovationGenerator;
            this.random = random;
        }

        public void Mutate(Genome genome)
        {
            var enabledConnections = genome.Connections.Values.Where(o => o.IsEnabled);

            var connection = enabledConnections.ElementAt(random.Next(enabledConnections.Count()));
            var (node, inConnection, outConnection) = GenerateNewNodeAndConnections(connection);

            genome.Nodes.Add(node.Id, node);
            genome.Connections.Add(inConnection.InnovationPoint, inConnection);
            genome.Connections.Add(outConnection.InnovationPoint, outConnection);

            connection.IsEnabled = false;
        }

        private (Node newNode, Connection inConnection, Connection outConnection) GenerateNewNodeAndConnections(Connection connection)
        {
            var node = new Node()
            {
                Id = nodeInnovationGenerator.GetNextInnovation(),
                NodeType = NodeType.Hidden
            };

            var inConnection = connection.Copy(connectionInnovationGenerator);
            inConnection.OutNodeId = node.Id;
            inConnection.Weight = 1;

            var outConnection = connection.Copy(connectionInnovationGenerator);
            outConnection.InNodeId = node.Id;

            return (node, inConnection, outConnection);
        }
    }
}
