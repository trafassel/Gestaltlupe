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


        /// <summary>
        /// Correct area bounds to aspect ratio of result image.
        /// </summary>
        public static void SetAspectRatio()
        {
            double xmin = ParameterDict.Current.GetDouble("Border.Min.x");
            double xmax = ParameterDict.Current.GetDouble("Border.Max.x");
            if (xmin > xmax)
            {
                double temPX = xmin; xmin = xmax; xmax = temPX;
            }
            double ymin = ParameterDict.Current.GetDouble("Border.Min.y");
            double ymax = ParameterDict.Current.GetDouble("Border.Max.y");
            if (ymin > ymax)
            {
                double temPY = ymin; ymin = ymax; ymax = temPY;
            }

            double zmin = ParameterDict.Current.GetDouble("Border.Min.z");
            double zmax = ParameterDict.Current.GetDouble("Border.Max.z");
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

            double screenWidth = ParameterDict.Current.GetDouble("View.Width");
            double screenHeight = ParameterDict.Current.GetDouble("View.Height");
            double maxScreenSize = Math.Max(screenWidth, screenHeight);

            // diffx/diffz should be equal to screenWidth/screenHeight;
            diffx = maxDiff * screenWidth / maxScreenSize;
            diffz = maxDiff * screenHeight / maxScreenSize;
            diffy = Math.Max(diffx, diffz);

            diffx /= 2.0; diffy /= 2.0; diffz /= 2.0;

            xmin = centerX - diffx; xmax = centerX + diffx;
            ymin = centerY - diffy; ymax = centerY + diffy;
            zmin = centerZ - diffz; zmax = centerZ + diffz;

            ParameterDict.Current.SetDouble("Border.Min.x", xmin);
            ParameterDict.Current.SetDouble("Border.Min.y", ymin);
            ParameterDict.Current.SetDouble("Border.Min.z", zmin);
            ParameterDict.Current.SetDouble("Border.Max.x", xmax);
            ParameterDict.Current.SetDouble("Border.Max.y", ymax);
            ParameterDict.Current.SetDouble("Border.Max.z", zmax);
        }


        protected Iterate _iterate = null;

    }
}
