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
        private const int distance = 1000; 

        private static Random random; 
        public static Player player;
        public static ChunkGenerator cg;
        public static JManager manager; 

        public QuestType type;

        public string acceptString, finishString; 

        public List<Creature> targets;

        public bool finished;

        public int currentProgress, maxProgress; 

        public Quest()
        {   
            Vec4 cam = player.Camera.loc;

            double r = random.NextDouble() * (Math.PI * 2d);
            float x = (float)Math.Cos(r) * distance + cam.X,
                  z = (float)Math.Sin(r) * distance + cam.Z;

            Vec4 baseLoc = new Vec4(x, cg.GetHeightAt(x, z), z);
            switch (random.Next(0, (int)QuestType.Size))
            {
                case ((int)QuestType.Kill):
                    GenerateEnemies(baseLoc); 
                    break; 
            }

            currentProgress = 0;             

            finished = false; 
        }
        
        public static void Init(Player player, ChunkGenerator cg, JManager manager)
        {
            random = new Random();
            Quest.player = player;
            Quest.cg = cg;
            Quest.manager = manager; 
        } 

        public void StartQuest()
        {
            foreach (Creature target in targets)
            {
                manager.creatures.Add(target); 
            }
        }

        public void GenerateEnemies(Vec4 baseLoc)
        {
            maxProgress = random.Next(2, 5); 
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

            for (int index = 0; index < maxProgress; index++)
            {
                int x = random.Next(-50 + (int)baseLoc.X, 50 + (int)baseLoc.X),
                    z = random.Next(-50 + (int)baseLoc.Z, 50 + (int)baseLoc.Z);
                
                switch (type)
                {
                    case (int)MonsterType.Zombie: 
                        targets.Add(new Zombie(new Vec4(x, cg.GetHeightAt(x, z), z)));
                        break; 
                }
            }

            acceptString = "Could you kill " + maxProgress + " " + typeStr + " for me?";
            finishString = "Thanks for dealing with those " + typeStr + "!"; 
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
                    break;              
            }

            return false;            
        }

        public bool Update()
        {
            currentProgress++; 
            if (currentProgress == maxProgress)
            {
                return true; 
            }
            return false; 
        }
    }
}
