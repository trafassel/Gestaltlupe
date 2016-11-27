using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Geometry
{

    public class VecRotation
    {

        /// <summary>
        /// Default constructor - initializes all fields to default values
        /// </summary>
        public VecRotation()
        {
            X = Y = 0;
            Z = 1;
            Angle = 0;
        }

        public VecRotation(double x,double y,double z,double angle)
        {
            X = x;
            Y = y;
            Z = z;
            Angle = angle;
        }

        /// <summary>Winkel in Bogenmaß</summary>	
        public double Angle = 0;
        /// <summary>x-Komponente der Drehachse</summary>	
        public double X = 0;
        /// <summary>y-Komponente der Drehachse</summary>	
        public double Y = 0;
        /// <summary>z-Komponente der Drehachse</summary>	
        public double Z = 1;


        /// <summary>
        /// calculates the effect of this rotation on a point
        /// the new point is given by=q * P1 * q'
        /// this version does not alter P1 but returns the result.
        /// 
        /// for theory see:
        /// http://www.martinb.com/maths/algebra/realNormedAlgebra/quaternions/transforms/index.htm
        /// </summary>
        /// <param name="point">point to be transformed</param>
        /// <returns>translated point</returns>
        public Vec3 getTransform(Vec3 p1)
        {
            double wh = Angle;
            double xh = X;
            double yh = Y;
            double zh = Z;
            double s = Math.Sin(Angle / 2);
            xh = X * s;
            yh = Y * s;
            zh = Z * s;
            wh = Math.Cos(Angle / 2);
            Vec3 p2 = new Vec3(0, 0, 0);
            p2.X = (double)(wh * wh * p1.X + 2 * yh * wh * p1.Z - 2 * zh * wh * p1.Y + xh * xh * p1.X + 2 * yh * xh * p1.Y + 2 * zh * xh * p1.Z - zh * zh * p1.X - yh * yh * p1.X);
            p2.Y = (double)(2 * xh * yh * p1.X + yh * yh * p1.Y + 2 * zh * yh * p1.Z + 2 * wh * zh * p1.X - zh * zh * p1.Y + wh * wh * p1.Y - 2 * xh * wh * p1.Z - xh * xh * p1.Y);
            p2.Z = (double)(2 * xh * zh * p1.X + 2 * yh * zh * p1.Y + zh * zh * p1.Z - 2 * wh * yh * p1.X - yh * yh * p1.Z + 2 * wh * xh * p1.Y - xh * xh * p1.Z + wh * wh * p1.Z);
            return p2;
        }


        /// <summary>
        /// constructor to create vecRotation from euler angles.
        /// </summary>
        /// <param name="heading">rotation about z axis</param>
        /// <param name="attitude">rotation about y axis</param>
        /// <param name="bank">rotation about x axis</param>
        public void FromEuler(double heading, double attitude, double bank)
        {
            double c1 = Math.Cos(heading / 2);
            double s1 = Math.Sin(heading / 2);
            double c2 = Math.Cos(attitude / 2);
            double s2 = Math.Sin(attitude / 2);
            double c3 = Math.Cos(bank / 2);
            double s3 = Math.Sin(bank / 2);
            double c1c2 = c1 * c2;
            double s1s2 = s1 * s2;
            Angle = (double)(c1c2 * c3 + s1s2 * s3);
            X = (double)(c1c2 * s3 - s1s2 * c3);
            Y = (double)(c1 * s2 * c3 + s1 * c2 * s3);
            Z = (double)(s1 * c2 * c3 - c1 * s2 * s3);
            toAxisAngle();
        }


        /// <summary>
        /// calculate total rotation by taking current rotation and then
        /// apply rotation r
        /// 
        /// if both angles are quaternions then this is a multiplication
        /// </summary>
        /// <param name="r"></param>
        public void combine(VecRotation r)
        {
            toQuaternion();
            if (r == null) return;
            double qax = X;
            double qay = Y;
            double qaz = Z;
            double qaw = Angle;
           
            double s = Math.Sin(r.Angle / 2);
            double qbx = r.X * s;
            double qby = r.Y * s;
            double qbz = r.Z * s;
            double qbw = Math.Cos(r.Angle / 2);

            // now multiply the quaternions
            Angle = (double)(qaw * qbw - qax * qbx - qay * qby - qaz * qbz);
            X = (double)(qax * qbw + qaw * qbx + qay * qbz - qaz * qby);
            Y = (double)(qaw * qby - qax * qbz + qay * qbw + qaz * qbx);
            Z = (double)(qaw * qbz + qax * qby - qay * qbx + qaz * qbw);
            toAxisAngle();
        }


        // <summary>
        /// combine a rotation expressed as euler angle with current rotation.
        /// first convert both values to quaternoins then combine and convert back to    
        /// axis angle. Theory about these conversions shown here:
        /// http://www.martinb.com/maths/geometry/rotations/conversions/index.htm
        /// </summary>
        /// <param name="heading">angle about x axis</param>
        /// <param name="attitude">angle about y axis</param>
        /// <param name="bank">angle about z axis</param>
        public void combine(double heading, double attitude, double bank)
        {
            // first calculate quaternion qb from heading, attitude and bank
            double c1 = Math.Cos(heading / 2);
            double s1 = Math.Sin(heading / 2);
            double c2 = Math.Cos(attitude / 2);
            double s2 = Math.Sin(attitude / 2);
            double c3 = Math.Cos(bank / 2);
            double s3 = Math.Sin(bank / 2);
            double c1c2 = c1 * c2;
            double s1s2 = s1 * s2;
            double qbw = c1c2 * c3 + s1s2 * s3;
            double qbx = c1c2 * s3 - s1s2 * c3;
            double qby = c1 * s2 * c3 + s1 * c2 * s3;
            double qbz = s1 * c2 * c3 - c1 * s2 * s3;
            // then convert axis-angle to quaternion if required
            toQuaternion();
            double qax = X;
            double qay = Y;
            double qaz = Z;
            double qaw = Angle;
            // now multiply the quaternions
            Angle = (double)(qaw * qbw - qax * qbx - qay * qby - qaz * qbz);
            X = (double)(qax * qbw + qaw * qbx + qay * qbz - qaz * qby);
            Y = (double)(qaw * qby - qax * qbz + qay * qbw + qaz * qbx);
            Z = (double)(qaw * qbz + qax * qby - qay * qbx + qaz * qbw);
            toAxisAngle();
        }


        /// <summary>
        /// if this rotation is not already coded as axis angle then convert it to axis    angle
        /// </summary>
        protected void toAxisAngle()
        {
            double s = Math.Sqrt(1 - Angle * Angle);
            if (Math.Abs(s) < 0.001) s = 1;
            Angle = (double)(2 * Math.Acos(Angle));
            X = (double)(X / s);
            Y = (double)(Y / s);
            Z = (double)(Z / s);
        }


        /// <summary>
        /// if this rotation is not already coded as quaternion then convert it to quaternion
        /// </summary>
        protected void toQuaternion()
        {
            double s = Math.Sin(Angle / 2);
            X = (double)(X * s);
            Y = (double)(Y * s);
            Z = (double)(Z * s);
            Angle = (double)Math.Cos(Angle / 2);
        }


        /// <summary>
        /// Converts the Rotation in Euler Axis
        /// </summary>
        /// <param name="heading">The heading.</param>
        /// <param name="attitude">The attitude.</param>
        /// <param name="bank">The bank.</param>
        public void toEuler(ref double heading, ref double attitude, ref double bank)
        {
            toQuaternion();
            double w = Angle;
            // Test nach:
            // http://www.euclideanspace.com/maths/geometry/rotations/euler/AndyGoldstein.htm
            double test = 2 * w * Y - 2 * X * Z;
            if (test > 1)
            {
                attitude = Math.Asin(1);
            }
            else
                attitude = Math.Asin(2 * w * Y - 2 * X * Z);

            heading = Math.Atan2(2 * w * Z + 2 * X * Y, w * w + X * X - Y * Y - Z * Z);

            bank = Math.Atan2(2 * w * X + 2 * Y * Z, w * w - X * X - Y * Y + Z * Z);
            toAxisAngle();
        }

        public void Normalize()
        {
            double r = Math.Sqrt(X * X + Y * Y + Z * Z);
            if (r != 0)
            {
                X = X / r;
                Y = Y / r;
                Z = Z / r;
            }
        }


    }
}
