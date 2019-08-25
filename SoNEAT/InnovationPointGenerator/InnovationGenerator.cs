using System.Collections;

namespace SoNEAT.InnovationPointGenerator
{
    public class InnovationGenerator : IInnovationPointGenerator
    {
        private int counter;

        public InnovationGenerator(int initialValue)
        {
            counter = initialValue;
        }

        public int GetNextInnovation()
        {
            counter++;
            return counter;
        }
    }
}
