using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacMan
{
    public class Heart : BaseGameSprite
    {
        public bool ShouldFlash = false;
        public TimeSpan elapsedTime;
        public TimeSpan showTime;
        public HeartStates HeartState;
        public int FlipCount = 0;
        public bool ShouldBeRemoved = false;

        public Heart(Texture2D texture, Vector2 position, Color color, Vector2 scale) : base(texture, position, color, scale)
        {
            elapsedTime = TimeSpan.Zero;
            showTime = TimeSpan.FromMilliseconds(500);
            HeartState = HeartStates.Show;
        }

        public override void Update(GameTime gameTime)
        {
            if(ShouldFlash)
            {
                elapsedTime += gameTime.ElapsedGameTime;
            }
            if(elapsedTime >= showTime)
            {
                elapsedTime = TimeSpan.Zero;
                HeartState = FlipState();
                FlipCount++;
            }

            if(FlipCount == 3)
            {
                ShouldBeRemoved = true;
            }

            base.Update(gameTime);
        }

        private HeartStates FlipState()
        {
            if(HeartState == HeartStates.Show)
            {
                return HeartStates.Hide;
            }
            else if(HeartState == HeartStates.Hide)
            {
                return HeartStates.Show;
            }

            return default(HeartStates);
        }

        public override void Draw(SpriteBatch sb)
        {
            if(ShouldFlash)
            {
                switch (HeartState)
                {
                    case HeartStates.Hide:
                        break;

                    case HeartStates.Show:
                        base.Draw(sb);
                        break;
                }
            }
            else
            {
                base.Draw(sb);
            }
        }
    }
}
