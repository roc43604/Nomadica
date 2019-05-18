using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using JModelling.JModelling;
using GraphicsEngine;
using JModelling.JModelling.Chunk;
using System.Diagnostics;
using JModelling.InventorySpace;

namespace JModelling.Creature.Nomad.ChiefQuestsHold
{
    class Quests
    {
        private int Reward;
        private int cnum;
        private string descript;
        private Texture2D giver;
        private bool complete;
        public Quests(int r, string d, Texture2D giver, int num)
        {
            Reward = r;
            descript = d;
            this.giver = giver;
            complete = false;
            cnum = num;
        }
        public void update(int check)
        {
            if (check == cnum)
            {
                complete = true;
            }
            if (complete)
            {

            }
        }
        public void Draw(SpriteBatch sb)
        {
        }
    }
}