using System;
using SoNEAT.Facades;

namespace SoNEAT.Mutations
{
    public class ApplyWeightMutation : IMutation
    {
        private readonly IRandom _random;
        private readonly double _probabilityPerturbing;

        public ApplyWeightMutation(IRandom random, double probabilityPerturbing)
        {
            _random = random;
            _probabilityPerturbing = probabilityPerturbing;
        }

        public ApplyWeightMutation(IRandom random, INeatConfiguration configuration)
        {
            _random = random;
            _probabilityPerturbing = configuration.PerturbingRate;
        }

        public bool Mutate(ref Genome genome)
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
            return true;
        }
    }
}
