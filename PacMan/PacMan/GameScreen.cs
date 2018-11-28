using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MichaelLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PacMan
{
    public class GameScreen : Screen
    {
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

        List<BaseGameSprite> Hearts = new List<BaseGameSprite>();

        List<BaseGameSprite> PowerUps = new List<BaseGameSprite>();
        
        bool shouldStart = false;

        int originalFoodCount = 0;

        public GameScreen(GraphicsDevice graphics, ContentManager content) : base(graphics, content)
        {
            //This is because we are using the same pacman in the skinscreen
            //and some of the variables were changed
            #region PacManReInitizilation
            pac.Position = new Vector2(120, 40);
            pac.Scale = Vector2.One;
            pac.TimePerFrame = TimeSpan.FromMilliseconds(75);
            #endregion

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

            pixel = new Texture2D(graphics, 1, 1);
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


            pinky = new Pinky(pinkyTexture, new Vector2(400, 360), Color.White, Vector2.One, Map);
            Clyde = new Clyde(clydeTexture, new Vector2(440, 360), Color.White, Vector2.One, Map);
            blinky = new Blinky(blinkyTexture, new Vector2(480, 360), Color.White, Vector2.One, Map);
            Inky = new Inky(inkyTexture, new Vector2(520, 360), Color.White, Vector2.One, InkyMap);

            for (int i = 0; i < food.Count; i++)
            {
                bool wasRemoved = false;
                for (int j = 0; j < PowerUps.Count; j++)
                {
                    if (food[i].HitBox.Contains(PowerUps[j].Position))
                    {
                        food.RemoveAt(i);
                        wasRemoved = true;
                    }
                }
                if (wasRemoved) i--;
            }

            Ghosts = new List<BaseGameSprite>
            {
                Inky,
                blinky,
                Clyde,
                pinky
            };

            originalFoodCount = food.Count;
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < food.Count; i++)
            {
                if (food[i].HitBox.Contains(pac.Position))
                {
                    food.RemoveAt(i);
                    i--;
                }
            }

            if (food.Count == 0)
            {
                Game1.CurrentUserData.Money += 100 * Hearts.Count;
                Game1.CurrentState = States.End;
                Game1.DidWin = true;
                Game1.Screens.Add(States.End, new EndScreen(Graphics, Content));
            }

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

                pac.Update(gameTime, Walls, Graphics);
                //blinky.Update(gameTime);
             //   pinky.Update(gameTime);
           //     Clyde.Update(gameTime);
                //Inky.Update(gameTime);


                foreach (var ghost in Ghosts)
                {
                    if (pac.HitBox.Contains(ghost.HitBox) && !pac.IsPowerActivated)
                    {
                        
                        Hearts.RemoveAt(Hearts.Count - 1);
                        if (Hearts.Count == 0)
                        {
                            Game1.CurrentUserData.Money += (int)(((float)food.Count / originalFoodCount) * 100);
                            Game1.CurrentState = States.End;
                            Game1.DidWin = false;
                            Game1.Screens.Add(States.End, new EndScreen(Graphics, Content));
                        }

                        shouldStart = false;

                        pac.Position = new Vector2(120, 40);
                        pinky.Position = new Vector2(400, 360);
                        Clyde.Position = new Vector2(440, 360);
                        blinky.Position = new Vector2(480, 360);
                        Inky.Position = new Vector2(520, 360);
                    }
                    else if (pac.HitBox.Contains(ghost.HitBox))
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            pac.Draw(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin();

            for (int i = 0; i <= Graphics.Viewport.Width / pac.Texture.Width; i++)
            {
                spriteBatch.Draw(pixel, new Rectangle(i * 40, 0, 1, Graphics.Viewport.Height), Color.White);
                spriteBatch.Draw(pixel, new Rectangle(0, i * 40, Graphics.Viewport.Width, 1), Color.White);
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

            blinky.Draw(spriteBatch);
            pinky.Draw(spriteBatch);
            Clyde.Draw(spriteBatch);
            Inky.Draw(spriteBatch);

            if (pac.IsPowerActivated)
            {
                if (pac.elapsedPowerTime >= pac.PowerTime - TimeSpan.FromSeconds(2))
                {
                    spriteBatch.Draw(pixel, new Rectangle(0, 0, Graphics.Viewport.Width, Graphics.Viewport.Height), Color.Red * 0.5f);
                }
                else
                {
                    spriteBatch.Draw(pixel, new Rectangle(0, 0, Graphics.Viewport.Width, Graphics.Viewport.Height), Color.Gray * 0.5f);
                }
            }

            base.Draw(spriteBatch);
        }
    }
}
