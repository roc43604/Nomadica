using JModelling.JModelling;
using JModelling.Pause;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Creature
{
    /// <summary>
    /// A box for displaying dialogue. 
    /// </summary>
    public class DialogueBox
    {
        private static Rectangle windowBounds, boxBounds, acceptBoxLoc, denyBoxLoc;
        private static Vector2 acceptTextLoc, denyTextLoc; 

        private Texture2D world, sky;
        private TalkingCreature source;
        private int index;

        private Vector2 textLoc; 

        public DialogueBox(Texture2D world, Texture2D sky, TalkingCreature source)
        {
            this.world = world;
            this.sky = sky; 
            this.source = source;
            index = 0;

            textLoc = new Vector2(
                boxBounds.X + 30, 
                boxBounds.Y + boxBounds.Height / 2 - PauseMenu.questionFont.MeasureString(source.Text[index]).Y / 2
            );
        }

        public static void Init(int screenWidth, int screenHeight)
        {
            ConfigureScreen(screenWidth, screenHeight);            
        }

        private static void ConfigureScreen(int screenWidth, int screenHeight)
        {
            windowBounds = new Rectangle(0, 0, screenWidth, screenHeight); 

            boxBounds = new Rectangle(
                screenWidth / 5,
                (int)(screenHeight / 5f * 3.5f),
                screenWidth / 5 * 3,
                screenHeight / 5
            );

            int width = boxBounds.Width / 3;
            int height = boxBounds.Height / 5;
            acceptBoxLoc = new Rectangle(
                boxBounds.X + width / 2 - width / 4,
                boxBounds.Y + boxBounds.Height - height - height / 2,
                width,
                height 
            );

            denyBoxLoc = new Rectangle(acceptBoxLoc.X + acceptBoxLoc.Width + width / 2, acceptBoxLoc.Y, acceptBoxLoc.Width, acceptBoxLoc.Height);

            acceptTextLoc = new Vector2(
                acceptBoxLoc.X + acceptBoxLoc.Width / 2 - PauseMenu.questionFont.MeasureString("Accept").X / 2,
                acceptBoxLoc.Y + acceptBoxLoc.Height / 2 - PauseMenu.questionFont.MeasureString("Accept").Y / 2
            );

            denyTextLoc = new Vector2(
                denyBoxLoc.X + denyBoxLoc.Width / 2 - PauseMenu.questionFont.MeasureString("deny").X / 2,
                denyBoxLoc.Y + denyBoxLoc.Height / 2 - PauseMenu.questionFont.MeasureString("deny").Y / 2
            );
        }

        public bool Update(KeyboardState kb, KeyboardState lastKb, MouseState ms, MouseState lastMs)
        {
            if (index != source.ResponseIndex)
            {
                if ((kb.IsKeyDown(Controls.Interact) && lastKb.IsKeyUp(Controls.Interact)) ||
                (ms.LeftButton == ButtonState.Pressed && lastMs.LeftButton == ButtonState.Released))
                {
                    if (source.QuitIndices.Contains(index))
                    {
                        return true;
                    }

                    index++;

                    textLoc = new Vector2(
                        boxBounds.X + 30,
                        boxBounds.Y + boxBounds.Height / 2 - PauseMenu.questionFont.MeasureString(source.Text[index]).Y / 2
                    );
                }
            }
            else
            {
                if (index == source.ResponseIndex && ms.LeftButton == ButtonState.Pressed && lastMs.LeftButton == ButtonState.Released)
                {
                    if (acceptBoxLoc.Contains(ms.X, ms.Y))
                    {
                        index = source.AcceptIndex;
                    }
                    else if (denyBoxLoc.Contains(ms.X, ms.Y))
                    {
                        index = source.DenyIndex;
                    }
                }
            }

            return false; 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Background
            spriteBatch.Draw(sky, windowBounds, Color.White);
            spriteBatch.Draw(world, windowBounds, Color.White);
            
            spriteBatch.Draw(PauseMenu.FilledRoundBox, boxBounds, new Color(0, 0, 0, 180));
            spriteBatch.DrawString(PauseMenu.questionFont, source.Text[index], textLoc, Color.White); 
            
            if (index != source.ResponseIndex)
            {
                spriteBatch.DrawString(PauseMenu.reminderFont, "Press E to progress...",
                    new Vector2(textLoc.X, boxBounds.Y + boxBounds.Height - PauseMenu.reminderFont.MeasureString("Press E to progress...").Y), Color.White); 
            }
            else
            {
                spriteBatch.Draw(PauseMenu.FilledRoundBox, acceptBoxLoc, Color.Green);
                spriteBatch.DrawString(PauseMenu.questionFont, "Accept", acceptTextLoc, Color.Black); 
                spriteBatch.Draw(PauseMenu.FilledRoundBox, denyBoxLoc, Color.Green);
                spriteBatch.DrawString(PauseMenu.questionFont, "Deny", denyTextLoc, Color.Black); 
            }
        }
    }
}
