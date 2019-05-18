using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Pause
{
    /// <summary>
    /// Shows the rules for what a pause menu subset should be. These subsets
    /// are things like "General", "Audio", "Video", etc. inside of the Pause
    /// Menu. 
    /// </summary>
    public interface PauseMenuSubset
    {
        string name
        {
            get;
        } 

        void Draw(SpriteBatch spriteBatch);

        void Update(MouseState ms, MouseState lastMs);
    }
}
