using MichaelLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class MakeMapScreen : Screen
    {
        Sprite Grid;
        Texture2D pixel;


        List<Button> Buttons;
        
        SpriteFont font;

        bool checkForGrid = false;
        Button currButtonClicked = null;

        Dictionary<Color, int> OneTimeUse;

        public MakeMapScreen(GraphicsDevice graphics, ContentManager content) : base(graphics, content)
        {
            pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData(new[] { Color.White });

            Buttons = new List<Button>();
            OneTimeUse = new Dictionary<Color, int>();

            font = Content.Load<SpriteFont>("Font");

            var gridTexture = content.Load<Texture2D>("LevelGrid");

            Grid = new Sprite(gridTexture, new Vector2(500, 400), Color.White, Vector2.One * 16, null);
            //Grid.Origin = new Vector2(gridTexture.Width/2, gridTexture.Height/2);
            
            var blank = Content.Load<Texture2D>("white1x1");
            
            var wallButton = new Button(blank, new Vector2(100, 50), Color.Black, Grid.Scale, null);

            var invalidFoodButton = new Button(blank, new Vector2(100, 100), Color.Green, Grid.Scale, null);

            var eraserButton = new Button(blank, new Vector2(100, 150), Color.White, Grid.Scale, null);

            var powerUpButton = new Button(blank, new Vector2(100, 200), Color.Gray, Grid.Scale, null);

            var pacmanButton = new Button(blank, new Vector2(100, 250), Color.Yellow, Grid.Scale, null);

            var inkyButton = new Button(blank, new Vector2(100, 300), Color.Blue, Grid.Scale, null);

            var blinkyButton = new Button(blank, new Vector2(100, 350), Color.Red, Grid.Scale, null);

            var pinkyButton = new Button(blank, new Vector2(100, 400), Color.Pink, Grid.Scale, null);

            var orangeButton = new Button(blank, new Vector2(100, 450), Color.Orange, Grid.Scale, null);

            Buttons.Add(wallButton);
            Buttons.Add(invalidFoodButton);
            Buttons.Add(eraserButton);
            Buttons.Add(powerUpButton);
            Buttons.Add(pacmanButton);
            Buttons.Add(inkyButton);
            Buttons.Add(blinkyButton);
            Buttons.Add(pinkyButton);
            Buttons.Add(orangeButton);
            
            OneTimeUse.Add(Color.Yellow, 0);
            OneTimeUse.Add(Color.Red, 0);
            OneTimeUse.Add(Color.Blue, 0);
            OneTimeUse.Add(Color.Orange, 0);
            OneTimeUse.Add(Color.Pink, 0);
        }
        
        public (int, int) GetGridCellPosition()
        {
            var artificalX = Game1.Mouse.X - (Grid.Position.X - Grid.ScaledWidth / 2);
            var artificalY = Game1.Mouse.Y - (Grid.Position.Y - Grid.ScaledHeight / 2);

            if(artificalX < 0 || artificalY < 0)
            {
                return (-1, -1);
            }

            var posX = (int)(artificalX / Grid.Scale.X);
            var posY = (int)(artificalY / Grid.Scale.Y);
            
            return (posX, posY);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var button in Buttons)
            {
                if(button.IsClicked(Game1.Mouse) && !button.IsClicked(Game1.OldMouse))
                {
                    currButtonClicked = button;   
                }
            }

           if(currButtonClicked != null)
           {
                checkForGrid = true;
            }
            
            if(checkForGrid)
            {
                if(OneTimeUse.ContainsKey(currButtonClicked.Color))
                {
                    if(OneTimeUse[currButtonClicked.Color] == 1)
                    {
                        return;
                    }
                }

                if (Grid.HitBox.Contains(Game1.Mouse.Position) && Game1.Mouse.LeftButton == ButtonState.Pressed)
                {
                    var point = GetGridCellPosition();

                    var colorarray = new Color[Grid.Texture.Width * Grid.Texture.Height];
                    Grid.Texture.GetData(colorarray);

                    for (int i = 0; i < colorarray.Length; i++)
                    {
                        //convert from 2d to 1d
                        if (i % Grid.Texture.Width == point.Item1 && i / Grid.Texture.Width == point.Item2)
                        {
                            //if it is a onetimeuse color being changed to white
                            //then we need to decrease the count in the dictionary
                            if(OneTimeUse.ContainsKey(colorarray[i]) && currButtonClicked.Color == Color.White)
                            {
                                OneTimeUse[colorarray[i]]--;
                            }
                            colorarray[i] = currButtonClicked.Color;
                            if(OneTimeUse.ContainsKey(currButtonClicked.Color))
                            {
                                OneTimeUse[currButtonClicked.Color]++;
                            }
                        }
                    }

                    Grid.Texture.SetData(colorarray);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin();

            Grid.Draw(spriteBatch);
            foreach(var button in Buttons)
            {
                button.Draw(spriteBatch);
            }

            spriteBatch.DrawString(font, $"X:{GetGridCellPosition().Item1}, Y:{GetGridCellPosition().Item2}", new Vector2(200, 100), Color.Black);

            for(int i = 0; i <= Grid.ScaledWidth; i += (int)Grid.Scale.X)
            {
                spriteBatch.Draw(pixel, new Rectangle((int)(Grid.Position.X - Grid.ScaledWidth / 2) + i, (int)(Grid.Position.Y - Grid.ScaledHeight / 2), 1, (int)Grid.ScaledHeight), Color.Black);
            }
            for(int i = 0; i <= Grid.ScaledHeight; i+= (int)Grid.Scale.Y)
            {
                spriteBatch.Draw(pixel, new Rectangle((int)(Grid.Position.X - Grid.ScaledWidth / 2), (int)(Grid.Position.Y - Grid.ScaledHeight / 2) + i, (int)Grid.ScaledWidth, 1), Color.Black);
            }
            
            base.Draw(spriteBatch);
        }
    }
}
