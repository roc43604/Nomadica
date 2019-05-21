using JModelling.JModelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Chunk
{
    public class AdorneeMesh
    {

        public Mesh mesh;
        public string Id;

        public AdorneeMesh(Mesh mesh, string Id)
        {
            this.mesh = mesh;
            this.Id = Id;
        }
    }
}
