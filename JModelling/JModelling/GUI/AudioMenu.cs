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
    /// A subset of the pause menu. Shows general settings, like the quit button. 
    /// </summary>
    public class AudioMenu : PauseMenuSubset
    {
        Option[] questions;

        public string name
        {
            get
            {
                return "Audio";
            }
        }

        public AudioMenu(Rectangle menuBounds)
        {
            Rectangle rec = new Rectangle(
                menuBounds.X + 10,
                menuBounds.Y + 10,
                menuBounds.Width - 20,
                menuBounds.Height / 5);

            string[] choices = new string[100]; 
            for (int k = 0; k < choices.Length; k++)
            {
                choices[k] = "";
            }

            questions = new Option[]
            {
                new MultipleChoiceOption(this, 0, "Volume", choices, rec)
            };
        }

        public void Update(MouseState ms, MouseState lastMs)
        {
            foreach (Option option in questions)
            {
                if (option.Update(ms, lastMs))
                {

                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Option option in questions)
            {
                option.Draw(spriteBatch);
            }
        }

        public void ButtonPressed(int id)
        {
            throw new NotImplementedException();
        }
    }
}
