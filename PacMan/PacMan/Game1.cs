using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using MichaelLibrary;

namespace PacMan
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Point mouse;

        public static Random Rand = new Random();

        Color screenTint = Color.Gray * 0.5f;

        Dictionary<States, Screen> Screens = new Dictionary<States, Screen>();
        public static States CurrentState;

        public static string ConnectionString;

        public static UserData CurrentUserData;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ConnectionString = "Server=MichaelsLaptop; Database=PacManDatabase; Trusted_Connection=True";

            CurrentState = States.Login;

            Screens.Add(States.Login, new LoginScreen(GraphicsDevice, Content));
            Screens.Add(States.SkinSelection, new SkinsScreen(GraphicsDevice, Content));
            Screens.Add(States.Play, new GameScreen(GraphicsDevice, Content));

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            mouse = Mouse.GetState().Position;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Screens[CurrentState].Update(gameTime);

            // TODO: Add your update logic here
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();


            Screens[CurrentState].Draw(spriteBatch);

            //get current vertex the mouse is at and its neighbors
            /*
            Vector2 mousePos = new Vector2(mouse.X / 40 * 40, mouse.Y / 40 * 40);
            var mouseBlock = Map.FindVertex(mousePos);
            if (mouseBlock != null)
            {
                spriteBatch.Draw(pixel, new Rectangle((int)mousePos.X, (int)mousePos.Y, 40, 40), Color.Red * 0.70f);
                foreach (var edge in Map.Edges)
                {
                    if (edge.StartingVertex == mouseBlock)
                    {
                        var friend = edge.EndingVertex;
                        spriteBatch.Draw(pixel, new Rectangle((int)friend.Value.X, (int)friend.Value.Y, 40, 40), Color.Pink * 0.70f);
                    }
                }
            }
            */

           
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
