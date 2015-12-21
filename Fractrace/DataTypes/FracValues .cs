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
            start_tupel.x = ParameterDict.Current.GetDouble("Border.Min.x");
            start_tupel.y = ParameterDict.Current.GetDouble("Border.Min.y");
            start_tupel.z = ParameterDict.Current.GetDouble("Border.Min.z");
            end_tupel.x = ParameterDict.Current.GetDouble("Border.Max.x");
            end_tupel.y = ParameterDict.Current.GetDouble("Border.Max.y");
            end_tupel.z = ParameterDict.Current.GetDouble("Border.Max.z");

            arc.x = ParameterDict.Current.GetDouble("Transformation.AngleX");
            arc.y = ParameterDict.Current.GetDouble("Transformation.AngleY");
            arc.z = ParameterDict.Current.GetDouble("Transformation.AngleZ");
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
