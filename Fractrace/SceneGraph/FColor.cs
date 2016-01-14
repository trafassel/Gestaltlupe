using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.SceneGraph
{

    /// <summary>
    /// Color with float entries for red, green, blue.
    /// </summary>
    public class FColor
    {

        public FColor() { }

        public FColor(float red,float green,float blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public float Red = 0;
        public float Green = 0;
        public float Blue = 0;

    }
}
