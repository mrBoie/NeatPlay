using System;
using System.Collections.Generic;
using System.Text;

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
        public void Mutate(Genome genome)
        {
            foreach (var connection in genome.Connections.Values)
            {
                //TODO: Look into Gaussian
                if (_random.NextDouble() < _probabilityPerturbing)
                {
                    //TODO: NOTE IN THE LONG RUN THIS WILL CAUSE THE WEIGHTS TO FALL!
                    connection.Weight = connection.Weight * _random.NextDouble() - 0.5;
                }
                else
                {
                    connection.Weight = _random.NextDouble() * 4.0 - 2.0;
                }
            }
        }
    }
}
