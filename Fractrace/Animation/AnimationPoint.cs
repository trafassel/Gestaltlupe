using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Animation
{


    /// <summary>
    /// One step in the animation time line. 
    /// </summary>
    public class AnimationPoint
    {

        /// <summary>
        /// Filename (if exist)
        /// </summary>
        public string fileName = "";


        /// <summary>
        /// Number of interpolated animation steps from this to the next AnimationPoint. 
        /// </summary>
        protected int _steps = 0;


        /// <summary>
        /// Number of interpolated animation steps from this to the next AnimationPoint. 
        /// </summary>
        public int Steps
        {
            get
            {
                return _steps;
            }
            set
            {
                _steps = value;
            }
        }


        /// <summary>
        /// Digital point in time.
        /// </summary>
        protected int _time = 0;


        /// <summary>
        /// Digital point in history corresponding to this frame.
        /// </summary>
        public int Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
            }
        }


    }
}
