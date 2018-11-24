using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacMan
{
    public class Blinky : BaseGameSprite
    {
        Graph Map;

        public Blinky(Texture2D texture, Vector2 position, Color color, Vector2 scale, Graph map)
            : base(texture, position, color, scale)
        {
            Map = map;
            UpdateTime = TimeSpan.FromMilliseconds(250);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!HasElapsedTimePassed)
            {
                return;
            }

            if (Position != Game1.pac.Position)
            {
                var startVertex = Map.FindVertex(Position);

                if(startVertex == null)
                {
                    throw new Exception("Something went terribly wrong");
                }

                var targetVertex = Map.FindVertex(Game1.pac.Position);

                if(targetVertex == null)
                {
                    throw new Exception("PacMan is in a wall?");
                }

                var path = Map.AStar(startVertex, targetVertex);
                //We pop right here because the first vertex in the stack
                //is the start position, which is were we are already
                path.Pop();
                var goalPos = path.Pop().Value;
                if (goalPos != Game1.pinky.Position && goalPos != Game1.Clyde.Position && goalPos != Game1.Inky.Position)
                {
                    Position = goalPos;
                }
            }
        }
    }
}
