using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class Inky : BaseGameSprite
    {
        Graph Map;

        public Inky(Texture2D texture, Vector2 position, Color color, Vector2 scale, Graph map)
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
                var targetVertex = Map.FindVertex(Game1.pac.Position);
                var path = Map.AStar(startVertex, targetVertex);
                //We pop right here because the first vertex in the stack
                //is the start position, which is were we are already
                path.Pop();
                var goalPos = path.Pop().Value;
                if (goalPos != Game1.pinky.Position && goalPos != Game1.Clyde.Position && goalPos != Game1.blinky.Position)
                {
                    Position = goalPos;
                }
            }
        }
    }
}
