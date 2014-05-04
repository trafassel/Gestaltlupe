using System;
using System.Collections.Generic;
using System.Text;

using Fractrace.Basic;

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


    public static void SetAspectRatio() {
        double xmin = ParameterDict.Exemplar.GetDouble("Border.Min.x");
        double xmax = ParameterDict.Exemplar.GetDouble("Border.Max.x");
        if (xmin > xmax)
        {
            double temPX = xmin; xmin = xmax; xmax = temPX;
        }
        double ymin = ParameterDict.Exemplar.GetDouble("Border.Min.y");
        double ymax = ParameterDict.Exemplar.GetDouble("Border.Max.y");
        if (ymin > ymax)
        {
            double temPY = ymin; ymin = ymax; ymax = temPY;
        }
        
        double zmin = ParameterDict.Exemplar.GetDouble("Border.Min.z");
        double zmax = ParameterDict.Exemplar.GetDouble("Border.Max.z");
        if (xmin > xmax)
        {
            double temPZ = zmin; zmin = zmax; zmax = temPZ;
        }
        
        double centerX = (xmax + xmin) / 2.0;
        double centerY = (ymax + ymin) / 2.0;
        double centerZ = (zmax + zmin) / 2.0;

        double diffx = xmax - xmin;
        double diffy = ymax - zmin;
        double diffz = zmax - zmin;
        double maxDiff = diffx;
        if (diffz > maxDiff)
            maxDiff = diffz;

        double screenWidth = ParameterDict.Exemplar.GetDouble("View.Width");
        double screenHeight = ParameterDict.Exemplar.GetDouble("View.Height");
        double maxScreenSize = Math.Max(screenWidth, screenHeight);

        // diffx/diffz should be equal to screenWidth/screenHeight;
        diffx = maxDiff * screenWidth / maxScreenSize;
        diffz = maxDiff * screenHeight / maxScreenSize;
        diffy = Math.Max(diffx, diffz);

        diffx /= 2.0; diffy /= 2.0; diffz /= 2.0;

        xmin = centerX - diffx; xmax = centerX + diffx;
        ymin = centerY - diffy; ymax = centerY + diffy;
        zmin = centerZ - diffz; zmax = centerZ + diffz;

        ParameterDict.Exemplar.SetDouble("Border.Min.x", xmin);
        ParameterDict.Exemplar.SetDouble("Border.Min.y", ymin);
        ParameterDict.Exemplar.SetDouble("Border.Min.z", zmin);
        ParameterDict.Exemplar.SetDouble("Border.Max.x", xmax);
        ParameterDict.Exemplar.SetDouble("Border.Max.y", ymax);
        ParameterDict.Exemplar.SetDouble("Border.Max.z", zmax);

    }


    protected Iterate mIterate = null;
   
  }
}
