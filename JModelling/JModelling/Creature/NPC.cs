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

        private string[] text; 
        public string[] Text
        {
            get
            {
                return text; 
            }
        }

        private int responseIndex; 
        public int ResponseIndex
        {
            get
            {
                return responseIndex; 
            }
        }

        private int acceptIndex; 
        public int AcceptIndex
        {
            get
            {
                return acceptIndex; 
            }
        }

        private int denyIndex; 
        public int DenyIndex
        {
            get
            {
                return denyIndex; 
            }
        }

        private HashSet<int> quitIndices; 
        public HashSet<int> QuitIndices
        {
            get
            {
                return quitIndices;
            }
        }

        private static Mesh mesh;

        public static JManager parent; 

        public NPC(Vec4 loc, string[] text, int responseIndex, int acceptIndex, int denyIndex, int[] quitIndices)
            : base(mesh, loc, speed, 0, 100, 100, new List<Item>(new Item[] { }))
        {
            this.text = text;
            this.responseIndex = responseIndex;
            this.acceptIndex = acceptIndex;
            this.denyIndex = denyIndex;
            this.quitIndices = new HashSet<int>(quitIndices); 
        }

        public static void Init(ContentManager content)
        {
            mesh = Load.Mesh(meshName, 25, 0, 0, 0);
        }

        /// <summary>
        /// If the player talks to this NPC, show a dialogue box. 
        /// </summary>
        /// <param name="kb"></param>
        public Quest Talk(Player player, KeyboardState kb)
        {    
            if (kb.IsKeyDown(Controls.Interact) && MathExtensions.Dist(player.Camera.loc, Loc) < 50)
            {
                return new Quest(); 
            }

            return null; 
        }

        public override void Update(Player player, ChunkGenerator cg)
        {
            float altitude = cg.GetHeightAt(Loc.X, Loc.Z) + height; 
            if (Loc.Y < altitude)
            {
                Loc.Y = altitude; 
            }
        }
    }
}
