using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Pause
{
    /// <summary>
    /// A menu the player can configure settings in/quit the game from. 
    /// </summary>
    public class PauseMenu
    {
        /// <summary>
        /// Used for drawing backgrounds/borders. 
        /// </summary>
        public static Texture2D WhiteTexture, RoundBox, ThinRoundBox, FilledRoundBox, ThinFilledRoundBox;

        /// <summary>
        /// For general text-drawing to the screen. 
        /// </summary>
        public static SpriteFont font, questionFont, answerFont; 

        /// <summary>
        /// The image behind the menu. 
        /// </summary>
        private Texture2D world, sky;        

        /// <summary>
        /// The bounds of the entire window. 
        /// </summary>
        private Rectangle windowBounds; 

        /// <summary>
        /// The area the menu is located in. Outside of this area is a darkened version of the
        /// last thing the player saw.
        /// </summary>
        private Rectangle menuArea;

        /// <summary>
        /// The area each of the subsets are located in. 
        /// </summary>
        private Rectangle subsetArea; 

        private PauseMenuSubset[] subsets;
        private int currentSubset; 

        /// <summary>
        /// The area you can select the type of menu you're searching for (general, audio, video). 
        /// </summary>
        private Rectangle menuTypeSelectionBoxArea;

        /// <summary>
        /// A single menu type selection box. There will be multiple in the above area. 
        /// </summary>
        private Rectangle menuTypeSelectionBox; 

        public PauseMenu(int screenWidth, int screenHeight)
        {
            ConfigureScreen(screenWidth, screenHeight); 

            Console.WriteLine(menuArea);
            subsets = new PauseMenuSubset[]
            {
                new GeneralMenu(subsetArea),
                new VideoMenu(subsetArea),
                new GeneralMenu(subsetArea),
                new GeneralMenu(subsetArea)
            };
            currentSubset = 0; 
        }

        /// <summary>
        /// Sets the rectangles/click-areas of everything shown in the screen. 
        /// </summary>
        private void ConfigureScreen(int screenWidth, int screenHeight)
        {
            windowBounds = new Rectangle(0, 0, screenWidth, screenHeight);

            menuArea = new Rectangle(
                screenWidth / 10,
                screenHeight / 10,
                screenWidth - screenWidth / 5,
                screenHeight - screenHeight / 5
            );

            menuTypeSelectionBoxArea = new Rectangle(
                menuArea.X,
                menuArea.Y,
                menuArea.Width,
                menuArea.Height / 5);

            menuTypeSelectionBox = new Rectangle(0, 0, menuTypeSelectionBoxArea.Width / 4, menuTypeSelectionBoxArea.Height);

            subsetArea = new Rectangle(
                menuTypeSelectionBoxArea.X,
                menuTypeSelectionBoxArea.Y + menuTypeSelectionBoxArea.Height,
                menuTypeSelectionBoxArea.Width,
                menuArea.Height - menuTypeSelectionBoxArea.Height);
        }

        public void Create(Texture2D world, Texture2D sky)
        {
            this.world = world;
            this.sky = sky; 
        }

        public static void LoadImages(ContentManager content)
        {
            WhiteTexture = content.Load<Texture2D>("Images/Menu/white");
            RoundBox = content.Load<Texture2D>("Images/Menu/round box");
            ThinRoundBox = content.Load<Texture2D>("Images/Menu/ThinRoundBox");
            FilledRoundBox = content.Load<Texture2D>("Images/Menu/filled round box");
            ThinFilledRoundBox = content.Load<Texture2D>("Images/Menu/ThinFilledRoundBox"); 
            font = content.Load<SpriteFont>("PauseFont");
            questionFont = content.Load<SpriteFont>("QuestionFont");
            answerFont = content.Load<SpriteFont>("AnswerFont"); 
        }

        public void Update(MouseState ms, MouseState lastMs)
        {
            if (ms.LeftButton == ButtonState.Pressed && lastMs.LeftButton == ButtonState.Released)
            {
                int index = 0; 
                for (int x = menuTypeSelectionBoxArea.X; x < menuTypeSelectionBoxArea.X + menuTypeSelectionBoxArea.Width; x += menuTypeSelectionBox.Width)
                {
                    Rectangle rec = new Rectangle(x, menuTypeSelectionBoxArea.Y, menuTypeSelectionBox.Width, menuTypeSelectionBox.Height);
                    
                    if (rec.Contains(ms.X, ms.Y))
                    {
                        Console.WriteLine("BOOM!"); 
                        currentSubset = index;
                        break; 
                    }
                    index++; 
                }
                subsets[currentSubset].Update(ms, lastMs);

                Console.WriteLine(ms.X + " " + ms.Y); 
            }
        }

        /// <summary>
        /// Draws this menu to the screen. 
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Background
            spriteBatch.Draw(sky, windowBounds, Color.Gray);
            spriteBatch.Draw(world, windowBounds, Color.Gray);

            Color color = new Color(0, 0, 0, 25); 
            spriteBatch.Draw(WhiteTexture, menuArea, color);

            int index = 0;
            for (int x = menuTypeSelectionBoxArea.X; x < menuTypeSelectionBoxArea.X + menuTypeSelectionBoxArea.Width; x += menuTypeSelectionBox.Width)
            {
                Rectangle rec = new Rectangle(x, menuTypeSelectionBoxArea.Y, menuTypeSelectionBox.Width, menuTypeSelectionBox.Height);
                if (index == currentSubset)
                {
                    spriteBatch.Draw(FilledRoundBox, rec, Color.LightGray);
                }
                else
                {
                    spriteBatch.Draw(FilledRoundBox, rec, Color.Gray);
                }
                
                Vector2 fontLoc = new Vector2(
                    rec.X + rec.Width / 2 - (int)font.MeasureString(subsets[index].name).X / 2,
                    rec.Y + rec.Height / 2 - (int)font.MeasureString(subsets[index].name).Y / 2
                ); 
                spriteBatch.DrawString(font, subsets[index].name, fontLoc, Color.White);
                index++;
            }

            subsets[currentSubset].Draw(spriteBatch); 
        }
    }
}
