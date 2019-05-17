using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Pause
{
    interface Option
    {
        int id
        {
            get; 
        }

        void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// True means a setting was changed, false means nothing happened. 
        /// </summary>
        bool Update(MouseState ms, MouseState lastMs); 
    }
}
