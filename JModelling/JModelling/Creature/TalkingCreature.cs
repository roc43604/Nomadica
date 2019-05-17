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
        /// <summary>
        /// The things the creature wants to say to the player. 
        /// </summary>
        string[] Text
        {
            get;
        }

        /// <summary>
        /// The index the player will accept or deny a quest from. 
        /// </summary>
        int ResponseIndex
        {
            get;
        } 

        /// <summary>
        /// The index the text will go to if the answer to the response
        /// was "accept". 
        /// </summary>
        int AcceptIndex
        {
            get;
        }

        /// <summary>
        /// The index the text will go to if the answer to the responce
        /// was "deny". 
        /// </summary>
        int DenyIndex
        {
            get;
        }

        bool Talk(Player player, KeyboardState kb); 
    }
}
