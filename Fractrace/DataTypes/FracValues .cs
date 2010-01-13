using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fractrace.Basic;

namespace Fractrace.DataTypes {

    public class FracValues {

        public FracValues() {
            initial_tupel = new XYZTupel();
            start_tupel = new XYZTupel();
            end_tupel = new XYZTupel();
            arc = new XYZTupel();
        }

      /// <summary>
      /// Liefert eine echte Kopie dieser Instanz
      /// </summary>
      /// <returns></returns>
        public FracValues Clone() {
          FracValues retVal = new FracValues();
          retVal.initial_tupel = initial_tupel.Clone();
          retVal.start_tupel = start_tupel.Clone();
          retVal.end_tupel = end_tupel.Clone();
          retVal.arc = arc.Clone();
          return retVal;
        }


        public void SetToDefault() {
            initial_tupel.x = 0;
            initial_tupel.y = 0;
            initial_tupel.z = 0;

            start_tupel.x = -1.1;
            start_tupel.y = -1.1; 
            start_tupel.z = -1.1;

            end_tupel.x = 1.1;
            end_tupel.y = 1.1;
            end_tupel.z = 1.1;
           
            arc.x = 0;
            arc.y = 0;
            arc.z = 0;

        }


        /// <summary>
        /// Das Feld Parameter wird aus dem ParameterDict gelesen.
        /// </summary>
        public void SetFromParameterDict() {
            start_tupel.x = ParameterDict.Exemplar.GetDouble("Border.Min.x");
            start_tupel.y = ParameterDict.Exemplar.GetDouble("Border.Min.y");
            start_tupel.z = ParameterDict.Exemplar.GetDouble("Border.Min.z");
            start_tupel.zz = ParameterDict.Exemplar.GetDouble("Border.Min.zz");
            end_tupel.x = ParameterDict.Exemplar.GetDouble("Border.Max.x");
            end_tupel.y = ParameterDict.Exemplar.GetDouble("Border.Max.y");
            end_tupel.z = ParameterDict.Exemplar.GetDouble("Border.Max.z");
            start_tupel.zz = ParameterDict.Exemplar.GetDouble("Border.Max.zz");

            arc.x = ParameterDict.Exemplar.GetDouble("Transformation.AngleX");
            arc.y = ParameterDict.Exemplar.GetDouble("Transformation.AngleY");
            arc.z = ParameterDict.Exemplar.GetDouble("Transformation.AngleZ");
        }


        public XYZTupel initial_tupel = null;
        public XYZTupel start_tupel = null;
        public XYZTupel end_tupel = null;
        public XYZTupel arc = null;
        public void print() { }

    }
}
