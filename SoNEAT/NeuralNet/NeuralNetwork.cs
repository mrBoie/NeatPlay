using System;
using System.Collections.Generic;
using System.Linq;

namespace SoNEAT.NeuralNet
{
    public class NeuralNetwork
    {
        private List<int> _inputs;
        private List<int> _outputs;
        private Dictionary<int, Neuron> _neurons;

        private List<Neuron> _unprocessed;

        public NeuralNetwork(Genome genome)
        {
            _inputs = new List<int>();
            _outputs = new List<int>();
            _neurons = new Dictionary<int, Neuron>();
            _unprocessed = new List<Neuron>();

            var activationFunction = new SigmoidActivationFunction();

            foreach (var node in genome.Nodes.Values)
            {
                var neuron = new Neuron(activationFunction);

                switch (node.NodeType)
                {
                    case Models.NodeType.Sensor:
                        neuron.AddInputConnection();
                        _inputs.Add(node.Id);
                        break;
                    case Models.NodeType.Output:
                        _outputs.Add(node.Id);
                        break;
                    default:
                        break;
                }
                _neurons.Add(node.Id, neuron);
            }

            foreach(var connection in genome.Connections.Values)
            {
                if (connection.IsEnabled)
                {
                    var inputNeuron = _neurons[connection.InNodeId];
                    inputNeuron.AddOutputConnection(connection.OutNodeId, connection.Weight);

                    var outputNeuron = _neurons[connection.OutNodeId];
                    outputNeuron.AddInputConnection();
                }
            }
        }

        public double[] Calculate(double[] input)
        {
            if (input.Length != _inputs.Count)
            {
                throw new Exception();
            }

            foreach (var neuron in _neurons.Values)
            {
                neuron.Reset();
            }

            _unprocessed.Clear();
            _unprocessed.AddRange(_neurons.Values);

            for (int i = 0; i < input.Length; i++)
            {
                var inputNeuron = _neurons[_inputs[i]];

                inputNeuron.FeedInput(input[i]);
                inputNeuron.Calculate();

                foreach (var output in inputNeuron.OutputConnections)
                {
                    _neurons[output.outputId].FeedInput(output.weight * inputNeuron.Output);
                }
                _unprocessed.Remove(inputNeuron);
            }

            var loops = 0;
            var processed = new List<Neuron>();
            while (_unprocessed.Count != processed.Count)
            {
                loops++;
                if (loops > 10000) return null;

                foreach (var ready in _unprocessed)
                {
                    if (ready.IsReady() && ready.Output == default(double))
                    {
                        ready.Calculate();

                        foreach (var output in ready.OutputConnections)
                        {
                            _neurons[output.outputId].FeedInput(output.weight * ready.Output);
                        }
                        processed.Add(ready);
                    }
                }
            }

            return _outputs.Select(o => _neurons[o].Output).ToArray();
        }



    }
}