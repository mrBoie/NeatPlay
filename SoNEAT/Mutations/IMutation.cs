using System;
using System.Collections.Generic;
using System.Text;

namespace SoNEAT.Mutations
{
    public interface IMutation
    {
        void Mutate(Genome genome);
    }
}
