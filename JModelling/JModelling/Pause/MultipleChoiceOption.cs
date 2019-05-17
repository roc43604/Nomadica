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
    /// Some text with a list of possible answers. 
    /// </summary>
    public class MultipleChoiceOption : Option
    {
        /// <summary>
        /// The thing using this option. 
        /// </summary>
        private PauseMenuSubset parent;

        private int identifier; 
        public int id
        {
            get
            {
                return identifier; 
            }
        }

        /// <summary>
        /// The question being asked. 
        /// </summary>
        private string text;

        /// <summary>
        /// The different answers the user could choose. 
        /// </summary>
        private string[] choices;

        /// <summary>
        /// Where the question will be displayed. 
        /// </summary>
        private Vector2 questionLocation;

        /// <summary>
        /// The x/y loc of where the first answer will be displayed. 
        /// </summary>
        private Vector2 answerStartLocation;

        /// <summary>
        /// The width/height of each answer. 
        /// </summary>
        private Vector2 answerSize; 

        /// <summary>
        /// The index of the answer that has been chosen. 
        /// </summary>
        public int ChosenIndex;

        private Rectangle displayArea; 

        /// <summary>
        /// Creates a new question with a list of possible choices. 
        /// </summary>
        /// <param name="parent">The screen implementing this question</param>
        /// <param name="text">The text/question to be asked</param>
        /// <param name="choices">The different choices the user could choose</param>
        /// <param name="displayArea">The area this option will take up</param>
        public MultipleChoiceOption(PauseMenuSubset parent, int identifier, string text, string[] choices, Rectangle displayArea)
        {
            this.parent = parent;
            this.identifier = identifier; 
            this.text = text;
            this.choices = choices;
            this.displayArea = displayArea; 

            ConfigureBounds(displayArea); 

            ChosenIndex = 0; 
        }

        private void ConfigureBounds(Rectangle displayArea)
        {
            questionLocation = new Vector2(displayArea.X + 10, displayArea.Y + displayArea.Height / 4 - PauseMenu.questionFont.MeasureString(text).Y / 2);
            answerStartLocation = new Vector2(displayArea.X, displayArea.Y + displayArea.Height / 2);
            answerSize = new Vector2(displayArea.Width / choices.Length, displayArea.Height / 2);  
        }

        public bool Update(MouseState ms, MouseState lastMs)
        {
            if (ms.LeftButton == ButtonState.Pressed && lastMs.LeftButton == ButtonState.Released)
            {
                for (int index = 0; index < choices.Length; index++)
                {
                    Rectangle answerBox = new Rectangle(
                        (int)(answerStartLocation.X + answerSize.X * index),
                        (int)(answerStartLocation.Y),
                        (int)(answerSize.X),
                        (int)(answerSize.Y)
                    );

                    if (answerBox.Contains(ms.X, ms.Y))
                    {
                        ChosenIndex = index;
                        return true;                         
                    }
                }
            }
            return false; 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(PauseMenu.WhiteTexture, displayArea, new Color(40, 40, 40, 150)); 
            spriteBatch.DrawString(PauseMenu.questionFont, text, questionLocation, Color.White); 
            for (int index = 0; index < choices.Length; index++)
            {
                Rectangle answerBox = new Rectangle(
                    (int)(answerStartLocation.X + answerSize.X * index),
                    (int)(answerStartLocation.Y),
                    (int)(answerSize.X),
                    (int)(answerSize.Y)
                );
                if (index == ChosenIndex)
                {
                    spriteBatch.Draw(PauseMenu.ThinFilledRoundBox, answerBox, new Color(200, 200, 40, 200));
                }
                else
                {
                    spriteBatch.Draw(PauseMenu.ThinFilledRoundBox, answerBox, new Color(40, 40, 40, 200));
                }
                spriteBatch.DrawString(
                    PauseMenu.answerFont, 
                    choices[index], 
                    new Vector2(
                        answerBox.X + answerSize.X / 2 - PauseMenu.answerFont.MeasureString(choices[index]).X / 2, 
                        answerBox.Y + answerSize.Y / 2 - PauseMenu.answerFont.MeasureString(choices[index]).Y / 2), 
                    Color.White
                ); 
            }
        }
    }
}
