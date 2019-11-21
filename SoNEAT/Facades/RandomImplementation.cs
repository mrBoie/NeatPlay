using System;

namespace SoNEAT.Facades
{
    public class RandomImplementation : IRandom
    {
        private readonly Random _random;

        public RandomImplementation(int seed) => _random = new Random(seed);

        public int Next(int count) => _random.Next(count);
        public double NextDouble() => _random.NextDouble();
    }
}