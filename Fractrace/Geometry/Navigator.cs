using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Geometry {

  /// <summary>
  /// Useful methods to navigate in the scene.
  /// </summary>
  public class Navigator {


    /// <summary>
    /// Initializes a new instance of the <see cref="Navigator"/> class.
    /// </summary>
    public Navigator(Iterate iter) {
     
    }

    /// <summary>
    /// Initialisation with the iteration object of the last render. 
    /// </summary>
    /// <param name="iter">The iter.</param>
    public void Init(Iterate iter) {
      mIterate = iter;
    }



    /// <summary>
    /// Center of the area to display.
    /// </summary>
    protected Vec3 mCenter = new Vec3();


    /// <summary>
    /// Size of the area to display.
    /// </summary>
    protected double mAreaSize = 0;


    protected Iterate mIterate = null;
   
  }
}
