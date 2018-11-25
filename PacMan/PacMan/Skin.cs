using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacMan
{
    public class Skin : BaseGameSprite
    {
        public int SkinID;
        public bool isUnlocked = false;


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


        public Skin(Texture2D texture, Vector2 position, Color color, Vector2 scale) 
            : base(texture, position, color, scale)
        {
            //We can hardcode for all skins this list of rectangles
            //because all of these type of skins, only modify on top of the 
            //original pacman, the size shouldn't change
            Frames = new List<Rectangle>()
            {
                new Rectangle(0, 0, 40, 40),
                new Rectangle(0, 50, 40, 40),
                new Rectangle(0, 100, 40, 40)
            };

            elapsedTimePerFrame = TimeSpan.Zero;
            timePerFrame = TimeSpan.FromMilliseconds(200);

        }

        public override void Update(GameTime gameTime)
        {
            elapsedTimePerFrame += gameTime.ElapsedGameTime;
            
            if (elapsedTimePerFrame >= timePerFrame)
            {
                elapsedTimePerFrame = TimeSpan.Zero;
                currentIndex++;
                if (currentIndex >= Frames.Count)
                {
                    currentIndex = 0;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            if(!isUnlocked)
            {
                Color = Color.Gray * 0.7f;
            }
            else
            {
                Color = Color.White;
            }

            sb.Draw(Texture, Position + Origin, Frames[currentIndex], Color, Rotation, Origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
