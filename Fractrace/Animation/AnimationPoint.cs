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
        protected int mSteps = 0;


        /// <summary>
        /// Number of interpolated animation steps from this to the next AnimationPoint. 
        /// </summary>
        public int Steps
        {
            get
            {
                return mSteps;
            }
            set
            {
                mSteps = value;
            }
        }


        /// <summary>
        /// Digital point in time.
        /// </summary>
        protected int mTime = 0;


        /// <summary>
        /// Digital point in history corresponding to this frame.
        /// </summary>
        public int Time
        {
            get
            {
                return mTime;
            }
            set
            {
                mTime = value;
            }
        }

    }
}
