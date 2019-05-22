using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Creature
{
    public class Dialogue
    {
        public string[] text;
        public int responseIndex;
        public int acceptIndex;
        public int denyIndex;
        public HashSet<int> quitIndices;

        public Dialogue(string[] text, int responseIndex, int acceptIndex, int denyIndex, int[] quitIndices)
        {
            this.text = text;
            this.responseIndex = responseIndex;
            this.acceptIndex = acceptIndex;
            this.denyIndex = denyIndex;
            this.quitIndices = new HashSet<int>(quitIndices); 
        }
    }
}
