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

        public Creature(Mesh mesh, Vec4 location, float Speed, int Damage, int Health, int NoticeDistance, List<Item> DroppedItems)
            : base(location, mesh)
        {
            IsHostile = false;
            this.Speed = Speed;
            this.Damage = Damage;
            this.Health = Health;
            this.NoticeDistance = NoticeDistance;
            this.DroppedItems = DroppedItems;
        }

        /// <summary>
        /// Will be called every game update-cycle, and determines the
        /// actions the creature will do (for instance, walk forward,
        /// attack, talk, etc.). The ChunkGenerator is needed for 
        /// collision purposes. 
        /// </summary>
        public abstract void Update(Player player, ChunkGenerator cg); 
    }
}
