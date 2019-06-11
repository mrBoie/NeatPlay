using System;
using System.Collections.Generic;
using System.Text;

namespace SoNEAT.Comparers
{
    public interface IGenomeComparer
    {
        double CalculateCompibilityDistance(Genome genome1, Genome genome2);
    }
}
