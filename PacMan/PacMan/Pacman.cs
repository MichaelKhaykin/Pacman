using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PacMan
{
    public class Pacman : BaseGameSprite
    {
        public bool IsPowerActivated = false;

        private TimeSpan PowerTime;
        private TimeSpan elapsedPowerTime;

        Dictionary<Keys, Action<List<BaseGameSprite>>> Movements;

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

        private SpriteFont font;

        private bool canMoveLeft;
        private bool canMoveRight;
        private bool canMoveUp;
        private bool canMoveDown;
        private Keys lastKeyPressed;

        public Pacman(Texture2D texture, Vector2 position, Color color, Vector2 scale, SpriteFont font)
            : base(texture, position, color, scale)
        {
            this.font = font;

            Movements = new Dictionary<Keys, Action<List<BaseGameSprite>>>()
            {
                [Keys.W] = new Action<List<BaseGameSprite>>((walls) => Move(walls, new Vector2(Position.X, Position.Y - moveAmount), ref canMoveUp, Directions.Up)),
                [Keys.S] = new Action<List<BaseGameSprite>>((walls) => Move(walls, new Vector2(Position.X, Position.Y + moveAmount), ref canMoveDown, Directions.Down)),
                [Keys.D] = new Action<List<BaseGameSprite>>((walls) => Move(walls, new Vector2(Position.X + moveAmount, Position.Y), ref canMoveRight, Directions.Right)),
                [Keys.A] = new Action<List<BaseGameSprite>>((walls) => Move(walls, new Vector2(Position.X - moveAmount, Position.Y), ref canMoveLeft, Directions.Left)),

                [Keys.Up] = new Action<List<BaseGameSprite>>((walls) => Move(walls, new Vector2(Position.X, Position.Y - moveAmount), ref canMoveUp, Directions.Up)),
                [Keys.Down] = new Action<List<BaseGameSprite>>((walls) => Move(walls, new Vector2(Position.X, Position.Y + moveAmount), ref canMoveDown, Directions.Down)),
                [Keys.Right] = new Action<List<BaseGameSprite>>((walls) => Move(walls, new Vector2(Position.X + moveAmount, Position.Y), ref canMoveRight, Directions.Right)),
                [Keys.Left] = new Action<List<BaseGameSprite>>((walls) => Move(walls, new Vector2(Position.X - moveAmount, Position.Y), ref canMoveLeft, Directions.Left)),
            };

            Direction = Directions.None;

            Frames = new List<Rectangle>()
            {
                new Rectangle(0, 0, 40, 40),
                new Rectangle(0, 50, 40, 40),
                new Rectangle(0, 100, 40, 40)
            };

            elapsedTimePerFrame = TimeSpan.Zero;
            timePerFrame = TimeSpan.FromMilliseconds(75);

            PowerTime = TimeSpan.FromSeconds(2);
            elapsedPowerTime = TimeSpan.Zero;
        }

        private void Move(List<BaseGameSprite> walls, Vector2 pointToCheck, ref bool booleanToSet, Directions directionToSet)
        {
            bool shouldSetNewDirection = true;
            foreach (var wall in walls)
            {
                if (wall.HitBox.Contains(pointToCheck))
                {
                    shouldSetNewDirection = false;
                }
            }

            if (shouldSetNewDirection)
            {
                Direction = directionToSet;
            }

            booleanToSet = shouldSetNewDirection;
        }
        
        public void Update(GameTime gameTime, List<BaseGameSprite> walls, GraphicsDevice graphicsDevice)
        {
            //IMPORTANT
            //THIS HAS TO RUN FIRST
            base.Update(gameTime);

            elapsedTimePerFrame += gameTime.ElapsedGameTime;

            if (IsPowerActivated)
            {
                elapsedPowerTime += gameTime.ElapsedGameTime;
            }

            if (elapsedPowerTime >= PowerTime)
            {
                elapsedPowerTime = TimeSpan.Zero;
                IsPowerActivated = false;
            }

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
                lastKeyPressed = pressedKeys[pressedKeys.Length - 1];   
            }

            if (Movements.ContainsKey(lastKeyPressed))
            {
                Movements[lastKeyPressed](walls);
            }

            if (!HasElapsedTimePassed)
            {
                return;
            }

            //TELEPORT CHECK
            if (Position.X + HitBox.Width >= graphicsDevice.Viewport.Width)
            {
                Position.X = 0 + HitBox.Width;
            }
            else if (Position.X <= 0)
            {
                Position.X = graphicsDevice.Viewport.Width - HitBox.Width;
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
                    if (canMoveRight)
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
            sb.DrawString(font, $"X:{Position.X},Y:{Position.Y}", new Vector2(120, 120), Color.Red);

            if (IsPowerActivated)
            {
                sb.DrawString(font, $"TimeLeft:{PowerTime - elapsedPowerTime}", new Vector2(0, 0), Color.Red);
            }
        }
    }
}
