using JModelling.Creature.Nomad;
using JModelling.InventorySpace;
using JModelling.JModelling;
using JModelling.JModelling.Chunk;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Creature
{
    /// <summary>
    /// A non-playable character. The player can talk to them and they will
    /// offer responses. 
    /// </summary>
    public class NPC : Creature, TalkingCreature
    {
        private const string meshName = @"Content/Models/cube.obj";
        private const int height = 20;
        private const float speed = 1.5f;

        public static JManager parent;

        public int index; 
        public int Index
        {
            get
            {
                return index; 
            }
            set
            {
                index = value;

                foreach (Settlement settlement in parent.settlements)
                {
                    foreach (NPC npc in settlement.group)
                    {
                        npc.index = index; 
                    }
                }

                if (index == 1)
                {
                    parent.compass = new GUI.Compass(parent.player.quest.targets[0].Loc);
                    parent.player.quest.StartQuest();
                }
            }
        }

        private Dialogue[] dialogues;
        public Dialogue[] Dialogues
        {
            get
            {
                return dialogues; 
            }
            set
            {
                dialogues = value; 
            }
        }

        private static Mesh mesh;

        public NPC(Vec4 loc, Dialogue[] dialogues)
            : base(mesh, loc, speed, 0, 100, 100, new List<Item>(new Item[] { }), MonsterType.None)
        {
            this.dialogues = dialogues; 
        }

        public static void Init(ContentManager content)
        {
            mesh = Load.Mesh(meshName, 25, 0, 0, 0);
        }

        /// <summary>
        /// If the player talks to this NPC, show a dialogue box. 
        /// </summary>
        /// <param name="kb"></param>
        public bool Talk(Player player, KeyboardState kb)
        {    
            if (kb.IsKeyDown(Controls.Interact) && MathExtensions.Dist(player.Camera.loc, Loc) < 50)
            {
                return true; 
            }

            return false; 
        }

        public override void Update(Player player)
        {
            float altitude = cg.GetHeightAt(Loc.X, Loc.Z) + height; 
            if (Loc.Y < altitude)
            {
                Loc.Y = altitude; 
            }
        }
    }
}
