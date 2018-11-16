using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class Vertex
    {
        public Vertex Founder { get; set; } = null;
        public double FScore { get; set; } = double.MaxValue;
        public double GScore { get; set; } = double.MaxValue;
        public Vector2 Value { get; set; }
        public bool hasBeenVisited = false;
        public Vertex(Vector2 value)
        {
            Value = value;
        }

        public int CompareTo(Vertex other) => FScore.CompareTo(other.FScore);
    }
}
