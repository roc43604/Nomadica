﻿using Microsoft.Xna.Framework;
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
    public class VideoMenu : PauseMenuSubset
    {
        Option[] questions;

        public string name
        {
            get
            {
                return "Video";
            }
        }

        public VideoMenu(Rectangle menuBounds)
        {
            Rectangle rec = new Rectangle(
                menuBounds.X + 10,
                menuBounds.Y + 10,
                menuBounds.Width - 20,
                menuBounds.Height / 5);

            Rectangle two = new Rectangle(
                rec.X, rec.Y + rec.Height + 20, rec.Width, rec.Height);

            Rectangle three = new Rectangle(
                two.X, two.Y + two.Height + 20, two.Width, two.Height);

            questions = new Option[]
            {
                new MultipleChoiceOption(this, 0, "This is a test", new string[] {"One", "Two", "Three" }, rec),
                new MultipleChoiceOption(this, 1, "This is a test", new string[] {"One", "Two", "Three" }, two),
                new Button(this, 2, "This is a test", "Answer", three)
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