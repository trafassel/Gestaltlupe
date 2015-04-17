using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Geometry
{
    public class Projection : Transform3D
    {


        public Projection(Vec3 camera, Vec3 viewPoint)
        {
            this.camera = camera;
            this.viewPoint = viewPoint;
            d = camera.Dist(viewPoint);
        }


        /// <summary>
        /// Kamerapunkt des Benutzers (üblicherweise parallel vor dem Zentrum des Vierecks 
        /// </summary>
        protected Vec3 camera = null;


        /// <summary>
        /// Der Punkt, der sich vor dem Kamerapunkt befindet und der auf das Zentrum des Bildschirmes
        /// plaziert werden soll.
        /// </summary>
        protected Vec3 viewPoint = null;


        /// <summary>
        /// Distance from Camera to view point.
        /// </summary>
        protected double d = 0;


        /// <summary>
        /// reverse transformation.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override Vec3 Transform(Vec3 input)
        {
            Vec3 p1 = input.Diff(camera);
            double dp = p1.Norm;
            double fac = dp / d;
            Vec3 p1_p = p1.Mult(fac);
            return (p1_p.Sum(camera));
        }


        /// <summary>
        /// Reversed reverse transformation.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Vec3 ReverseTransform(Vec3 input)
        {
            Vec3 transformedCamera = Transform(camera);
            Vec3 tempVec = input.Diff(transformedCamera);
            double l = tempVec.Norm;
            double dt = Math.Sqrt(l * d);
            tempVec.Normalize();
            tempVec = tempVec.Mult(dt);
            return (tempVec.Sum(transformedCamera));
        }


    }
}
