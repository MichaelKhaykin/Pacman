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

        public int Cost;
        
        public Rectangle HitBox { get; set; }

        public string Name;

        bool drawBox = false;

        public PacManDirectSkin(Texture2D originalTexture, Texture2D appliedTexture, Vector2 position, Color color, Vector2 scale, Effect effect, int money, ContentManager content, string name, TimeSpan timePerFrame) 
            : base(originalTexture, position, color, scale, timePerFrame, content)
        {
            AppliedSkin = appliedTexture;
            Effect = effect;

            elapsedTimePerFrame = TimeSpan.FromMilliseconds(200);

            var font = content.Load<SpriteFont>("Font");
            MoneyLabel = new TextLabel(new Vector2(Position.X, (Position.Y + (40 * Scale.Y) - 20)), Color.White, $"${money}", font);

            Cost = money;

            Name = name;
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTimePerFrame += gameTime.ElapsedGameTime;
            if(elapsedTimePerFrame >= TimePerFrame)
            {
                elapsedTimePerFrame = TimeSpan.Zero;
                currentIndex++;
                if(currentIndex >= Frames.Count)
                {
                    currentIndex = 0;
                }
            }

            if (!IsUnlocked)
            {
                Color = Color.Gray * 0.6f;
            }
            else
            {
                Color = Color.White;
            }
            if (HitBox.Contains(Game1.Mouse.Position))
            {
                drawBox = true;
            }
            else
            {
                drawBox = false;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.End();
            sb.Begin();
            MoneyLabel.Draw(sb);
            if(drawBox)
            {
                sb.Draw(Game1.Pixel, HitBox, Color.Red);
            }
            sb.End();
            sb.Begin(sortMode: SpriteSortMode.Immediate);

            if (!Apply)
            {
                base.Draw(sb);
                return;
            }

            if(Effect == null)
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
