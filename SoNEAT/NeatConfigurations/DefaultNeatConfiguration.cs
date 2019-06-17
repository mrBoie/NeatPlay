using System;
using System.Collections.Generic;
using System.Text;

namespace SoNEAT
{
    public class DefaultNeatConfiguration : INeatConfiguration
    {
        public double C1 => 1.0;

        public double C2 => 1.0;

        public double C3 => 0.4;

        public double DT => 3.0;

        public double ASexualReproductionRate => 0.25;

        public double MutationRate => 0.8;

        public double PerturbingRate => 0.9;

        public double DisabledGeneInheritingChance => 0.75;

        public double AddConnectionRate => 0.05;

        public double AddNodeRate => 0.03;

        public double OffspringFromCrossover => 0.75;

        public int PopulationSize { get; }

        public int MaxAttemptsAtFindingConnections => 10;

        public DefaultNeatConfiguration(int popSize)
        {
            PopulationSize = popSize;
        }
    }
}
