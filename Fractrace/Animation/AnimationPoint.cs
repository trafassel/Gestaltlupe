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


        protected int mTime = 0;



        /// <summary>
        /// Digital point in time.
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
