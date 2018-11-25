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
        Point mouse;
        Texture2D pixel;

        public static Pacman pac;
        public static Blinky blinky;
        public static Pinky pinky;
        public static Clyde Clyde;
        public static Inky Inky;

        List<BaseGameSprite> Ghosts;

        List<Keys> StartKeys;

        Graph Map;
        Graph InkyMap;

        List<BaseGameSprite> Walls = new List<BaseGameSprite>();

        List<BaseGameSprite> food;

        public static Random Rand = new Random();

        bool shouldStart = false;

        List<BaseGameSprite> Hearts = new List<BaseGameSprite>();

        List<BaseGameSprite> PowerUps = new List<BaseGameSprite>();

        Color screenTint = Color.Gray * 0.5f;

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

            #region DeclareStartKeys
            StartKeys = new List<Keys>();
            StartKeys.Add(Keys.W);
            StartKeys.Add(Keys.Up);
            StartKeys.Add(Keys.S);
            StartKeys.Add(Keys.Down);
            StartKeys.Add(Keys.D);
            StartKeys.Add(Keys.Left);
            StartKeys.Add(Keys.Right);
            StartKeys.Add(Keys.A);
            #endregion

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

            SpriteFont font = Content.Load<SpriteFont>("Font");

            Map = new Graph(Manhattan);
            InkyMap = new Graph(Euclidean);

            var square = Content.Load<Texture2D>("PacManImage");
            var blinkyTexture = Content.Load<Texture2D>("blinky");
            var pinkyTexture = Content.Load<Texture2D>("pinky");
            var clydeTexture = Content.Load<Texture2D>("clyde");
            var pacManTexture = Content.Load<Texture2D>("pacmanspritesheet");
            var inkyTexture = Content.Load<Texture2D>("inky");

            var foodTexture = Content.Load<Texture2D>("PacManFood");
            food = new List<BaseGameSprite>();

            var heartTexture = Content.Load<Texture2D>("RedHeart");

            var poweruptexture = Content.Load<Texture2D>("pacmanpowerup");

            var wallColor = Color.Black;
            var invalidFoodColor = new Color(237, 28, 36);
            var foodColor = new Color(255, 242, 0);

            //add all verticies that aren't black
            for (int i = 0; i < colorData.Length; i++)
            {
                var x = i % map.Width * 40;
                var y = i / map.Width * 40;

                if (colorData[i] == wallColor)
                {
                    if (Hearts.Count < 3)
                    {
                        Hearts.Add(new BaseGameSprite(heartTexture, new Vector2(x, 0), Color.White, Vector2.One));
                    }
                    Walls.Add(new BaseGameSprite(square, new Vector2(x, y), Color.Black, Vector2.One));
                    continue;
                }


                if (colorData[i] != invalidFoodColor)
                {
                    Map.AddVertex(new Vector2(x, y));
                    InkyMap.AddVertex(new Vector2(x, y));
                    food.Add(new BaseGameSprite(foodTexture, new Vector2(x, y), Color.Yellow, Vector2.One));
                }

                if (colorData[i] == foodColor)
                {
                    PowerUps.Add(new BaseGameSprite(poweruptexture, new Vector2(x, y), Color.White, Vector2.One));
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

                if (currentVertex == null) continue;

                var leftVertex = Map.FindVertex(new Vector2(x - 40, y));
                var rightVertex = Map.FindVertex(new Vector2(x + 40, y));
                var upVertex = Map.FindVertex(new Vector2(x, y - 40));
                var downVertex = Map.FindVertex(new Vector2(x, y + 40));

                Map.AddEdge(40, currentVertex, leftVertex);
                Map.AddEdge(40, currentVertex, rightVertex);
                Map.AddEdge(40, currentVertex, upVertex);
                Map.AddEdge(40, currentVertex, downVertex);


                var upLeftDiagonal = InkyMap.FindVertex(new Vector2(x - 40, y - 40));
                var upRightDiagonal = InkyMap.FindVertex(new Vector2(x + 40, y - 40));
                var downLeftDiagonal = InkyMap.FindVertex(new Vector2(x - 40, y + 40));
                var downRightDiagonal = InkyMap.FindVertex(new Vector2(x + 40, y + 40));

                currentVertex = InkyMap.FindVertex(new Vector2(x, y));

                leftVertex = InkyMap.FindVertex(new Vector2(x - 40, y));
                rightVertex = InkyMap.FindVertex(new Vector2(x + 40, y));
                upVertex = InkyMap.FindVertex(new Vector2(x, y - 40));
                downVertex = InkyMap.FindVertex(new Vector2(x, y + 40));


                InkyMap.AddEdge(40, currentVertex, leftVertex);
                InkyMap.AddEdge(40, currentVertex, rightVertex);
                InkyMap.AddEdge(40, currentVertex, upVertex);
                InkyMap.AddEdge(40, currentVertex, downVertex);
                InkyMap.AddEdge(40, currentVertex, upLeftDiagonal);
                InkyMap.AddEdge(40, currentVertex, upRightDiagonal);
                InkyMap.AddEdge(40, currentVertex, downLeftDiagonal);
                InkyMap.AddEdge(40, currentVertex, downRightDiagonal);
            }


            pac = new Pacman(pacManTexture, new Vector2(120, 40), Color.Yellow, Vector2.One, font);
            pinky = new Pinky(pinkyTexture, new Vector2(400, 360), Color.White, Vector2.One, Map);
            Clyde = new Clyde(clydeTexture, new Vector2(440, 360), Color.White, Vector2.One, Map);
            blinky = new Blinky(blinkyTexture, new Vector2(480, 360), Color.White, Vector2.One, Map);
            Inky = new Inky(inkyTexture, new Vector2(520, 360), Color.White, Vector2.One, InkyMap);

            for (int i = 0; i < food.Count; i++)
            {
                if (food[i].Position == pinky.Position || food[i].Position == Clyde.Position
                    || food[i].Position == blinky.Position)
                {
                    food.RemoveAt(i);
                    i--;
                }
            }

            Ghosts = new List<BaseGameSprite>();
            Ghosts.Add(Inky);
            Ghosts.Add(blinky);
            Ghosts.Add(Clyde);
            Ghosts.Add(pinky);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            mouse = Mouse.GetState().Position;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here


            var keyboard = Keyboard.GetState();
            foreach (var key in StartKeys)
            {
                if (keyboard.IsKeyDown(key))
                {
                    shouldStart = true;
                }
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
                Inky.Update(gameTime);

                
                foreach (var ghost in Ghosts)
                {
                    if (pac.HitBox.Contains(ghost.HitBox) && !pac.IsPowerActivated)
                    {
                        if (Hearts.Count > 0)
                        {
                            Hearts.RemoveAt(Hearts.Count - 1);
                        }
                        shouldStart = false;

                        pac.Position = new Vector2(120, 40);
                        pinky.Position = new Vector2(400, 360);
                        Clyde.Position = new Vector2(440, 360);
                        blinky.Position = new Vector2(480, 360);
                        Inky.Position = new Vector2(520, 360);
                    }
                    else if(pac.HitBox.Contains(ghost.HitBox))
                    {
                        ghost.Position = ghost.StartingPosition;
                    }
                }
            }

            for (int i = 0; i < PowerUps.Count; i++)
            {
                if (PowerUps[i].HitBox.Contains(pac.HitBox))
                {
                    //able to kill
                    pac.elapsedPowerTime = TimeSpan.Zero;
                    pac.IsPowerActivated = true;
                    PowerUps.RemoveAt(i);
                    i--;
                 }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            
            for (int i = 0; i <= GraphicsDevice.Viewport.Width / pac.Texture.Width; i++)
            {
                spriteBatch.Draw(pixel, new Rectangle(i * 40, 0, 1, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.Draw(pixel, new Rectangle(0, i * 40, GraphicsDevice.Viewport.Width, 1), Color.White);
            }

            //get current vertex the mouse is at and its neighbors
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

            foreach (var fud in food)
            {
                fud.Draw(spriteBatch);
            }

            foreach (var wall in Walls)
            {
                wall.Draw(spriteBatch);
            }

            foreach (var heart in Hearts)
            {
                heart.Draw(spriteBatch);
            }

            foreach (var power in PowerUps)
            {
                power.Draw(spriteBatch);
            }

            pac.Draw(spriteBatch);
            blinky.Draw(spriteBatch);
            pinky.Draw(spriteBatch);
            Clyde.Draw(spriteBatch);
            Inky.Draw(spriteBatch);

            if (pac.IsPowerActivated)
            {
                if (pac.elapsedPowerTime >= TimeSpan.FromSeconds(8))
                {
                    spriteBatch.Draw(pixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.Red * 0.5f);
                }
                else
                {
                    spriteBatch.Draw(pixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), screenTint);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
