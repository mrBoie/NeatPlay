﻿using System;
using System.Collections.Generic;

namespace SoNEAT.NeuralNet
{
    public class Neuron
    {
        public double Output { get; private set; }
        private List<double> _inputs;
        private int _inputsCount;

        public List<(int outputId, double weight)> OutputConnections { get; private set; }
        private readonly IActivationFunction _activationFunction;

        public Neuron(IActivationFunction activationFunction)
        {
            _inputsCount = 0;
            _inputs = new List<double>();
            OutputConnections = new List<(int outputId, double weight)>();
            _activationFunction = activationFunction;
        }

        public void AddOutputConnection(int outputID, double weight)
        {
            OutputConnections.Add((outputID, weight));
        }

        public void AddInputConnection()
        {
            _inputsCount++;
        }

        public double Calculate()
        {
            var sum = 0.0;
            foreach (var input in _inputs)
            {
                sum += input;
            }
            Output = _activationFunction.CalculateOutput(sum);
            return Output;
        }

        public bool IsReady()
        {
            return _inputsCount == _inputs.Count;
        }

        public void FeedInput(double input)
        {
            if (_inputs.Count < _inputsCount)
            {
                _inputs.Add(input);
            }
            else
            {
                throw new Exception();
            }
        }

        public void Reset()
        {
            _inputs.Clear();
            Output = 0;
        }
    }
}