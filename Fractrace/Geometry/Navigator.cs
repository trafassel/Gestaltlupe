using System;
using System.Collections.Generic;
using System.Text;

using Fractrace.Basic;

namespace Fractrace.Geometry
{

    /// <summary>
    /// Useful methods to navigate in the scene.
    /// </summary>
    public class Navigator
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Navigator"/> class.
        /// </summary>
        public Navigator(Iterate iter)
        {

        }


        /// <summary>
        /// Initialisation with the iteration object of the last render. 
        /// </summary>
        /// <param name="iter">The iter.</param>
        public void Init(Iterate iter)
        {
            _iterate = iter;
        }


        /// <summary>
        /// Center of the area to display.
        /// </summary>
        protected Vec3 _center = new Vec3();


        protected Iterate _iterate = null;

    }
}
