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
            x = y = 0;
            z = 1;
            angle = 0;
        }





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
            double wh = angle;
            double xh = x;
            double yh = y;
            double zh = z;
            double s = Math.Sin(angle / 2);
            xh = x * s;
            yh = y * s;
            zh = z * s;
            wh = Math.Cos(angle / 2);
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
            angle = (double)(c1c2 * c3 + s1s2 * s3);
            x = (double)(c1c2 * s3 - s1s2 * c3);
            y = (double)(c1 * s2 * c3 + s1 * c2 * s3);
            z = (double)(s1 * c2 * c3 - c1 * s2 * s3);
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
            double qax = x;
            double qay = y;
            double qaz = z;
            double qaw = angle;
            double qbx;
            double qby;
            double qbz;
            double qbw;

            double s = Math.Sin(r.angle / 2);
            qbx = r.x * s;
            qby = r.y * s;
            qbz = r.z * s;
            qbw = Math.Cos(r.angle / 2);

            // now multiply the quaternions
            angle = (double)(qaw * qbw - qax * qbx - qay * qby - qaz * qbz);
            x = (double)(qax * qbw + qaw * qbx + qay * qbz - qaz * qby);
            y = (double)(qaw * qby - qax * qbz + qay * qbw + qaz * qbx);
            z = (double)(qaw * qbz + qax * qby - qay * qbx + qaz * qbw);
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
            double qax = x;
            double qay = y;
            double qaz = z;
            double qaw = angle;
            // now multiply the quaternions
            angle = (double)(qaw * qbw - qax * qbx - qay * qby - qaz * qbz);
            x = (double)(qax * qbw + qaw * qbx + qay * qbz - qaz * qby);
            y = (double)(qaw * qby - qax * qbz + qay * qbw + qaz * qbx);
            z = (double)(qaw * qbz + qax * qby - qay * qbx + qaz * qbw);
            toAxisAngle();
        }


        /// <summary>
        /// if this rotation is not already coded as axis angle then convert it to axis    angle
        /// </summary>
        protected void toAxisAngle()
        {
            double s = Math.Sqrt(1 - angle * angle);
            if (Math.Abs(s) < 0.001) s = 1;
            angle = (double)(2 * Math.Acos(angle));
            x = (double)(x / s);
            y = (double)(y / s);
            z = (double)(z / s);
        }


        /// <summary>
        /// if this rotation is not already coded as quaternion then convert it to quaternion
        /// </summary>
        protected void toQuaternion()
        {
            double s = Math.Sin(angle / 2);
            x = (double)(x * s);
            y = (double)(y * s);
            z = (double)(z * s);
            angle = (double)Math.Cos(angle / 2);
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
            double w = angle;
            // Test nach:
            // http://www.euclideanspace.com/maths/geometry/rotations/euler/AndyGoldstein.htm
            double test = 2 * w * y - 2 * x * z;
            if (test > 1)
            {
                attitude = Math.Asin(1);
            }
            else
                attitude = Math.Asin(2 * w * y - 2 * x * z);

            double test2 = 2 * w * z + 2 * x * y;
            double test3 = w * w + x * x - y * y - z * z;
            heading = Math.Atan2(2 * w * z + 2 * x * y, w * w + x * x - y * y - z * z);
            test2 = 2 * w * x + 2 * y * z;
            test3 = w * w - x * x - y * y + z * z;

            bank = Math.Atan2(2 * w * x + 2 * y * z, w * w - x * x - y * y + z * z);
            toAxisAngle();
        }

        public void Normalize()
        {
            double r = Math.Sqrt(x * x + y * y + z * z);
            x = x / r;
            y = y / r;
            z = z / r;
        }


        /// <summary>Winkel in Bogenmaß</summary>	
        public double angle = 0;
        /// <summary>x-Komponente der Drehachse</summary>	
        public double x = 0;
        /// <summary>y-Komponente der Drehachse</summary>	
        public double y = 0;
        /// <summary>z-Komponente der Drehachse</summary>	
        public double z = 1;
    }
}
