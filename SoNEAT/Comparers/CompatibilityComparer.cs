using SoNEAT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoNEAT.Comparers
{
    public class CompatibilityComparer : IGenomeComparer
    {
        private readonly double _c1;
        private readonly double _c2;
        private readonly double _c3;

        public CompatibilityComparer(double c1, double c2, double c3)
        {
            _c1 = c1;
            _c2 = c2;
            _c3 = c3;
        }

        public double CalculateCompibilityDistance(Genome genome1, Genome genome2)
        {
            int excessGenes = CountExessGenes(genome1, genome2);
            int disjointGenes = CountDisjointGenes(genome1, genome2);
            double avgWeightDiff = AverageWeightDifference(genome1, genome2);

            return excessGenes * _c1 + disjointGenes * _c2 + avgWeightDiff * _c3;
        }

        private int CountDisjointGenes(Genome genome1, Genome genome2)
        {
            var disjointGenes = GetDisjointGenes(genome1.Nodes, genome2.Nodes);

            disjointGenes += GetDisjointGenes(genome1.Connections, genome2.Connections);

            return disjointGenes;
        }

        private int CountExessGenes(Genome genome1, Genome genome2)
        {
            var excessGenes = GetExessGenes(genome1.Nodes, genome2.Nodes);

            excessGenes += GetExessGenes(genome1.Connections, genome2.Connections);

            return excessGenes;
        }

        private double AverageWeightDifference(Genome genome1, Genome genome2)
        {
            var mathingGenes = 0;
            var weightDifference = 0.0;

            var highestGenome1Innovation = genome1.Connections.Keys.Max();
            var highestGenome2Innovation = genome2.Connections.Keys.Max();

            var higestInnovation = Math.Max(highestGenome1Innovation, highestGenome2Innovation);

            for (int i = 0; i < higestInnovation; i++)
            {
                var connection1Exists = genome1.Connections.TryGetValue(i, out var connection1);
                var connection2Exists = genome1.Connections.TryGetValue(i, out var connection2);

                if (connection1Exists && connection2Exists)
                {
                    mathingGenes++;
                    weightDifference += Math.Abs(connection1.Weight - connection2.Weight);
                }
            }

            return weightDifference / mathingGenes;
        }

        private static int GetExessGenes<T>(Dictionary<int, T> genes1, Dictionary<int, T> genes2) where T : IGene
        {
            var excessGenes = 0;

            var highestGenome1Innovation = genes1.Keys.Max();
            var highestGenome2Innovation = genes2.Keys.Max();

            var higestInnovation = Math.Max(highestGenome1Innovation, highestGenome2Innovation);

            for (int i = 0; i < higestInnovation; i++)
            {
                var node1Exists = genes1.ContainsKey(i);
                var node2Exists = genes2.ContainsKey(i);

                if (!node1Exists && highestGenome1Innovation < i && node2Exists)
                {
                    excessGenes++;
                }
                else if (node1Exists && highestGenome2Innovation < i && !node2Exists)
                {
                    excessGenes++;
                }
            }

            return excessGenes;
        }

        private static int GetDisjointGenes<T>(Dictionary<int, T> genes1, Dictionary<int, T> genes2) where T : IGene
        {
            var disjointGenes = 0;

            var highestGenome1Innovation = genes1.Keys.Max();
            var highestGenome2Innovation = genes2.Keys.Max();

            var higestInnovation = Math.Max(highestGenome1Innovation, highestGenome2Innovation);

            for (int i = 0; i < higestInnovation; i++)
            {
                var node1Exists = genes1.ContainsKey(i);
                var node2Exists = genes2.ContainsKey(i);

                if (!node1Exists && highestGenome1Innovation > i && node2Exists)
                {
                    disjointGenes++;
                }
                else if (node1Exists && highestGenome2Innovation > i && !node2Exists)
                {
                    disjointGenes++;
                }
            }

            return disjointGenes;
        }
    }
}
