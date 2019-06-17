using System;

namespace SoNEAT.NeuralNet
{
    public class SigmoidActivationFunction : IActivationFunction
    {
        public double CalculateOutput(double input)
        {
            return (double)(1.0 / (1.0 + Math.Exp(-4.9 * input)));
        }
    }
}