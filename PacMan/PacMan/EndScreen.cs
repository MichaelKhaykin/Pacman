using MichaelLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class EndScreen : Screen
    {
        TextLabel label;

        public EndScreen(GraphicsDevice graphics, ContentManager content) 
            : base(graphics, content)
        {
            var font = Content.Load<SpriteFont>("BigFont");

            var text = Game1.DidWin ? "You Won" : "You Lose";
            label = new TextLabel(new Vector2(100, 100), Color.Black, text, font);


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
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            label.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }
    }
}
