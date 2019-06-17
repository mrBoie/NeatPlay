using NUnit.Framework;
using SoNEAT;
using SoNEAT.Models;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        private Genome genome1;
        private Genome genome2;


        [SetUp]
        public void Setup()
        {
            var random = new Random(23);
            genome1 = new Genome()
            {
                Nodes = new Dictionary<int, Node>
                {
                    {1, new Node { Id = 1, NodeType = NodeType.Sensor }},
                    {2, new Node { Id = 2, NodeType = NodeType.Sensor }},
                    {3, new Node { Id = 3, NodeType = NodeType.Sensor }},
                    {4, new Node { Id = 4, NodeType = NodeType.Output }},
                    {5, new Node { Id = 5, NodeType = NodeType.Hidden }},
                },
                Connections = new Dictionary<int, Connection>
                {
                    {1, new Connection { InNodeId = 1, OutNodeId = 4, Id = 1, IsEnabled = true, Weight = 0.7 }},
                    {2, new Connection { InNodeId = 2, OutNodeId = 4, Id = 2, IsEnabled = false, Weight = -0.5 }},
                    {3, new Connection { InNodeId = 3, OutNodeId = 4, Id = 3, IsEnabled = true, Weight = 0.5 }},
                    {4, new Connection { InNodeId = 2, OutNodeId = 5, Id = 4, IsEnabled = true, Weight = 0.2 }},
                    {5, new Connection { InNodeId = 5, OutNodeId = 4, Id = 5, IsEnabled = true, Weight = 0.4 }},
                    {6, new Connection { InNodeId = 1, OutNodeId = 5, Id = 6, IsEnabled = true, Weight = 0.6 }},
                    {11, new Connection { InNodeId = 4, OutNodeId = 5, Id = 11, IsEnabled = true, Weight = 0.6 }}
                }
            };

            genome2 = new Genome()
            {
                Nodes = new Dictionary<int, Node>()
                {
                    {1, new Node { Id = 1, NodeType = NodeType.Sensor}},
                    {2, new Node { Id = 2, NodeType = NodeType.Sensor}},
                    {3, new Node { Id = 3, NodeType = NodeType.Sensor}},
                    {4, new Node { Id = 4, NodeType = NodeType.Output}},
                    {5, new Node { Id = 5, NodeType = NodeType.Hidden}}
                },
                Connections = new Dictionary<int, Connection>
                {
                    {1, new Connection { InNodeId = 1, OutNodeId = 4, Id = 1, IsEnabled = true, Weight = 1}},
                    {2, new Connection { InNodeId = 2, OutNodeId = 4, Id = 2, IsEnabled = false, Weight = 1}},
                    {3, new Connection { InNodeId = 3, OutNodeId = 4, Id = 3, IsEnabled = true, Weight = 1}},
                    {4, new Connection { InNodeId = 2, OutNodeId = 5, Id = 4, IsEnabled = true, Weight = 1}},
                    {5, new Connection { InNodeId = 5, OutNodeId = 4, Id = 5, IsEnabled = true, Weight = 1}},
                    {8, new Connection { InNodeId = 1, OutNodeId = 5, Id = 8, IsEnabled = true, Weight = 1}},
                }
            };

            var genome3 = new Genome()
            {
                Nodes = new Dictionary<int, Node>()
                {
                    {1, new Node { Id = 1, NodeType = NodeType.Sensor}},
                    {2, new Node { Id = 2, NodeType = NodeType.Sensor}},
                    {3, new Node { Id = 3, NodeType = NodeType.Sensor}},
                    {4, new Node { Id = 4, NodeType = NodeType.Output}},
                    {5, new Node { Id = 5, NodeType = NodeType.Hidden}},
                    {6, new Node { Id = 6, NodeType = NodeType.Hidden}}
                },
                Connections = new Dictionary<int, Connection>
                {
                    {1, new Connection { InNodeId = 1, OutNodeId = 4, Id = 1, IsEnabled = true, Weight = 1} },
                    {2, new Connection { InNodeId = 2, OutNodeId = 4, Id = 2, IsEnabled = false, Weight = 1}},
                    {3, new Connection { InNodeId = 3, OutNodeId = 4, Id = 3, IsEnabled = true, Weight = 1}},
                    {4, new Connection { InNodeId = 2, OutNodeId = 5, Id = 4, IsEnabled = true, Weight = 1}},
                    {5, new Connection { InNodeId = 5, OutNodeId = 4, Id = 5, IsEnabled = false, Weight = 1}},
                    {6, new Connection { InNodeId = 5, OutNodeId = 6, Id = 6, IsEnabled = true, Weight = 1}},
                    {7, new Connection { InNodeId = 6, OutNodeId = 4, Id = 7, IsEnabled = true, Weight = 1}},
                    {9, new Connection { InNodeId = 5, OutNodeId = 4, Id = 9, IsEnabled = true, Weight = 1}},
                    {10, new Connection { InNodeId = 1, OutNodeId = 6, Id = 10, IsEnabled = true, Weight = 1}},
                }
            };

        }

        [Test]
        public void Test1()
        {
            var mermaid = genome1.GenerateMermaid();

            var a = "a";
        }
    }
}