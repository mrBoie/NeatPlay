using System;

namespace SoNEAT.Mutations
{
    public class ApplyWeightMutation : IMutation
    {
        private readonly Random _random;
        private readonly double _probabilityPerturbing;

        public ApplyWeightMutation(Random random, double probabilityPerturbing)
        {
            _random = random;
            _probabilityPerturbing = probabilityPerturbing;
        }

        public ApplyWeightMutation(Random random, INeatConfiguration configuration)
        {
            _random = random;
            _probabilityPerturbing = configuration.PerturbingRate;
        }

        public void Mutate(Genome genome)
        {
            foreach (var connection in genome.Connections.Values)
            {
                //TODO: Look into Gaussian
                if (_random.NextDouble() < _probabilityPerturbing)
                {
                    //TODO: NOTE IN THE LONG RUN THIS WILL CAUSE THE WEIGHTS TO FALL!
                    var weight = connection.Weight;
                    weight = weight * (_random.NextDouble() - 0.5);
                    connection.Weight = weight;
                }
                else
                {
                    var weight = connection.Weight;
                    weight = weight * ((_random.NextDouble() * 4.0) - 2.0);
                    connection.Weight = weight;
                }
            }
        }
    }
}
