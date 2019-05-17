using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Pause
{
    /// <summary>
    /// Some text with a button next to it. 
    /// </summary>
    public class Button : Option
    {
        /// <summary>
        /// The thing that uses this option. 
        /// </summary>
        private PauseMenuSubset parent;

        int identifier; 
        public int id
        {
            get
            {
                return identifier; 
            }
        }

        /// <summary>
        /// The text referring to the button. 
        /// </summary>
        private string text;

        /// <summary>
        /// The choice referring to the text. 
        /// </summary>
        private string choice;

        /// <summary>
        /// Where the text will be displayed. 
        /// </summary>
        private Vector2 textLocation;

        /// <summary>
        /// The location of where the choice will be displayed. 
        /// </summary>
        private Rectangle choiceBoxLoc;
        private Vector2 choiceStringLoc; 

        /// <summary>
        /// Whether or not this button has been clicked. 
        /// </summary>
        public bool chosen;

        private Rectangle displayArea;         

        public Button(PauseMenuSubset parent, int identifier, string text, string choice, Rectangle displayArea)
        {
            this.parent = parent;
            this.identifier = identifier; 
            this.text = text;
            this.choice = choice;
            this.displayArea = displayArea;

            ConfigureBounds(displayArea);

            chosen = false; 
        }

        private void ConfigureBounds(Rectangle displayArea)
        {
            textLocation = new Vector2(
                displayArea.X + 10,
                displayArea.Y + displayArea.Height / 2 - PauseMenu.questionFont.MeasureString(text).Y / 2
            );

            int x = (int)(textLocation.X + PauseMenu.questionFont.MeasureString(text).X + 10);
            choiceBoxLoc = new Rectangle(
                x,
                displayArea.Y + 10,
                displayArea.Width - (x - displayArea.X),
                displayArea.Height - 20);

            choiceStringLoc = new Vector2(
                choiceBoxLoc.X + choiceBoxLoc.Width / 2 - PauseMenu.answerFont.MeasureString(choice).X / 2,
                choiceBoxLoc.Y + choiceBoxLoc.Height / 2 - PauseMenu.answerFont.MeasureString(choice).Y / 2);
        }

        public bool Update(MouseState ms, MouseState lastMs)
        {
            if (ms.LeftButton == ButtonState.Pressed && lastMs.LeftButton == ButtonState.Released)
            {
                if (choiceBoxLoc.Contains(ms.X, ms.Y))
                {
                    chosen = !chosen;
                    return true; 
                }
            }
            return false; 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(PauseMenu.WhiteTexture, displayArea, new Color(40, 40, 40, 150));
            spriteBatch.DrawString(PauseMenu.questionFont, text, textLocation, Color.White);
            if (chosen)
            {
                spriteBatch.Draw(PauseMenu.ThinFilledRoundBox, choiceBoxLoc, new Color(200, 200, 40, 200));
            }
            else
            {
                spriteBatch.Draw(PauseMenu.ThinFilledRoundBox, choiceBoxLoc, new Color(40, 40, 40, 200));
            }
            spriteBatch.DrawString(PauseMenu.answerFont, choice, choiceStringLoc, Color.White); 
        }
    }
}
