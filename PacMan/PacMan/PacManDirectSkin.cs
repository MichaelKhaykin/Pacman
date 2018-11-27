using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MichaelLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PacMan
{
    public class PacManDirectSkin : Pacman
    {
        public bool Apply { get; set; } = true;

        public Effect Effect;

        public int SkinID = 0;
        public bool IsUnlocked = false;
        
        TextLabel MoneyLabel;
        
        public PacManDirectSkin(Texture2D originalTexture, Texture2D appliedTexture, Vector2 position, Color color, Vector2 scale, Effect effect, int money, ContentManager content) 
            : base(originalTexture, position, color, scale)
        {
            AppliedSkin = appliedTexture;
            Effect = effect;

            elapsedTimePerFrame = TimeSpan.FromMilliseconds(200);

            var font = content.Load<SpriteFont>("Font");
            MoneyLabel = new TextLabel(new Vector2(Position.X, (Position.Y + (40 * Scale.Y) - 20)), Color.White, $"${money}", font);
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTimePerFrame += gameTime.ElapsedGameTime;
            if(elapsedTimePerFrame >= timePerFrame)
            {
                elapsedTimePerFrame = TimeSpan.Zero;
                currentIndex++;
                if(currentIndex >= Frames.Count)
                {
                    currentIndex = 0;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.End();
            sb.Begin();
            MoneyLabel.Draw(sb);
            sb.End();
            sb.Begin(sortMode: SpriteSortMode.Immediate);

            if(!IsUnlocked)
            {
                Color = Color.Gray * 0.6f;
            }

            if (!Apply)
            {
                base.Draw(sb);
                return;
            }

            Effect.Parameters["Skin"].SetValue(AppliedSkin);
            foreach(var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                base.Draw(sb);
            }
        }
    }
}
