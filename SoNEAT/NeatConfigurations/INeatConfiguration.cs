using System;
using System.Collections.Generic;
using System.Text;

namespace SoNEAT
{
    public interface INeatConfiguration
    {
        /// <summary>
        /// Constant used in genomic distance calculation - this is the weight of excess genes
        /// </summary>
        double C1 { get; }

        /// <summary>
        /// Constant used in genomic distance calculation - this is the weight of disjoint genes
        /// </summary>
        double C2 { get; }

        /// <summary>
        /// Constant used in genomic distance calculation - this is the weight of average connection weight difference
        /// </summary>
        double C3 { get; }

        /// <summary>
        /// Genomic distance we allow before two genomes are in seperate species - two genomes belong to the same species if genomic difference is less than this number
        /// </summary>
        double DT { get; }

        /// <summary>
        /// Fraction of children genomes resulting from mutation without crossover. The remaining children come from mating with corssover.
        /// </summary>
        double ASexualReproductionRate { get; }

        /// <summary>
        /// Chance for each child to have it's weights mutated, each weight in the genome having a PERTURBING_RATE chance of being uniformly perturbed, and 1-PERTURBING_RATE chance of being assigned a new random value
        /// </summary>
        double MutationRate { get; }

        /// <summary>
        /// This applies to mutation of genomes.
	    /// Each child has a MUTATION_RATE chance of mutating in each generation
        /// </summary>
        double PerturbingRate { get; }

        /// <summary>
        /// Chance of a weight being disabled if it is disabled in either parent
        /// </summary>
        double DisabledGeneInheritingChance { get; }

        /// <summary>
        /// Chance of mutating a child in a way that adds a node to the genome.
        /// </summary>
        double AddConnectionRate { get; }

        /// <summary>
        /// Chance of mutating a child in a way that adds a connection to the genome.
        /// </summary>
        double AddNodeRate { get; }

        /// <summary>
        /// Percentage of offspring generated using crossover of two parents - the rest comes from asexual mutation
        /// </summary>
        double OffspringFromCrossover { get; }

        int PopulationSize { get; }
    }
}
