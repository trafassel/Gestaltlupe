using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fractrace.Basic;

namespace Fractrace.Geometry {
    public class Rotation: Transform3D{


        protected double CenterX = 0;

        protected double CenterY = 0;

        protected double CenterZ = 0;

        protected double AngleX=0;

        protected double AngleY=0;

        protected double AngleZ=0;

        public override void Init() {
            CenterX=ParameterDict.Exemplar.GetDouble("Transformation.3.CenterX");
            CenterY=ParameterDict.Exemplar.GetDouble("Transformation.3.CenterY");
            CenterZ=ParameterDict.Exemplar.GetDouble("Transformation.3.CenterZ");
            AngleX=ParameterDict.Exemplar.GetDouble("Transformation.3.AngleX");
            AngleY=ParameterDict.Exemplar.GetDouble("Transformation.3.AngleY");
            AngleZ=ParameterDict.Exemplar.GetDouble("Transformation.3.AngleZ");
        }

        public override Vec3 Transform(Vec3 input) {

            double x=input.X;
            double y=input.Y;
            double z=input.Z;

            /* Einbeziehung des Winkels  */
            double f = Math.PI / 180.0;
            /*xmi=(x1-x2)/2;ymi=(y1+y2)/2;zmi=(z1+z2)/2;*/
            // Drehung
//            xmi = 0; ymi = 0; zmi = 0;
            x -= CenterX; y -= CenterY; z -= CenterZ;
            double re = Math.Cos(AngleZ * f); 
            double im = Math.Sin(AngleZ * f);
            double a = re * x - im * y;
            y = re * y + im * x;
            x = a;
            // Neigung
            re = Math.Cos(AngleY * f); im = Math.Sin(AngleY * f);
            a = re * z - im * x;
            x = re * x + im * z;
            z = a;
            // Kippen
            re = Math.Cos(AngleX * f); im = Math.Sin(AngleX * f);
            a = re * y - im * z;
            z = re * z + im * y;
            y = a;
            x += CenterX; y += CenterY; z += CenterZ;

      return new Vec3(x,y,z);
        }
    }
}
