using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PacMan
{
    public class Pacman : Sprite
    {
        Dictionary<Keys, Action<GameTime>> Movements;
        
        public Directions Direction;

        private List<Rectangle> Frames = new List<Rectangle>();

        private int currentIndex = 0;

        private TimeSpan elapsedTimePerFrame;
        private TimeSpan timePerFrame;

        protected override Vector2 Origin
        {
            get
            {
                return new Vector2(Frames[currentIndex].Width / 2, Frames[currentIndex].Height / 2);
            }
        }

        public override Rectangle HitBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Frames[currentIndex].Width, Frames[currentIndex].Height);
            }
        }

        public Pacman(Texture2D texture, Vector2 position, Color color, Vector2 scale)
            : base(texture, position, color, scale)
        {
            Movements = new Dictionary<Keys, Action<GameTime>>()
            {
                [Keys.W] = new Action<GameTime>((gameTime) => Direction = Directions.Up),
                [Keys.S] = new Action<GameTime>((gameTime) => Direction = Directions.Down),
                [Keys.D] = new Action<GameTime>((gameTime) => Direction = Directions.Right),
                [Keys.A] = new Action<GameTime>((gameTime) => Direction = Directions.Left)
            };

            Direction = Directions.None;

            Frames = new List<Rectangle>()
            {
                new Rectangle(0, 0, 40, 40),
                new Rectangle(0, 50, 40, 40),
                new Rectangle(0, 100, 40, 40)
            };

            elapsedTimePerFrame = TimeSpan.Zero;
            timePerFrame = TimeSpan.FromMilliseconds(150);
            
        }
        
        public void Update(GameTime gameTime, List<Sprite> walls, GraphicsDevice graphicsDevice)
        {
            //IMPORTANT
            //THIS HAS TO RUN FIRST
            base.Update(gameTime);

            elapsedTimePerFrame += gameTime.ElapsedGameTime;
            
            if (elapsedTimePerFrame >= timePerFrame)
            {
                currentIndex++;
                elapsedTimePerFrame = TimeSpan.Zero;
            }

            if (currentIndex >= Frames.Count)
            {
                currentIndex = 0;
            }

            KeyboardState keyboard = Keyboard.GetState();
            var pressedKeys = keyboard.GetPressedKeys();

            if (pressedKeys.Length > 0)
            {
                var lastKeyPress = pressedKeys[pressedKeys.Length - 1];

                if (Movements.ContainsKey(lastKeyPress))
                {
                    Movements[lastKeyPress](gameTime);
                }
            }

            if(!HasElapsedTimePassed)
            {
                return;
            }
            
            //TELEPORT CHECK
            if(Position.X + HitBox.Width >= graphicsDevice.Viewport.Width)
            {
                Position.X = 0 + HitBox.Width;
            }
            else if(Position.X <= 0)
            {
                Position.X = graphicsDevice.Viewport.Width - HitBox.Width;
            }

            var projectedUpPosition = new Vector2(Position.X, Position.Y - moveAmount);
            var projectedDownPosition = new Vector2(Position.X, Position.Y + moveAmount);
            var projectedRightPosition = new Vector2(Position.X + moveAmount, Position.Y);
            var projectedLeftPosition = new Vector2(Position.X - moveAmount, Position.Y);

            bool canMoveLeft = true;
            bool canMoveRight = true;
            bool canMoveUp = true;
            bool canMoveDown = true;

            foreach(var wall in walls)
            {
                if(wall.HitBox.Contains(projectedUpPosition))
                {
                    canMoveUp = false;
                }
                if(wall.HitBox.Contains(projectedDownPosition))
                {
                    canMoveDown = false;
                }
                if(wall.HitBox.Contains(projectedLeftPosition))
                {
                    canMoveLeft = false;
                }
                if(wall.HitBox.Contains(projectedRightPosition))
                {
                    canMoveRight = false;
                }
            }

            switch (Direction)
            {
                case Directions.None:
                    break;

                case Directions.Up:
                    if (canMoveUp)
                    {
                        Rotation = -MathHelper.PiOver2;
                        Position.Y -= moveAmount;
                    }
                    break;

                case Directions.Down:
                    if (canMoveDown)
                    {
                        Rotation = MathHelper.PiOver2;
                        Position.Y += moveAmount;
                    }
                    break;

                case Directions.Right:
                    if(canMoveRight)
                    {
                        Rotation = 0;
                        Position.X += moveAmount;
                    }
                    break;

                case Directions.Left:
                    if (canMoveLeft)
                    {
                        Rotation = MathHelper.Pi;
                        Position.X -= moveAmount;
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Position + Origin, Frames[currentIndex], Color, Rotation, Origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
 