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
            start_tupel.x = ParameterDict.Exemplar.GetDouble("Border.Min.x");
            start_tupel.y = ParameterDict.Exemplar.GetDouble("Border.Min.y");
            start_tupel.z = ParameterDict.Exemplar.GetDouble("Border.Min.z");
            end_tupel.x = ParameterDict.Exemplar.GetDouble("Border.Max.x");
            end_tupel.y = ParameterDict.Exemplar.GetDouble("Border.Max.y");
            end_tupel.z = ParameterDict.Exemplar.GetDouble("Border.Max.z");

            arc.x = ParameterDict.Exemplar.GetDouble("Transformation.AngleX");
            arc.y = ParameterDict.Exemplar.GetDouble("Transformation.AngleY");
            arc.z = ParameterDict.Exemplar.GetDouble("Transformation.AngleZ");
        }
        public XYZTupel initial_tupel = null;
        public XYZTupel start_tupel = null;
        public XYZTupel end_tupel = null;
        public XYZTupel arc = null;
    }

}
