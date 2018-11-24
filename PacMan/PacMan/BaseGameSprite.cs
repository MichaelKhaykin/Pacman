using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class BaseGameSprite
    {
        public Texture2D Texture { get; set; }
        public Vector2 StartingPosition;
        private Vector2 position;
        public ref Vector2 Position
        {
            get
            {
                return ref position;
            }
        }
        public Color Color { get; set; }
        public Vector2 Scale { get; set; }
        protected virtual Vector2 Origin { get; set; }
        protected float Rotation;

        public virtual Rectangle HitBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * Scale.X), (int)(Texture.Height * Scale.Y));
            }
        }

        protected int moveAmount = 0;

        private TimeSpan elapsedTime;
        protected TimeSpan UpdateTime;

        protected bool HasElapsedTimePassed = false;

        public BaseGameSprite(Texture2D texture, Vector2 position, Color color, Vector2 scale)
        {
            Origin = Vector2.Zero;

            Texture = texture;
            Position = position;
            Color = color;
            Scale = scale;
            
            moveAmount = Texture.Width;

            elapsedTime = TimeSpan.Zero;
            //controls update time
            UpdateTime = TimeSpan.FromMilliseconds(150);

            Rotation = 0;

            StartingPosition = position;
        }

        public virtual void Update(GameTime gameTime)
        {
            HasElapsedTimePassed = false;

            elapsedTime += gameTime.ElapsedGameTime;

            if(elapsedTime >= UpdateTime)
            {
                HasElapsedTimePassed = true;
                elapsedTime = TimeSpan.Zero;
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Position, null, Color, Rotation, Origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
