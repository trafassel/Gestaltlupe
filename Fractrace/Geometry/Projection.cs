using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Geometry
{

    /// <summary>
    /// Use camera projection.
    /// </summary>
    public class Projection : Transform3D
    {

        public Projection(Vec3 camera, Vec3 viewPoint)
        {
            this._camera = camera;
            this._viewPoint = viewPoint;
            d = camera.Dist(viewPoint);
        }


        /// <summary>
        /// Camera position. 
        /// </summary>
        protected Vec3 _camera = null;


        /// <summary>
        /// Point to view.
        /// </summary>
        protected Vec3 _viewPoint = null;


        /// <summary>
        /// Distance from Camera to view point.
        /// </summary>
        protected double d = 0;


        /// <summary>
        /// reverse transformation.
        /// </summary>
        public override Vec3 Transform(Vec3 input)
        {
            Vec3 p1 = input.Diff(_camera);
            double dp = p1.Norm;
            double fac = dp / d;
            Vec3 p1_p = p1.Mult(fac);
            return (p1_p.Sum(_camera));
        }


        /// <summary>
        /// Reversed reverse transformation.
        /// </summary>
        public Vec3 ReverseTransform(Vec3 input)
        {
            Vec3 transformedCamera = Transform(_camera);
            Vec3 tempVec = input.Diff(transformedCamera);
            double l = tempVec.Norm;
            double dt = Math.Sqrt(l * d);
            tempVec.Normalize();
            tempVec = tempVec.Mult(dt);
            return (tempVec.Sum(transformedCamera));
        }


    }
}
