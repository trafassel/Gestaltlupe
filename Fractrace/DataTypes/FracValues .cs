using System;
using System.Collections.Generic;
using System.Text;

using Fractrace.Basic;

namespace Fractrace.DataTypes
{

    // Compatibility entries to original Fractrace.
    // TODO: replace start_tupel, end_tupel with center and size.

    public class FracValues
    {

        public FracValues()
        {
            initial_tupel = new XYZTupel();
            start_tupel = new XYZTupel();
            end_tupel = new XYZTupel();
            arc = new XYZTupel();
        }


        /// <summary>
        /// Liefert eine echte Kopie dieser Instanz
        /// </summary>
        /// <returns></returns>
        public FracValues Clone()
        {
            FracValues retVal = new FracValues();
            retVal.initial_tupel = initial_tupel.Clone();
            retVal.start_tupel = start_tupel.Clone();
            retVal.end_tupel = end_tupel.Clone();
            retVal.arc = arc.Clone();
            return retVal;
        }


        /// <summary>
        /// Read parameters from global ParameterDict.
        /// </summary>
        public void SetFromParameterDict()
        {
            double centerX = ParameterDict.Current.GetDouble("Scene.CenterX");
            double centerY = ParameterDict.Current.GetDouble("Scene.CenterY");
            double centerZ = ParameterDict.Current.GetDouble("Scene.CenterZ");
            double radius= ParameterDict.Current.GetDouble("Scene.Radius");

            double screenWidth = ParameterDict.Current.GetDouble("View.Width");
            double screenHeight = ParameterDict.Current.GetDouble("View.Height");
            double maxScreenSize = Math.Max(screenWidth, screenHeight);

            // diffx/diffz should be equal to screenWidth/screenHeight;
            double diffx = radius * screenWidth / maxScreenSize;
            double diffz = radius * screenHeight / maxScreenSize;
            double diffy = Math.Max(diffx, diffz);

            diffx /= 2.0; diffy /= 2.0; diffz /= 2.0;

            start_tupel.x = centerX - diffx; end_tupel.x = centerX + diffx;
            start_tupel.y = centerY - diffy; end_tupel.y = centerY + diffy;
            start_tupel.z = centerZ - diffz; end_tupel.z = centerZ + diffz;

            arc.x = ParameterDict.Current.GetDouble("Transformation.AngleX");
            arc.y = ParameterDict.Current.GetDouble("Transformation.AngleY");
            arc.z = ParameterDict.Current.GetDouble("Transformation.AngleZ");
        }


        public void TransferToParameterDict()
        {
            ParameterDict.Current.SetDouble("Scene.CenterX",(start_tupel.x+ end_tupel.x)/2.0);
            ParameterDict.Current.SetDouble("Scene.CenterY", (start_tupel.y + end_tupel.y) / 2.0);
            ParameterDict.Current.SetDouble("Scene.CenterZ", (start_tupel.z + end_tupel.z) / 2.0);

            double diffx =  end_tupel.x- start_tupel.x ;
            double diffy =  end_tupel.y- start_tupel.y;
            double diffz =  end_tupel.z- start_tupel.z;
            double maxDiff = diffx;
            if (diffy > maxDiff)
                maxDiff = diffy;
            if (diffz > maxDiff)
                maxDiff = diffz;

            ParameterDict.Current.SetDouble("Scene.Radius", maxDiff);
            ParameterDict.Current.SetDouble("Transformation.AngleX", arc.x);
            ParameterDict.Current.SetDouble("Transformation.AngleY", arc.y);
            ParameterDict.Current.SetDouble("Transformation.AngleZ", arc.z);
        }

        public XYZTupel initial_tupel = null;
        public XYZTupel start_tupel = null;
        public XYZTupel end_tupel = null;
        public XYZTupel arc = null;

        public XYZTupel Center
        {
            get
            {
                XYZTupel retVal = new XYZTupel();
                retVal.x = (end_tupel.x + start_tupel.x) / 2.0;
                retVal.y = (end_tupel.y + start_tupel.y) / 2.0;
                retVal.z = (end_tupel.z + start_tupel.z) / 2.0;
                return retVal;
            }
        }


    }
}
