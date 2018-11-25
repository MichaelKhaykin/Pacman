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
    public class SkinsScreen : Screen
    {
        List<Skin> Skins;

        public SkinsScreen(GraphicsDevice graphics, ContentManager content)
            : base(graphics, content)
        {
            Skins = new List<Skin>();
            var skin1Texture = Content.Load<Texture2D>("pacmanskin1");
            Skins.Add(new Skin(skin1Texture, new Vector2(200, 200), Color.White, Vector2.One));


            //set ids
            for (int i = 0; i < Skins.Count; i++)
            {
                Skins[i].SkinID = i + 1;
            }
        }

        public override void Update(GameTime gameTime)
        {
            //figure out which skins were already bought
            //going to need to call sql
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
                for(int j = 0; j < Skins.Count; j++)
                {
                    if (Skins[j].SkinID == (int)table.Rows[i]["SkinID"])
                    {
                        Skins[j].isUnlocked = true;
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
            foreach (var skin in Skins)
            {
                skin.Draw(spriteBatch);
            }
            base.Draw(spriteBatch);
        }
    }
}
