using JModelling.Creature.Nomad;
using JModelling.InventorySpace;
using JModelling.JModelling;
using JModelling.JModelling.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Creature
{
    /// <summary>
    /// A living thing. This defines what every living thing should
    /// have, as well as what they should do in certain scenarios. 
    /// When making a living thing, create a class that extends this; 
    /// no living thing should JUST be defined as a "creature", but
    /// instead as a "human" or "zombie". 
    /// </summary>
    public abstract class Creature : ThreeDim
    {
        private static JManager manager;
        protected static ChunkGenerator cg; 

        /// <summary>
        /// Is this creature willing to attack the player if given the
        /// chance? This usually means that they'll act differently in
        /// the Update() method, walking towards the player to attack
        /// them or shoot an arrow at them, etc. 
        /// </summary>
        public bool IsHostile;

        /// <summary>
        /// How fast this creature moves. 
        /// </summary>
        public float Speed; 

        /// <summary>
        /// How much damage this creature does when it attacks the
        /// player. A zombie deals damage by touching, an archer does
        /// damage through arrows. 
        /// </summary>
        public int Damage;

        /// <summary>
        /// How much health this creature has before dying. 
        /// </summary>
        public int Health;

        /// <summary>
        /// How far the player needs to be before it takes notice to
        /// them. 
        /// </summary>
        public int NoticeDistance;

        /// <summary>
        /// The items this creature drops when killed. 
        /// </summary>
        public List<Item> DroppedItems;

        public bool tookDamage;

        protected Vec4 gravityVelocity;
        public Vec4 TravelVector;

        /// <summary>
        /// The angle this creature makes to the player. 
        /// </summary>
        public float AngleToPlayer;

        public MonsterType type;

        public bool killed; 

        public Creature(Mesh mesh, Vec4 location, float Speed, int Damage, int Health, int NoticeDistance, List<Item> DroppedItems, MonsterType type)
            : base(location, mesh)
        {
            IsHostile = false;
            this.Speed = Speed;
            this.Damage = Damage;
            this.Health = Health;
            this.NoticeDistance = NoticeDistance;
            this.DroppedItems = DroppedItems;
            this.type = type;
            killed = false; 
        }

        public static void Init(JManager manager, ChunkGenerator cg)
        {
            Creature.manager = manager;
            Creature.cg = cg; 
        }

        public void TookDamage(Player player, bool removeInList)
        {
            Health -= player.Damage;
            tookDamage = true;
            gravityVelocity = new Vec4(0, 1, 0);

            if (Health <= 0)
            {
                Killed(removeInList); 
            }
        }

        /// <summary>
        /// Will be called every game update-cycle, and determines the
        /// actions the creature will do (for instance, walk forward,
        /// attack, talk, etc.). The ChunkGenerator is needed for 
        /// collision purposes. 
        /// </summary>
        public abstract void Update(Player player); 

        public void Killed(bool removeInList)
        {
            Vec4 itemLoc = Loc;
            itemLoc.Y = cg.GetHeightAt(itemLoc.X, itemLoc.Z) + 10;
            foreach (Item item in DroppedItems)
            {
                item.SetInWorldSpace(itemLoc);
                manager.itemsInWorld.AddLast(item);
            }
            if (removeInList)
            {
                manager.creatures.Remove(this);
            }
            killed = true;

            Quest quest = manager.player.quest; 
            if (quest.targets.Contains(this))
            {
                if (quest.Update())
                {
                    manager.compass = new GUI.Compass(manager.settlements[0].group[0].Loc);
                    manager.settlements[0].group[0].Index++; 
                }
            }
        }
    }
}
