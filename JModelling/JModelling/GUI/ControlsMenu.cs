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
    public class ControlsMenu : PauseMenuSubset
    {
        Option[] questions;

        public string name
        {
            get
            {
                return "Controls";
            }
        }

        public ControlsMenu(Rectangle menuBounds)
        {
            Rectangle rec = new Rectangle(
                menuBounds.X + 10,
                menuBounds.Y + 10,
                menuBounds.Width - 20,
                menuBounds.Height / 8
            );

            Rectangle two = new Rectangle(
                rec.X,
                rec.Y + rec.Height + 10,
                rec.Width,
                rec.Height
            );

            Rectangle thr = new Rectangle(
                two.X,
                two.Y + two.Height + 10,
                two.Width,
                two.Height
            );

            Rectangle fou = new Rectangle(
                thr.X,
                thr.Y + thr.Height + 10,
                thr.Width,
                thr.Height
            );

            Rectangle fiv = new Rectangle(
                fou.X,
                fou.Y + fou.Height + 10,
                fou.Width,
                fou.Height
            );

            Rectangle six = new Rectangle(
                fiv.X,
                fiv.Y + fiv.Height + 10,
                fiv.Width,
                fiv.Height
            );

            Rectangle sev = new Rectangle(
                six.X,
                six.Y + six.Height + 10,
                six.Width,
                six.Height
            );

            Rectangle eig = new Rectangle(
                sev.X,
                sev.Y + sev.Height + 10,
                sev.Width,
                sev.Height
            );

            questions = new Option[]
            {
                new Button(this, 1, "Foward                      ", "W", rec),
                new Button(this, 1, "Backwards                   ", "S", two),
                new Button(this, 1, "Strafe Left                 ", "A", thr),
                new Button(this, 1, "Strafe Right                ", "D", fou),
                new Button(this, 1, "Jump                        ", "Spacebar", fiv),
                new Button(this, 1, "Open Menu                   ", "Escape", six),
                new Button(this, 1, "Check Inventory             ", "Tab", sev)
            };
        }

        public void Update(MouseState ms, MouseState lastMs)
        {
            foreach (Option option in questions)
            {
                if (option.Update(ms, lastMs))
                {
                    if (option.id == 1) // Quit game
                    {
                        System.Environment.Exit(0);
                    }
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
