using System.Collections.Generic;

namespace Fractrace.Animation
{

    /// <summary>
    /// Manage list of AnimationPoints.
    /// </summary>
    public class AnimationSteps
    {

        
        public AnimationSteps()
        {
        }


        private List<AnimationPoint> _steps = new List<AnimationPoint>();


        /// <summary>
        /// List of all animation frames. 
        /// </summary>
        public List<AnimationPoint> Steps { get { return _steps; }  }


    }
}
