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

        Button BuyButton;

        TextLabel MoneyLabel;
        TextLabel CurrentSkinNameLabel;

        PacManDirectSkin currSkinClicked = null;

        public SkinsScreen(GraphicsDevice graphics, ContentManager content)
            : base(graphics, content)
        {
            var font = Content.Load<SpriteFont>("Font");

            MoneyLabel = new TextLabel(new Vector2(0, 0), Color.Black, "", font);
            CurrentSkinNameLabel = new TextLabel(new Vector2(0, 50), Color.Black, "", font);

            var playButtonTexture = Content.Load<Texture2D>("button_play");
            goToGameButton = new Button(playButtonTexture, new Vector2(500, 705), Color.White, Vector2.One * 0.3f, null);
            
            var buyButtonTexture = Content.Load<Texture2D>("BuyButton");
            BuyButton = new Button(buyButtonTexture, new Vector2(500, 600), Color.White, Vector2.One * 0.4f, null);

            Skins = new List<PacManDirectSkin>();

            var pacmanimage = Content.Load<Texture2D>("pacmanspritesheet");
            var effect = Content.Load<Effect>("skinEffect");

            var skinScale = 2;
            var yValue = 420;

            var animateFrameTime = TimeSpan.FromMilliseconds(200);

            #region AddCookie
            var ogSkin = new PacManDirectSkin(pacmanimage, null, new Vector2(20, yValue), Color.White, Vector2.One * skinScale, null, 0, Content, "O.G. PacMan", animateFrameTime);
            ogSkin.IsUnlocked = true;
            Skins.Add(ogSkin);

            var cookieTexture = Content.Load<Texture2D>("Cookie");
            var skin = new PacManDirectSkin(pacmanimage, cookieTexture, new Vector2(150, yValue), Color.White, Vector2.One * skinScale, effect, 350, Content, "ChocolateChip", animateFrameTime);
            Skins.Add(skin);

            var cookie2Texture = Content.Load<Texture2D>("Cookie-02");
            var skin2 = new PacManDirectSkin(pacmanimage, cookie2Texture, new Vector2(280, yValue), Color.White, Vector2.One * skinScale, effect, 600, Content, "Oreo", animateFrameTime);
            Skins.Add(skin2);

            var cookie3Texture = Content.Load<Texture2D>("Cookie-03");
            var skin3 = new PacManDirectSkin(pacmanimage, cookie3Texture, new Vector2(410, yValue), Color.White, Vector2.One * skinScale, effect, 850, Content, "Sprinkles", animateFrameTime);
            Skins.Add(skin3);

            var cookie4Texture = Content.Load<Texture2D>("Cookie-04");
            var skin4 = new PacManDirectSkin(pacmanimage, cookie4Texture, new Vector2(540, yValue), Color.White, Vector2.One * skinScale, effect, 1100, Content, "M&M", animateFrameTime);
            Skins.Add(skin4);

            //skipping 5 for now

            var cookie6Texture = Content.Load<Texture2D>("Cookie-06");
            var skin6 = new PacManDirectSkin(pacmanimage, cookie6Texture, new Vector2(670, yValue), Color.White, Vector2.One * skinScale, effect, 1350, Content, "Skull", animateFrameTime);
            Skins.Add(skin6);

            var cookie7Texture = Content.Load<Texture2D>("Cookie-07");
            var skin7 = new PacManDirectSkin(pacmanimage, cookie7Texture, new Vector2(800, yValue), Color.White, Vector2.One * skinScale, effect, 1600, Content, "Wall", animateFrameTime);
            Skins.Add(skin7);

            #endregion

            //set ids
            for (int i = 0; i < Skins.Count; i++)
            {
                int width = 40;
                int height = 40;
                int offsetX = width / 2;
                int offsetY = height / 2;
                Skins[i].HitBox = new Rectangle((int)(Skins[i].Position.X - offsetX), (int)(Skins[i].Position.Y - offsetY), (int)(width * Skins[i].Scale.X), (int)(height * Skins[i].Scale.Y));
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
                //j starts at 1 to skip the default O.G. pacman skin
                for (int j = 1; j < Skins.Count; j++)
                {
                    if (Skins[j].SkinID == (int)table.Rows[i]["SkinID"])
                    {
                        Skins[j].IsUnlocked = true;
                    }
                }
            }

            var pacManTexture = Content.Load<Texture2D>("pacmanspritesheet");
            
            //120, 40, scale 1
            GameScreen.pac = new Pacman(pacManTexture, new Vector2(400, 150), Color.Yellow, Vector2.One * 5, animateFrameTime);
        }

        public override void Update(GameTime gameTime)
        {
            //figure out which skins were already bought
            //going to need to call sql

            GameScreen.pac.Animate(gameTime);
           
            MoneyLabel.Text = $"Money:{Game1.CurrentUserData.Money}";

            foreach (var skin in Skins)
            {
                if (skin.HitBox.Contains(Game1.Mouse.Position) && Game1.Mouse.LeftButton == ButtonState.Pressed)
                {
                    BuyButton.IsVisible = !skin.IsUnlocked;

                    GameScreen.pac.effect = skin.Effect;
                    GameScreen.pac.AppliedSkin = skin.AppliedSkin;
                }
            }


            if (goToGameButton.IsClicked(Game1.Mouse) && !goToGameButton.IsClicked(Game1.OldMouse))
            {
                Game1.CurrentState = States.Play;
                Game1.Screens.Add(States.Play, new GameScreen(Graphics, Content));
            }

            if (BuyButton.IsClicked(Game1.Mouse) && !BuyButton.IsClicked(Game1.OldMouse))
            {
                if (currSkinClicked == null)
                {
                    return;
                }

                if (Game1.CurrentUserData.Money - currSkinClicked.Cost < 0 || currSkinClicked.IsUnlocked)
                {
                    return;
                }

                currSkinClicked.IsUnlocked = true;
                Game1.CurrentUserData.Money -= currSkinClicked.Cost;

                #region UpdatingMoney
                SqlConnection connection = new SqlConnection(Game1.ConnectionString);
                SqlCommand command = new SqlCommand("usp_UpdateMoney", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@PlayerID", Game1.CurrentUserData.PlayerID));
                command.Parameters.Add(new SqlParameter("@NewMoney", Game1.CurrentUserData.Money));

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                connection.Open();
                adapter.Fill(table);
                connection.Close();

                #endregion

                #region BuyingSkin
                command = new SqlCommand("usp_BuySkin", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@PlayerID", Game1.CurrentUserData.PlayerID));
                command.Parameters.Add(new SqlParameter("@SkinID", currSkinClicked.SkinID));

                table = new DataTable();
                adapter = new SqlDataAdapter(command);

                connection.Open();
                adapter.Fill(table);
                connection.Close();
                #endregion
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
            CurrentSkinNameLabel.Draw(spriteBatch);
            GameScreen.pac.Draw(spriteBatch);

            foreach (var skin in Skins)
            {
                skin.Draw(spriteBatch);
            }
            base.Draw(spriteBatch);
        }
    }
}
