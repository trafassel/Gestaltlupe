using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fractrace.Animation {


   public class AnimationPoint {

       protected int mSteps = 0;

       public int Steps {
           get {
               return mSteps;
           }
           set {
               mSteps = value;
           }
       }
       protected int mTime = 0;



       /// <summary>
       /// Digitaler Zeitpunkt
       /// </summary>
       public int Time {
           get {
               return mTime;
           }
           set {
               mTime = value;
           }
       }
       
    }
}
