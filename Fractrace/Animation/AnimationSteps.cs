using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Animation {



  public class AnimationSteps {

    public AnimationSteps() {

    }


    private List<AnimationPoint> mSteps = new List<AnimationPoint>();


    public List<AnimationPoint> Steps {
      get {
        return mSteps;
      }

    }
  }
}
