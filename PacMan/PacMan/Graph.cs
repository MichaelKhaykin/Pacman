using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class Graph
    {
        public List<Vertex> Vertexes = new List<Vertex>();
        public List<Edge> Edges = new List<Edge>();

        public Func<Vertex, Vertex, double> Hueristic;

        public Graph(Func<Vertex, Vertex, double> hueristic)
        {
            Hueristic = hueristic;
        }


        public void AddVertex(Vector2 value)
        {
            Vertex newVertex = new Vertex(value);
            Vertexes.Add(newVertex);
        }

        public void AddVertex(Vertex vertex)
        {
            Vertexes.Add(vertex);
        }

        public void AddEdge(double weightForEdge, Vertex firstVertex, Vertex secondVertex)
        {
            if(firstVertex == null || secondVertex == null)
            {
                return;
            }

            Edges.Add(new Edge(weightForEdge, firstVertex, secondVertex));
        }

        public void AddEdge(Edge edge)
        {
            Edges.Add(edge);
        }

        public Stack<Vertex> AStar(Vertex startingVertex, Vertex endingVertex)
        {
            if (startingVertex == null || endingVertex == null)
            {
                return null;
            }
            //setup
            //  set FSCORE, GSCORE, to infinity and FOUNDER TO NULL
            // set the GSCORE of start to 0, the FSCORE of start to Hueristic(start, end)
            for (int i = 0; i < Vertexes.Count; i++)
            {
                Vertexes[i].FScore = double.PositiveInfinity;
                Vertexes[i].GScore = double.PositiveInfinity;
                Vertexes[i].Founder = null;
                Vertexes[i].hasBeenVisited = false;
            }

            startingVertex.GScore = 0;
            startingVertex.FScore = Hueristic(startingVertex, endingVertex);


            //astar
            MinHeapTree queue = new MinHeapTree();

            queue.Add(startingVertex);
            while (queue.Count != 0)
            {
                var current = queue.Pop();

                if (current == endingVertex)
                {
                    break;
                }

                current.hasBeenVisited = true;

                //generate 
                for (int i = 0; i < Edges.Count; i++)
                {
                    if (Edges[i].StartingVertex == current)
                    {
                        var friend = Edges[i].EndingVertex;
                        if (friend.hasBeenVisited)
                        {
                            continue;
                        }

                        var tempGSCORE = current.GScore + Edges[i].Weight;
                        if (tempGSCORE < friend.GScore)
                        {
                            friend.Founder = current;
                            friend.GScore = tempGSCORE;
                            friend.FScore = friend.GScore + Hueristic(friend, endingVertex);
                        }

                        if (!queue.Contains(friend))
                        {
                            queue.Add(friend);
                        }
                    }
                }
            }
            //construct path
            Stack<Vertex> founders = new Stack<Vertex>();

            founders.Push(endingVertex);
            while (!founders.Peek().Equals(startingVertex))
            {
                founders.Push(founders.Peek().Founder);
            }
            return founders;
        }

        public Vertex FindVertex(Vector2 pos)
        {
            foreach(var vertex in Vertexes)
            {
                if(vertex.Value == pos)
                {
                    return vertex;
                }
            }

            return null;
        }
    }
}
