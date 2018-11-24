using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacMan
{
    public class Pinky : BaseGameSprite
    {
        Graph Map;

        public Pinky(Texture2D texture, Vector2 position, Color color, Vector2 scale, Graph map) 
            : base(texture, position, color, scale)
        {
            Map = map;
            UpdateTime = TimeSpan.FromMilliseconds(250);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var startingVertex = Map.FindVertex(Position);
            var endingVertex = Map.FindVertex(FindPositionBasedOffDirection());

            if(!HasElapsedTimePassed)
            {
                return;
            }

            if (Position != Game1.pac.Position)
            {
                var path = Map.AStar(startingVertex, endingVertex);
                path.Pop();
                if (path.Count > 0)
                {
                    //So they dont overlap with any other ghost
                    var goalPos = path.Pop().Value;
                    if (goalPos != Game1.blinky.Position && goalPos != Game1.Clyde.Position && goalPos != Game1.Inky.Position)
                    {
                        Position = goalPos;
                    }
                }
            }
        }

        private Vector2 FindPositionBasedOffDirection()
        {
            Vector2 newVector = Game1.pac.Position;
            
            int curr = 6;
            do
            {
                switch (Game1.pac.Direction)
                {
                    case Directions.None:
                        break;

                    case Directions.Up:
                        newVector.Y -= Game1.pac.HitBox.Height * curr;
                        break;
                    case Directions.Down:
                        newVector.Y += Game1.pac.HitBox.Height * curr;
                        break;
                    case Directions.Right:
                        newVector.X += Game1.pac.HitBox.Width * curr;
                        break;
                    case Directions.Left:
                        newVector.X -= Game1.pac.HitBox.Width * curr;
                        break;
                }
                curr--;
            } while (Map.FindVertex(newVector) == null);

            return newVector;
        }
    }
}
