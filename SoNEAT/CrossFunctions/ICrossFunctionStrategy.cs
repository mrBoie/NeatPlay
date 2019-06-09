using System;
using System.Collections.Generic;
using System.Text;

namespace SoNEAT.CrossFunctions
{
    public interface ICrossFunctionStrategy
    {
        Genome GenerateOffspring(Genome moreFitParent, Genome lessFitParent);
    }
}
