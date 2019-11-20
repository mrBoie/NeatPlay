using System;
using System.Collections.Generic;
using System.Text;

namespace SoNEAT.Facades
{
    public interface IRandom
    {
        int Next(int count);
        double NextDouble();
    }

    public class RandomImplementation : IRandom
    {
        private readonly Random _random;

        public RandomImplementation(int seed) => _random = new Random(seed);

        public int Next(int count) => _random.Next(count);
        public double NextDouble() => _random.NextDouble();
    }
}
