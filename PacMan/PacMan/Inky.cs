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

            if (Position != GameScreen.pac.Position)
            {
                var twoinfrontofpacman = FindPositionBasedOffDirection();
                var newVector = twoinfrontofpacman - GameScreen.blinky.Position;

                var destinationVector = twoinfrontofpacman + newVector;

                destinationVector.X = MathHelper.Clamp(destinationVector.X, 0, 1000);
                destinationVector.Y = MathHelper.Clamp(destinationVector.Y, 0, 1000);

                int changeX = 40;
                int changeY = -40;

                destinationVector.X = (int)(destinationVector.X / 40) * 40;
                destinationVector.Y = (int)(destinationVector.Y / 40) * 40;

                while(Map.FindVertex(destinationVector) == null)
                {
                    if(destinationVector.X <= 0)
                    {
                        changeX = 40;
                    }
                    else if(destinationVector.X >= 1000)
                    {
                        changeX = -40;
                    }
                    if(destinationVector.Y <= 0)
                    {
                        changeY = 40;
                    }
                    else if(destinationVector.Y >= 800)
                    {
                        changeY = -40;
                    }

                    destinationVector.X += changeX;
                    destinationVector.Y += changeY;
                }

                if(Position == destinationVector)
                {
                    return;
                }

                var startVertex = Map.FindVertex(Position);
                var targetVertex = Map.FindVertex(destinationVector);
                
                var path = Map.AStar(startVertex, targetVertex);
                //We pop right here because the first vertex in the stack
                //is the start position, which is were we are already
                path.Pop();
                var goalPos = path.Pop().Value;
                if (goalPos != GameScreen.pinky.Position && goalPos != GameScreen.Clyde.Position && goalPos != GameScreen.blinky.Position)
                {
                    Position = goalPos;
                }
            }
        }

        private Vector2 FindPositionBasedOffDirection()
        {
            Vector2 newVector = GameScreen.pac.Position;

            int curr = 2;
            do
            {
                switch (GameScreen.pac.Direction)
                {
                    case Directions.None:
                        break;

                    case Directions.Up:
                        newVector.Y -= GameScreen.pac.HitBox.Height * curr;
                        break;
                    case Directions.Down:
                        newVector.Y += GameScreen.pac.HitBox.Height * curr;
                        break;
                    case Directions.Right:
                        newVector.X += GameScreen.pac.HitBox.Width * curr;
                        break;
                    case Directions.Left:
                        newVector.X -= GameScreen.pac.HitBox.Width * curr;
                        break;
                }
                curr--;
            } while (Map.FindVertex(newVector) == null);

            return newVector;
        }
    }
}
