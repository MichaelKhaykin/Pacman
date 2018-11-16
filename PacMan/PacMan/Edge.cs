using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class Edge
    {
        public Vertex StartingVertex { get; set; }
        public Vertex EndingVertex { get; set; }
        public double Weight { get; set; }

        public Edge(double weight, Vertex startingVertex = null, Vertex endingVertex = null)
        {
            Weight = weight;
            StartingVertex = startingVertex;
            EndingVertex = endingVertex;
        }

        public bool Equals(Edge other)
        {
            return (Weight == other.Weight && StartingVertex.Equals(other.StartingVertex) && EndingVertex.Equals(other.EndingVertex));
        }
    }
}
