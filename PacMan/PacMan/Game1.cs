using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PacMan
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D pixel;

        public static Pacman pac;
        public static Blinky blinky;
        public static Pinky pinky;
        public static Clyde Clyde;

        Graph Map;
        Graph InkyMap;

        List<Sprite> Walls = new List<Sprite>();

        List<Sprite> food;

        public static Random Rand = new Random();

        bool shouldStart = false;

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

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });


            var map = Content.Load<Texture2D>("Map");
            Color[] colorData = new Color[map.Height * map.Width];

            map.GetData(colorData);

            Func<Vertex, Vertex, double> Manhattan = (a, b) => 
            {
                return Math.Abs(b.Value.X - a.Value.X) + Math.Abs(b.Value.Y - a.Value.Y);
            };

            Func<Vertex, Vertex, double> Euclidean = (a, b) =>
            {
                return Math.Pow(b.Value.X - a.Value.X, 2) + Math.Pow(b.Value.Y - a.Value.Y, 2);
            };

            Map = new Graph(Manhattan);
            InkyMap = new Graph(Euclidean);

            var square = Content.Load<Texture2D>("PacManImage");
            var blinkyTexture = Content.Load<Texture2D>("blinky");
            var pinkyTexture = Content.Load<Texture2D>("pinky");
            var clydeTexture = Content.Load<Texture2D>("clyde");
            var pacManTexture = Content.Load<Texture2D>("pacmanspritesheet");
            var inkyTexture = Content.Load<Texture2D>("inky");

            var foodTexture = Content.Load<Texture2D>("PacManFood");
            food = new List<Sprite>();

            var wallColor = Color.Black;
            var invalidFoodColor = new Color(237, 28, 36);
            //add all verticies that aren't black
            for (int i = 0; i < colorData.Length; i++)
            {
                var x = i % map.Width * 40;
                var y = i / map.Width * 40;

                if (colorData[i] == wallColor)
                {
                    Walls.Add(new Sprite(square, new Vector2(x, y), Color.Black, Vector2.One));
                    continue;
                }


                if (colorData[i] != invalidFoodColor)
                {
                    Map.AddVertex(new Vector2(x, y));
                    InkyMap.AddVertex(new Vector2(x, y));
                    food.Add(new Sprite(foodTexture, new Vector2(x, y), Color.Yellow, Vector2.One));
                }
            }

            //make connections
            for (int i = 0; i < colorData.Length; i++)
            {
                if (colorData[i] == Color.Black)
                {
                    continue;
                }

                var x = i % map.Width * 40;
                var y = i / map.Width * 40;
                var currentVertex = Map.FindVertex(new Vector2(x, y));

                var leftVertex = Map.FindVertex(new Vector2(x - 40, y));
                var rightVertex = Map.FindVertex(new Vector2(x + 40, y));
                var upVertex = Map.FindVertex(new Vector2(x, y - 40));
                var downVertex = Map.FindVertex(new Vector2(x, y + 40));

                Map.AddEdge(40, currentVertex, leftVertex);
                Map.AddEdge(40, currentVertex, rightVertex);
                Map.AddEdge(40, currentVertex, upVertex);
                Map.AddEdge(40, currentVertex, downVertex);

            
                var upLeftDiagonal = Map.FindVertex(new Vector2(x - 40, y - 40));
                var upRightDiagonal = Map.FindVertex(new Vector2(x + 40, y - 40));
                var downLeftDiagonal = Map.FindVertex(new Vector2(x - 40, y + 40));
                var downRightDiagonal = Map.FindVertex(new Vector2(x + 40, y + 40));

                InkyMap.AddEdge(40, currentVertex, leftVertex);
                InkyMap.AddEdge(40, currentVertex, rightVertex);
                InkyMap.AddEdge(40, currentVertex, upVertex);
                InkyMap.AddEdge(40, currentVertex, downVertex);
                InkyMap.AddEdge(40, currentVertex, upLeftDiagonal);
                InkyMap.AddEdge(40, currentVertex, upRightDiagonal);
                InkyMap.AddEdge(40, currentVertex, downLeftDiagonal);
                InkyMap.AddEdge(40, currentVertex, downRightDiagonal);
            }


            pac = new Pacman(pacManTexture, new Vector2(120, 40), Color.Yellow, Vector2.One);
            pinky = new Pinky(pinkyTexture, new Vector2(400, 360), Color.White, Vector2.One, Map);
            Clyde = new Clyde(clydeTexture, new Vector2(440, 360), Color.White, Vector2.One, Map);
            blinky = new Blinky(blinkyTexture, new Vector2(480, 360), Color.White, Vector2.One, Map);

            for (int i = 0; i < food.Count; i++)
            {
                if (food[i].Position == pinky.Position || food[i].Position == Clyde.Position
                    || food[i].Position == blinky.Position)
                {
                    food.RemoveAt(i);
                    i--;
                }
            }

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.S)
                || keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.A))
            {
                shouldStart = true;
            }

            if (shouldStart)
            {

                for (int i = 0; i < food.Count; i++)
                {
                    if (food[i].HitBox.Intersects(pac.HitBox))
                    {
                        food.RemoveAt(i);
                        i--;
                    }
                }

                pac.Update(gameTime, Walls, GraphicsDevice);
                blinky.Update(gameTime);
                pinky.Update(gameTime);
                Clyde.Update(gameTime);

            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            
            for(int i = 0; i <= GraphicsDevice.Viewport.Width / pac.Texture.Width; i++)
            {
                spriteBatch.Draw(pixel, new Rectangle(i * 40, 0, 1, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.Draw(pixel, new Rectangle(0, i * 40, GraphicsDevice.Viewport.Width, 1), Color.White);
            }
            
            foreach (var fud in food)
            {
                fud.Draw(spriteBatch);
            }

            foreach (var wall in Walls)
            {
                wall.Draw(spriteBatch);
            }


            pac.Draw(spriteBatch);
            blinky.Draw(spriteBatch);
            pinky.Draw(spriteBatch);
            Clyde.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
