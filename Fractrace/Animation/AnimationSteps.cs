using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Animation
{


    public class AnimationSteps
    {

        /// <summary>
        /// Manage list of AnimationPoint.
        /// </summary>
        public AnimationSteps()
        {

        }


        private List<AnimationPoint> mSteps = new List<AnimationPoint>();


        /// <summary>
        /// List of all animation frames. 
        /// </summary>
        public List<AnimationPoint> Steps
        {
            get
            {
                return mSteps;
            }

        }


    }

}
