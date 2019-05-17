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
        private static Rectangle windowBounds, boxBounds;

        private Texture2D world, sky;
        private TalkingCreature source;
        private int index; 

        public DialogueBox(Texture2D world, Texture2D sky, TalkingCreature source)
        {
            this.world = world;
            this.sky = sky; 
            this.source = source;
            index = 0; 
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
        }

        public bool Update(KeyboardState kb, KeyboardState lastKb)
        {


            return false; 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Background
            spriteBatch.Draw(sky, windowBounds, Color.White);
            spriteBatch.Draw(world, windowBounds, Color.White);

            Console.WriteLine(boxBounds); 
            spriteBatch.Draw(PauseMenu.FilledRoundBox, boxBounds, new Color(0, 0, 0, 180)); 
        }
    }
}
