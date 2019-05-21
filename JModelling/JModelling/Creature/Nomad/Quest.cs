using JModelling.JModelling;
using JModelling.JModelling.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Creature.Nomad
{
    public class Quest
    {
        private static Random random; 
        public static Player player;
        public static ChunkGenerator cg; 

        public QuestType type;

        public string acceptString; 

        public List<Creature> targets;

        public Vec4 loc;

        public bool finished; 

        public Quest()
        {   
            Vec4 cam = player.Camera.loc;
            
            float x = (random.Next(0, 2) == 0) ? random.Next(-300, -100) : random.Next(100, 300);
            float z = (random.Next(0, 2) == 0) ? random.Next(-300, -100) : random.Next(100, 300);

            loc = new Vec4(x, cg.GetHeightAt(x, z), z);
            switch (random.Next(0, (int)QuestType.Size))
            {
                case ((int)QuestType.Kill):
                    GenerateEnemies(); 
                    break; 
            }

            finished = false; 
        }
        
        public static void Init(Player player, ChunkGenerator cg)
        {
            random = new Random();
            Quest.player = player;
            Quest.cg = cg; 
        } 

        public void GenerateEnemies()
        {
            int size = random.Next(2, 5); 
            // Create a few monsters around the location. 
            targets = new List<Creature>();

            int type = random.Next(0, (int)MonsterType.Size);
            string typeStr = "";
            switch(type)
            {
                case (int)MonsterType.Zombie:
                    typeStr = "Zombies";
                    break; 
            }

            for (int index = 0; index < size; index++)
            {
                int x = random.Next(-50 + (int)loc.X, 50 + (int)loc.X),
                    z = random.Next(-50 + (int)loc.Z, 50 + (int)loc.Z);
                
                switch (type)
                {
                    case (int)MonsterType.Zombie: 
                        targets.Add(new Zombie(new Vec4(x, cg.GetHeightAt(x, z), z)));
                        break; 
                }
            }

            acceptString = "Could you kill " + size + " " + typeStr + " for me?"; 
        }

        public bool isFinished()
        {
            switch (type)
            {
                case QuestType.Kill:
                    if (targets.Count == 0)
                    {
                        return true; 
                    }
                    return false;                 
            }

            return false;            
        }
    }
}
