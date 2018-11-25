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
    public class LoginScreen : Screen
    {
        Button CreateButton;
        Button LoginButton;
        Button PlayButton;

        TextBox UsernameBox;
        TextBox PasswordBox;

        SpriteFont font;

        MouseState oldMouse;

        bool canMoveOnToNextScreen = false;

        TextLabel WelcomeLabel;
        TextLabel UsernameLabel;
        TextLabel PasswordLabel;

        public LoginScreen(GraphicsDevice graphics, ContentManager content)
            : base(graphics, content)
        {
            font = Content.Load<SpriteFont>("Font");
            SpriteFont bigFont = Content.Load<SpriteFont>("BigFont");

            UsernameLabel = new TextLabel(new Vector2(400, 170), Color.Black, "Username:", font);
            PasswordLabel = new TextLabel(new Vector2(400, 370), Color.Black, "Password:", font);

            UsernameBox = new TextBox(Graphics, new Rectangle(200, 200, 600, 0), font, Color.Black, Color.White, Color.Red, false, false);
            PasswordBox = new TextBox(Graphics, new Rectangle(200, 400, 600, 0), font, Color.Black, Color.White, Color.Red, true, false);

            var createButtonTexture = Content.Load<Texture2D>("createButton");
            var scale = 0.4f;
            CreateButton = new Button(createButtonTexture, new Vector2(200 + (createButtonTexture.Width / 2) * scale, 600), Color.White, Vector2.One * scale, null);

            var loginButtonTexture = Content.Load<Texture2D>("tempbutton");
            LoginButton = new Button(loginButtonTexture, new Vector2(400, 600), Color.White, Vector2.One, null);

            var playButtonTexture = Content.Load<Texture2D>("nextScreenButton");
            PlayButton = new Button(playButtonTexture, new Vector2(600, 600), Color.White, Vector2.One, null);

            WelcomeLabel = new TextLabel(new Vector2(110, 0), Color.Black, "Welcome to PACMAN!", bigFont);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            UsernameBox.Update(gameTime);
            PasswordBox.Update(gameTime);

            if(CreateButton.IsClicked(mouse) && !CreateButton.IsClicked(oldMouse))
            {
                //call sql and crap

                if(UsernameBox.Text.Length == 0 && PasswordBox.Text.Length == 0)
                {
                    return;
                }

                SqlConnection connection = new SqlConnection(Game1.ConnectionString);

                SqlCommand command = new SqlCommand("usp_CreateUser", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Username", UsernameBox.Text));
                command.Parameters.Add(new SqlParameter("@Password", PasswordBox.Text));

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                connection.Open();
                adapter.Fill(table);
                connection.Close();

                UsernameBox.ClearText();
                PasswordBox.ClearText();

                //table created at this point
            }
            if (LoginButton.IsClicked(mouse) && !LoginButton.IsClicked(oldMouse))
            {
                if (UsernameBox.Text.Length == 0 && PasswordBox.Text.Length == 0)
                {
                    return;
                }

                SqlConnection connection = new SqlConnection(Game1.ConnectionString);

                SqlCommand command = new SqlCommand("usp_Login", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@Username", UsernameBox.Text));
                command.Parameters.Add(new SqlParameter("@Password", PasswordBox.Text));

                DataTable table = new DataTable();

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                connection.Open();
                adapter.Fill(table);
                connection.Close();

                //succesful login
                if(table.Rows.Count > 0)
                {
                    var highestScore = (int)table.Rows[0]["HighestScore"];
                    var money = (int)table.Rows[0]["Money"];
                    var playerID = (int)table.Rows[0]["PlayerID"];

                    Game1.CurrentUserData = new UserData(highestScore, money, playerID);
                    canMoveOnToNextScreen = true;

                    UsernameBox.ClearText();
                    PasswordBox.ClearText();
                }
            }
            if(PlayButton.IsClicked(mouse) && !PlayButton.IsClicked(oldMouse) && canMoveOnToNextScreen)
            {
                Game1.CurrentState = States.SkinSelection;
            }

            oldMouse = mouse;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            CreateButton.Draw(spriteBatch);
            UsernameBox.Draw(spriteBatch);
            PasswordBox.Draw(spriteBatch);
            LoginButton.Draw(spriteBatch);
            PlayButton.Draw(spriteBatch);
            UsernameLabel.Draw(spriteBatch);
            PasswordLabel.Draw(spriteBatch);
            WelcomeLabel.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }
    }
}
