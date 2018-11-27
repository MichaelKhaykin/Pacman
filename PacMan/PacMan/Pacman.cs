using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PacMan
{
    public class Pacman : BaseGameSprite
    {
        public Effect effect;
        public Texture2D AppliedSkin;

        public bool IsPowerActivated = false;

        public TimeSpan PowerTime;
        public TimeSpan elapsedPowerTime;

        Dictionary<Keys, Func<List<BaseGameSprite>, bool>> Movements;

        public Directions Direction;

        protected List<Rectangle> Frames = new List<Rectangle>();

        protected int currentIndex = 0;

        protected TimeSpan elapsedTimePerFrame;
        protected TimeSpan timePerFrame;

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
        
        private bool canMoveLeft;
        private bool canMoveRight;
        private bool canMoveUp;
        private bool canMoveDown;
        private Keys lastValidKeyPressed;

        public Pacman(Texture2D texture, Vector2 position, Color color, Vector2 scale)
            : base(texture, position, color, scale)
        {
            Movements = new Dictionary<Keys, Func<List<BaseGameSprite>, bool>>()
            {
                [Keys.W] = (walls) => Move(walls, new Vector2(Position.X, Position.Y - moveAmount), ref canMoveUp, Directions.Up),
                [Keys.S] = (walls) => Move(walls, new Vector2(Position.X, Position.Y + moveAmount), ref canMoveDown, Directions.Down),
                [Keys.D] = (walls) => Move(walls, new Vector2(Position.X + moveAmount, Position.Y), ref canMoveRight, Directions.Right),
                [Keys.A] = (walls) => Move(walls, new Vector2(Position.X - moveAmount, Position.Y), ref canMoveLeft, Directions.Left),

                [Keys.Up] = (walls) => Move(walls, new Vector2(Position.X, Position.Y - moveAmount), ref canMoveUp, Directions.Up),
                [Keys.Down] = (walls) => Move(walls, new Vector2(Position.X, Position.Y + moveAmount), ref canMoveDown, Directions.Down),
                [Keys.Right] = (walls) => Move(walls, new Vector2(Position.X + moveAmount, Position.Y), ref canMoveRight, Directions.Right),
                [Keys.Left] = (walls) => Move(walls, new Vector2(Position.X - moveAmount, Position.Y), ref canMoveLeft, Directions.Left),
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

            PowerTime = TimeSpan.FromSeconds(6);
            elapsedPowerTime = TimeSpan.Zero;
        }

        private bool Move(List<BaseGameSprite> walls, Vector2 pointToCheck, ref bool booleanToSet, Directions directionToSet)
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
            return shouldSetNewDirection;
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
                var lastKeyPressed = pressedKeys[pressedKeys.Length - 1];

                if (Movements.ContainsKey(lastKeyPressed))
                {
                    if (Movements[lastKeyPressed](walls))
                    {
                        lastValidKeyPressed = lastKeyPressed;
                    }
                }
            }

            Movements[lastValidKeyPressed](walls);

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
            if(effect != null)
            {
                effect.Parameters["Skin"].SetValue(AppliedSkin);
                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    sb.Draw(Texture, Position + Origin, Frames[currentIndex], Color, Rotation, Origin, Scale, SpriteEffects.None, 0f);
                }
            }
            else
            {
                sb.Draw(Texture, Position + Origin, Frames[currentIndex], Color, Rotation, Origin, Scale, SpriteEffects.None, 0f);
            }
        }
    }
}
