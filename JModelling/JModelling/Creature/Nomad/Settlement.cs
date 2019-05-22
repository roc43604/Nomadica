using JModelling.JModelling;
using JModelling.JModelling.Chunk;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Creature.Nomad
{
    public class Settlement
    {
        private static ChunkGenerator cg;

        private static List<Settlement> settlements; 

        public List<NPC> group;
        private static Dialogue[] dialogues;

        public Vec4 loc; 

        public Settlement(Vec4 loc, Color color)
        {
            this.loc = loc; 
            group = new List<NPC>();

            Random random = new Random(); 
            for (int k = random.Next(3, 6); k >= 0; k--)
            {
                float x = loc.X + random.Next(60, 200);
                float z = loc.Z + random.Next(60, 200);
                Vec4 newloc = new Vec4(x, cg.GetHeightAt(x, z), z);

                NPC npc = new NPC(newloc, null);
                npc.Mesh.SetColor(color); 
                 
                group.Add(npc); 
            }

            settlements.Add(this); 
        }

        public static void SetQuest(Quest quest)
        {
            dialogues = new Dialogue[]
            {
                new Dialogue(
                    new string[]
                    {
                        "Hello.",
                        quest.acceptString,
                        "Thanks. Please come back here \nwhen it's completed.",
                        "Alright, let me know if you \nchange your mind. "
                    },
                    1, 2, 3,
                    new int[]
                    {
                        2, 3
                    }
                ),
                new Dialogue(
                    new string[]
                    {
                        "Come back once the job is finished. "
                    },
                    -1, -1, -1,
                    new int[]
                    {
                        0
                    }
                ),
                new Dialogue(
                    new string[]
                    {
                        "Thanks for dealing with that for us.",
                        "Here is an item as a reward."
                    },
                    -1, -1, -1,
                    new int[]
                    {
                        1
                    }
                )
            };

            foreach (Settlement settlement in settlements)
            {
                foreach (NPC npc in settlement.group)
                {
                    npc.Dialogues = dialogues;
                    npc.index = 0; 
                }
            }
        }

        public static void Init(ChunkGenerator cg)
        {
            settlements = new List<Settlement>(); 
            Settlement.cg = cg; 
        }
    }
}
