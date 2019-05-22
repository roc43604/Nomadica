using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Creature
{
    /// <summary>
    /// A talking creature is a creature that can talk to the player. The player
    /// clicks the "Interact" button to talk to the creature, and the creature
    /// will give some text. Clicking "Interact" again will advance the text to
    /// the next part, and could show a few responses the player could give, like
    /// "Accept" or "Decline". 
    /// </summary>
    public interface TalkingCreature
    {
        int Index
        {
            get;
            set; 
        }

        Dialogue[] Dialogues
        {
            get;
            set; 
        }

        bool Talk(Player player, KeyboardState kb); 
    }
}
