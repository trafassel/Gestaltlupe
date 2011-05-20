using System;
using System.Collections.Generic;
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


      /// <summary>
      /// Diese Objektinstanz wird mit den Werten aus den Einstellungen befüllt.
      /// </summary>
        public override void Init() {
            CenterX=ParameterDict.Exemplar.GetDouble("Transformation.3.CenterX");
            CenterY=ParameterDict.Exemplar.GetDouble("Transformation.3.CenterY");
            CenterZ=ParameterDict.Exemplar.GetDouble("Transformation.3.CenterZ");
            AngleX=ParameterDict.Exemplar.GetDouble("Transformation.3.AngleX");
            AngleY=ParameterDict.Exemplar.GetDouble("Transformation.3.AngleY");
            AngleZ=ParameterDict.Exemplar.GetDouble("Transformation.3.AngleZ");
        }


        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="centerX">The center X.</param>
        /// <param name="centerY">The center Y.</param>
        /// <param name="centerZ">The center Z.</param>
        /// <param name="angleX">The angle X.</param>
        /// <param name="angleY">The angle Y.</param>
        /// <param name="angleZ">The angle Z.</param>
      public void Init(double centerX,double centerY,double centerZ,double angleX,double angleY,double angleZ) {
        CenterX=centerX;
        CenterY=centerY;
        CenterZ=centerZ;
        AngleX=angleX;
        AngleY=angleY;  
        AngleZ=angleZ;
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



        /// <summary>
        /// Like Transform, but with different angle combination.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public  Vec3 TransformForNavigation(Vec3 input) {

          double x = input.X;
          double y = input.Y;
          double z = input.Z;

          /* Einbeziehung des Winkels  */
          double f = Math.PI / 180.0;
          /*xmi=(x1-x2)/2;ymi=(y1+y2)/2;zmi=(z1+z2)/2;*/
          double re = 0, im = 0, a = 0;

         
          //            xmi = 0; ymi = 0; zmi = 0;
          x -= CenterX; y -= CenterY; z -= CenterZ;

          // Kippen
          re = Math.Cos(AngleX * f); im = Math.Sin(AngleX * f);
          a = re * y - im * z;
          z = re * z + im * y;
          y = a;

          // Neigung
          re = Math.Cos(AngleY * f); im = Math.Sin(AngleY * f);
          a = re * z - im * x;
          x = re * x + im * z;
          z = a;

          // Drehung
          re = Math.Cos(AngleZ * f);
          im = Math.Sin(AngleZ * f);
          a = re * x - im * y;
          y = re * y + im * x;
          x = a;

        

          x += CenterX; y += CenterY; z += CenterZ;

          return new Vec3(x, y, z);
        }
    }
}
