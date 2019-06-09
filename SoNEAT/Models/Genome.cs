using SoNEAT.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SoNEAT
{
    public class Genome
    {
        public Dictionary<int, Node> Nodes { get; set; }
        public Dictionary<int, Connection> Connections { get; set; }

        public Genome()
        {
        }

        public Genome Copy()
        {
            var genome = new Genome()
            {
                Connections = Connections.Values.Select(o => o.Copy()).ToDictionary(k => k.InnovationPoint, c => c),
                Nodes = Nodes.Values.Select(n => n.Copy()).ToDictionary(k => k.Id, n => n)
            };
            return genome;
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
