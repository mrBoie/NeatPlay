namespace SoNEAT
{
    public class InovationGenerator : IInnovationPointGenerator
    {
        private int counter;

        public InovationGenerator(int initialValue)
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
