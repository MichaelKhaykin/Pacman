using MichaelLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class SkinsScreen : Screen
    {
        List<PacManDirectSkin> Skins;

        Button goToGameButton;

        Button SelectButton;
        Button BuyButton;

        bool isSelecting = false;

        TextLabel MoneyLabel;

        public SkinsScreen(GraphicsDevice graphics, ContentManager content)
            : base(graphics, content)
        {
            

            var font = Content.Load<SpriteFont>("Font");

            MoneyLabel = new TextLabel(new Vector2(0, 0), Color.Black, "", font);

            var playButtonTexture = Content.Load<Texture2D>("button_play");
            goToGameButton = new Button(playButtonTexture, new Vector2(500, 705), Color.White, Vector2.One * 0.3f, null);

            var selectButtonTexture = Content.Load<Texture2D>("selectbutton");
            SelectButton = new Button(selectButtonTexture, new Vector2(500, 600), Color.White, Vector2.One * 0.3f, null);

            var buyButtonTexture = Content.Load<Texture2D>("BuyButton");
            BuyButton = new Button(buyButtonTexture, new Vector2(500, 500), Color.White, Vector2.One * 0.4f, null);

            Skins = new List<PacManDirectSkin>();

            var pacmanimage = Content.Load<Texture2D>("pacmanspritesheet");
            var effect = Content.Load<Effect>("skinEffect");

            #region AddCookie
            var cookieTexture = Content.Load<Texture2D>("Cookie");
            var skin = new PacManDirectSkin(pacmanimage, cookieTexture, new Vector2(200, 100), Color.White, Vector2.One * 3, effect, 350, Content);
            Skins.Add(skin);

            var cookie2Texture = Content.Load<Texture2D>("Cookie-02");
            var skin2 = new PacManDirectSkin(pacmanimage, cookie2Texture, new Vector2(400, 100), Color.White, Vector2.One * 3, effect, 600, Content);
            Skins.Add(skin2);

            var cookie3Texture = Content.Load<Texture2D>("Cookie-03");
            var skin3 = new PacManDirectSkin(pacmanimage, cookie3Texture, new Vector2(600, 100), Color.White, Vector2.One * 3, effect, 850, Content);
            Skins.Add(skin3);

            var cookie4Texture = Content.Load<Texture2D>("Cookie-04");
            var skin4 = new PacManDirectSkin(pacmanimage, cookie4Texture, new Vector2(800, 100), Color.White, Vector2.One * 3, effect, 1100, Content);
            Skins.Add(skin4);

            //skipping 5 for now

            var cookie6Texture = Content.Load<Texture2D>("Cookie-06");
            var skin6 = new PacManDirectSkin(pacmanimage, cookie6Texture, new Vector2(200, 300), Color.White, Vector2.One * 3, effect, 1350, Content);
            Skins.Add(skin6);

            var cookie7Texture = Content.Load<Texture2D>("Cookie-07");
            var skin7 = new PacManDirectSkin(pacmanimage, cookie7Texture, new Vector2(400, 300), Color.White, Vector2.One * 3, effect, 1600, Content);
            Skins.Add(skin7);

            #endregion

            //set ids
            for (int i = 0; i < Skins.Count; i++)
            {
                int width = GameScreen.pac.Texture.Width;
                int height = GameScreen.pac.Texture.Height;
                Skins[i].HitBox = new Rectangle((int)(Skins[i].Position.X - width), (int)(Skins[i].Position.Y - height), (int)(width * Skins[i].Scale.X), (int)(height * Skins[i].Scale.Y));
                Skins[i].SkinID = i + 1;
            }
            
            SqlConnection connection = new SqlConnection(Game1.ConnectionString);
            SqlCommand command = new SqlCommand("usp_GetBoughtSkinIDs", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@PlayerID", Game1.CurrentUserData.PlayerID));

            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            connection.Open();
            adapter.Fill(table);
            connection.Close();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < Skins.Count; j++)
                {
                    if (Skins[j].SkinID == (int)table.Rows[i]["SkinID"])
                    {
                        Skins[j].IsUnlocked = true;
                    }
                }
            }

            var pacManTexture = Content.Load<Texture2D>("pacmanspritesheet");

            GameScreen.pac = new Pacman(pacManTexture, new Vector2(120, 40), Color.Yellow, Vector2.One);
        }

        public override void Update(GameTime gameTime)
        {
            //figure out which skins were already bought
            //going to need to call sql

            MoneyLabel.Text = $"Money:{Game1.CurrentUserData.Money}";

            if(goToGameButton.IsClicked(Game1.Mouse) && !goToGameButton.IsClicked(Game1.OldMouse))
            {
                 Game1.CurrentState = States.Play;
                Game1.Screens.Add(States.Play, new GameScreen(Graphics, Content));
            }
            if(SelectButton.IsClicked(Game1.Mouse) && !SelectButton.IsClicked(Game1.OldMouse))
            {
                isSelecting = true;
            }

            if(isSelecting)
            {
                foreach (var skin in Skins)
                {
                    
                    if (skin.IsUnlocked && skin.HitBox.Contains(Game1.Mouse.Position) && Game1.Mouse.LeftButton == ButtonState.Pressed)
                    {
                        //figure out how to make a new texture
                        GameScreen.pac.effect = skin.Effect;
                        GameScreen.pac.AppliedSkin = skin.AppliedSkin;
                    }
                }
            }
            
            foreach (var skin in Skins)
            {
                skin.Update(gameTime);
            }
            
            base.Update(gameTime);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            MoneyLabel.Draw(spriteBatch);
            goToGameButton.Draw(spriteBatch);
            BuyButton.Draw(spriteBatch);
            SelectButton.Draw(spriteBatch);

            foreach (var skin in Skins)
            {
                skin.Draw(spriteBatch);
            }
            base.Draw(spriteBatch);
        }
    }
}
