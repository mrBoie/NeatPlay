using System.Collections.Generic;
using System.Text;

namespace SoNEAT.Facades
{
    public interface IRandom
    {
        int Next(int count);
        double NextDouble();
    }
}
