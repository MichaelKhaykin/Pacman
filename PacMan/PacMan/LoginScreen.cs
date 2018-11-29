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
        
        bool canMoveOnToNextScreen = false;

        TextLabel WelcomeLabel;
        TextLabel UsernameLabel;
        TextLabel PasswordLabel;
        TextLabel ResponseLabel;

        public LoginScreen(GraphicsDevice graphics, ContentManager content)
            : base(graphics, content)
        {
            font = Content.Load<SpriteFont>("Font");
            SpriteFont bigFont = Content.Load<SpriteFont>("BigFont");

            UsernameLabel = new TextLabel(new Vector2(460, 170), Color.Black, "Username:", font);
            PasswordLabel = new TextLabel(new Vector2(460, 370), Color.Black, "Password:", font);

            UsernameBox = new TextBox(Graphics, new Rectangle(200, 200, 600, 0), font, Color.Black, Color.White, Color.Red, false, false);
            PasswordBox = new TextBox(Graphics, new Rectangle(200, 400, 600, 0), font, Color.Black, Color.White, Color.Red, true, false);

            ResponseLabel = new TextLabel(new Vector2(100, 100), Color.Black, "", bigFont);
            ResponseLabel.Rotation = MathHelper.ToRadians(90);

            var createButtonTexture = Content.Load<Texture2D>("CreateAnAccountButton");
            var scale = 0.5f;
            CreateButton = new Button(createButtonTexture, new Vector2(500, 500), Color.White, Vector2.One * scale, null);

            var loginButtonTexture = Content.Load<Texture2D>("SignInButton");
            LoginButton = new Button(loginButtonTexture, new Vector2(500, 600), Color.White, Vector2.One * scale, null);

            var playButtonTexture = Content.Load<Texture2D>("button_play");
            PlayButton = new Button(playButtonTexture, new Vector2(500, 705), Color.White, Vector2.One * 0.3f, null);

            WelcomeLabel = new TextLabel(new Vector2(110, 0), Color.Black, "Welcome to PACMAN!", bigFont);
        }

        public override void Update(GameTime gameTime)
        {
            UsernameBox.Update(gameTime);
            PasswordBox.Update(gameTime);

            if(CreateButton.IsClicked(Game1.Mouse) && !CreateButton.IsClicked(Game1.OldMouse))
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

                if (table.Rows.Count > 0)
                {
                    var id = int.Parse(table.Rows[0]["PlayerID"].ToString());
                    if (id < 0)
                    {
                        ResponseLabel.Text = "Username Taken";
                    }
                    else
                    {
                        ResponseLabel.Text = "Created!";
                    }
                }

                //table created at this point
            }
            if (LoginButton.IsClicked(Game1.Mouse) && !LoginButton.IsClicked(Game1.OldMouse))
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

                    ResponseLabel.Text = "Login Succesful";
                }
                else
                {
                    ResponseLabel.Text = "Invalid login";
                }
            }
            if(PlayButton.IsClicked(Game1.Mouse) && !PlayButton.IsClicked(Game1.OldMouse) && canMoveOnToNextScreen)
            {
                Game1.Screens.Add(ScreenStates.SkinSelection, new SkinsScreen(Graphics, Content));
                Game1.CurrentState = ScreenStates.SkinSelection;
            }

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
            ResponseLabel.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }
    }
}
