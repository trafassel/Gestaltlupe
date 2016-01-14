using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.SceneGraph
{

    /// <summary>
    /// Point with float coordinates.
    /// </summary>
    public class FPoint
    {

        public FPoint()
        {

        }

        public FPoint(float x,float y,float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float X = 0;
        public float Y = 0;
        public float Z = 0;
    }


}
